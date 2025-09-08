using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonLoader : MonoBehaviour
{
    [Header("Optional: override next scene by name")]
    [SerializeField] private string overrideNextSceneName = ""; // Leave empty to use build index order

    // Hook this up to the Button's OnClick() event
    public void OnStartButtonPressed()
    {
        // If a scene name is provided, try to load that
        if (!string.IsNullOrWhiteSpace(overrideNextSceneName))
        {
            LoadByName(overrideNextSceneName);
            return;
        }

        // Otherwise, load the next scene by build index
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("StartButtonLoader: No next scene found. Add more scenes to Build Settings or set an override name.");
        }
    }

    private void LoadByName(string sceneName)
    {
        // Basic guard: verify the scene exists in build (optional but helpful)
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogWarning($"StartButtonLoader: Scene '{sceneName}' is not in Build Settings. Add it before loading.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // Utility: check if scene is in build settings
    private bool IsSceneInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
}