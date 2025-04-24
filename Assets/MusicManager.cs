using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public void toggleVolume()
    {
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer != null)
        {
            AudioSource audioSource = musicPlayer.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.mute = !audioSource.mute;
            }
        }
        else
        {
            Debug.LogWarning("MusicPlayer not found in the scene.");
        }
    }
}
