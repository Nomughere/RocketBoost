using UnityEngine;
using UnityEngine.UI;

public class VolumeToggleButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public Sprite volumeIcon;
    public Sprite mutedIcon;

    [Header("Audio Settings")]
    public bool muteMusicOnly = false;
    public AudioSource musicSource; // Assign if muteMusicOnly is true

    private bool isMuted = false;

    void Start()
    {
        // Optional: Load saved state
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        ApplyState();
    }

    public void OnButtonPress()
    {
        isMuted = !isMuted;
        ApplyState();

        // Optional: Save state
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyState()
    {
        // Change icon
        iconImage.sprite = isMuted ? mutedIcon : volumeIcon;

        // Control audio
        if (muteMusicOnly && musicSource != null)
        {
            musicSource.mute = isMuted;
        }
        else
        {
            AudioListener.pause = isMuted;
        }
    }
}