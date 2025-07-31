using UnityEngine;

public class Soundcontroller : MonoBehaviour
{
    //Design pattern: singleton
    public static Soundcontroller instance;
    private void Awake()
    {
          if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject); // Giữ lại khi chuyển scene
    }
    public void PlaySound(string clipName, float volume = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + clipName);
        if (clip == null)
        {
            Debug.LogWarning("Không tìm thấy âm thanh: " + clipName);
            return;
        }

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
        Destroy(audioSource, clip.length); // Xóa AudioSource sau khi phát
    }
}
