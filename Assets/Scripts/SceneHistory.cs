// SceneHistory.cs
// Persistent scene navigation history with a "Go Back" API.
// Place this on an empty GameObject in your initial scene.
// It will persist across scene loads.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHistory : MonoBehaviour
{
    public static SceneHistory Instance { get; private set; }

    [Header("Behavior")]
    [Tooltip("Optional: If no previous scene exists, load this scene (by name). Leave empty to do nothing.")]
    [SerializeField] private string fallbackSceneName = "";

    [Tooltip("Log debug info for scene transitions and history.")]
    [SerializeField] private bool verboseLogging = false;

    // Stack of visited scene build indices; top is current scene.
    private readonly Stack<int> history = new Stack<int>();

    // Prevent history mutation when we are intentionally navigating back.
    private bool navigatingBack = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Seed the stack with the starting active scene.
        var active = SceneManager.GetActiveScene();
        if (active.IsValid())
        {
            history.Push(active.buildIndex);
            if (verboseLogging) Debug.Log($"[SceneHistory] Seed: {active.name} ({active.buildIndex})");
        }

        // Track subsequent loads (Single mode).
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;

        if (navigatingBack)
        {
            // We already adjusted the stack before the back load; do not push again.
            navigatingBack = false;
            if (verboseLogging) Debug.Log($"[SceneHistory] Loaded via Back: {scene.name} ({scene.buildIndex})");
            return;
        }

        // Avoid duplicate push if the same scene loaded redundantly.
        if (history.Count > 0 && history.Peek() == scene.buildIndex)
        {
            if (verboseLogging) Debug.Log($"[SceneHistory] Duplicate load ignored: {scene.name}");
            return;
        }

        history.Push(scene.buildIndex);
        if (verboseLogging) Debug.Log($"[SceneHistory] Push: {scene.name} ({scene.buildIndex}) | Count={history.Count}");
    }

    /// <summary>
    /// Attempts to go back to the previous scene in the navigation history.
    /// Returns true if a back navigation was initiated.
    /// </summary>
    public bool GoBack()
    {
        // Need at least [current, previous]
        if (history.Count < 2)
        {
            if (!string.IsNullOrEmpty(fallbackSceneName))
            {
                if (verboseLogging) Debug.Log($"[SceneHistory] No previous. Loading fallback: {fallbackSceneName}");
                navigatingBack = true; // Prevent push when fallback loads.
                SceneManager.LoadScene(fallbackSceneName, LoadSceneMode.Single);
                return true;
            }

            if (verboseLogging) Debug.Log("[SceneHistory] No previous scene to go back to.");
            return false;
        }

        // Pop current
        int current = history.Pop();

        // Peek previous (now top)
        int previous = history.Peek();

        var previousScenePath = SceneUtility.GetScenePathByBuildIndex(previous);
        if (string.IsNullOrEmpty(previousScenePath))
        {
            if (verboseLogging) Debug.LogWarning($"[SceneHistory] Previous buildIndex {previous} is invalid.");
            // Put current back to keep state consistent
            history.Push(current);
            return false;
        }

        navigatingBack = true;
        if (verboseLogging) Debug.Log($"[SceneHistory] Back: {SceneName(previous)} ({previous})");
        SceneManager.LoadScene(previous, LoadSceneMode.Single);
        return true;
    }

    private static string SceneName(int buildIndex)
    {
        var path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        if (string.IsNullOrEmpty(path)) return $"#{buildIndex}";
        int slash = path.LastIndexOf('/');
        int dot = path.LastIndexOf('.');
        if (slash >= 0 && dot > slash) return path.Substring(slash + 1, dot - slash - 1);
        return path;
    }
}