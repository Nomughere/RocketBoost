using UnityEngine;

public class GoBackButton : MonoBehaviour
{
    // Called by the UI Button's OnClick event
    public void OnBackPressed()
    {
        if (SceneHistory.Instance != null)
        {
            bool success = SceneHistory.Instance.GoBack();
            if (!success)
            {
                Debug.Log("[GoBackButton] No previous scene to go back to.");
            }
        }
        else
        {
            Debug.LogWarning("[GoBackButton] SceneHistory instance not found.");
        }
    }
}