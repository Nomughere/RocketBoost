using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToOptions : MonoBehaviour
{
    // Called by the UI Button's OnClick event
    public void LoadSceneOne()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}