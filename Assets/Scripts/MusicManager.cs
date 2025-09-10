using System.Collections; 

using UnityEngine; 

using UnityEngine.SceneManagement; 

 

public class MusicManager : MonoBehaviour 

{ 

    private static MusicManager _instance; 

 

    // Two sources for smooth crossfades 

    private AudioSource _a; 

    private AudioSource _b; 

    private AudioSource _current; 

    private AudioSource _next; 

 

    void Awake() 

    { 

        if (_instance != null && _instance != this) 

        { 

            Destroy(gameObject); 

            return; 

        } 

        _instance = this; 

        DontDestroyOnLoad(gameObject); 

 

        // Create/prepare audio sources 

        _a = gameObject.AddComponent<AudioSource>(); 

        _b = gameObject.AddComponent<AudioSource>(); 

        _a.loop = _b.loop = true; 

        _a.playOnAwake = _b.playOnAwake = false; 

 

        _current = _a; 

        _next = _b; 

 

        SceneManager.sceneLoaded += OnSceneLoaded; 

    } 

 

    private void Start() 

    { 

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);  

    } 

 

    private void OnDestroy() 

    { 

        if (_instance == this) 

            SceneManager.sceneLoaded -= OnSceneLoaded; 

    } 

 

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 

    { 

        // Find the level’s requested track 

        var levelMusic = FindObjectOfType<LevelMusic>(); 

        if (levelMusic == null || levelMusic.Clip == null) 

            return; // no music change for this scene 

 

        // If we’re already playing this clip, do nothing (helps on death/reload) 

        if (_current.clip == levelMusic.Clip && _current.isPlaying) 

            return; 

 

        CrossfadeTo(levelMusic.Clip, levelMusic.Volume, levelMusic.FadeSeconds); 

    } 

 

    public void CrossfadeTo(AudioClip newClip, float targetVol = 1f, float fadeSeconds = 1f) 

    { 

        // Swap roles 

        var temp = _current; 

        _current = _next; 

        _next = temp; 

 

        // Prepare next 

        _current.clip = newClip; 

        _current.volume = 0f; 

        _current.Play(); 

 

        StopAllCoroutines(); 

        StartCoroutine(FadeRoutine(_current, _next, targetVol, fadeSeconds)); 

    } 

 

    private IEnumerator FadeRoutine(AudioSource fadeIn, AudioSource fadeOut, float targetVol, float seconds) 

    { 

        float t = 0f; 

        float startOut = fadeOut.volume; 

        float startIn = fadeIn.volume; 

 

        // Avoid divide by zero + snap if instant 

        if (seconds <= 0.001f) 

        { 

            fadeOut.Stop(); 

            fadeIn.volume = targetVol; 

            yield break; 

        } 

 

        while (t < seconds) 

        { 

            t += Time.unscaledDeltaTime; // use unscaled to ignore timescale pauses if you wish 

            float k = Mathf.Clamp01(t / seconds); 

            fadeIn.volume = Mathf.Lerp(startIn, targetVol, k); 

            fadeOut.volume = Mathf.Lerp(startOut, 0f, k); 

            yield return null; 

        } 

 

        fadeIn.volume = targetVol; 

        fadeOut.Stop(); 

        fadeOut.clip = null; 

    } 

} 