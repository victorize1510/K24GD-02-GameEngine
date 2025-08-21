using UnityEngine;

public class MyAudio : MonoBehaviour
{
    public AudioSource BackgroundAudioSource;
    public AudioSource EffectAudioSource;

    public AudioClip backgroundclip;
    public AudioClip jumpclip;
    public AudioClip coinclip;
    void Start()
    {
        PlayBlackGroundMusic();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayBlackGroundMusic()
    {
        BackgroundAudioSource.clip = backgroundclip;
        BackgroundAudioSource.Play();
    }
    public void PlayCoinSound()
    {
        EffectAudioSource.PlayOneShot(coinclip);
    }
    public void PlayJumpSound()
    {
        EffectAudioSource.PlayOneShot(jumpclip);
    }
}