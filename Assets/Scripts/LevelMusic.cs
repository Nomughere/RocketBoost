using UnityEngine; 

 

public class LevelMusic : MonoBehaviour 

{ 

    [Header("Track for this level")] 

    [SerializeField] AudioClip clip; 

 

    [Range(0f, 1f)] [SerializeField] float volume = 1f; 

 

    [Tooltip("Seconds to crossfade from previous track")] 

    [SerializeField] float fadeSeconds = 1f; 

 

    //public getters for MusicManager to access 

    public AudioClip Clip => clip; 

    public float Volume => volume; 

    public float FadeSeconds => fadeSeconds; 

 

} 