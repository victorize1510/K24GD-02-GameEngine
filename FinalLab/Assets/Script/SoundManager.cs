using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-200)]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Mixer (optional)")]
    public AudioMixer mixer;                 // có thì kéo vào
    public AudioMixerGroup musicGroup;       // group Music
    public AudioMixerGroup sfxGroup;         // group SFX

    [Header("Music")]
    public bool playMusicOnAwake = false;
    public AudioClip defaultMusic;           // nhạc nền Level 1
    [Range(0f,1f)] public float musicVolume = 0.8f;
    public float musicFadeTime = 0.5f;

    [Header("SFX Clips")]
    public AudioClip sfxAttack;
    public AudioClip sfxJump;
    public AudioClip sfxCoin;

    [Header("SFX Settings")]
    [Range(0f,1f)] public float sfxVolume = 1f;
    [Range(0.1f,3f)] public float pitchMin = 0.95f;
    [Range(0.1f,3f)] public float pitchMax = 1.05f;
    public int sfxPoolSize = 8;

    AudioSource musicA, musicB;              // crossfade đôi
    List<AudioSource> sfxPool = new List<AudioSource>();
    int nextSfxIndex = 0;
    bool usingA = true;
    Coroutine fadeCo;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Music sources
        musicA = gameObject.AddComponent<AudioSource>();
        musicB = gameObject.AddComponent<AudioSource>();
        SetupMusicSource(musicA);
        SetupMusicSource(musicB);

        // SFX pool
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.outputAudioMixerGroup = sfxGroup ? sfxGroup : null;
            sfxPool.Add(src);
        }

        if (playMusicOnAwake && defaultMusic)
            PlayMusic(defaultMusic, true, musicFadeTime);
    }

    void SetupMusicSource(AudioSource src)
    {
        src.playOnAwake = false;
        src.loop = true;
        src.volume = 0f;
        src.outputAudioMixerGroup = musicGroup ? musicGroup : null;
    }

    AudioSource GetNextSfxSource()
    {
        var src = sfxPool[nextSfxIndex];
        nextSfxIndex = (nextSfxIndex + 1) % sfxPool.Count;
        return src;
    }

    // ===================== MUSIC =====================
    public void PlayMusic(AudioClip clip, bool loop = true, float fade = 0.5f)
    {
        if (clip == null) return;

        var from = usingA ? musicA : musicB;
        var to   = usingA ? musicB : musicA;
        usingA = !usingA;

        to.clip = clip;
        to.loop = loop;
        to.volume = 0f;
        to.Play();

        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(Crossfade(from, to, fade));
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to, float time)
    {
        float t = 0f;
        float startFrom = from.volume;
        float targetTo  = musicVolume;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = time > 0f ? t / time : 1f;
            from.volume = Mathf.Lerp(startFrom, 0f, k);
            to.volume   = Mathf.Lerp(0f, targetTo, k);
            yield return null;
        }
        from.Stop();
        from.volume = 0f;
        to.volume = targetTo;
        fadeCo = null;
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        (usingA ? musicA : musicB).volume = musicVolume;
    }

    public void StopMusic(float fade = 0.3f)
    {
        var cur = usingA ? musicA : musicB;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeOut(cur, fade));
    }

    IEnumerator FadeOut(AudioSource src, float time)
    {
        float t = 0f;
        float start = src.volume;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = time > 0f ? t / time : 1f;
            src.volume = Mathf.Lerp(start, 0f, k);
            yield return null;
        }
        src.Stop();
        src.volume = 0f;
        fadeCo = null;
    }

    // ====================== SFX ======================
    public void PlaySFX(AudioClip clip, float vol = 1f)
    {
        if (!clip) return;
        var src = GetNextSfxSource();
        src.pitch  = Random.Range(pitchMin, pitchMax);
        src.volume = sfxVolume * Mathf.Clamp01(vol);
        src.PlayOneShot(clip);
    }

    // convenience wrappers
    public void PlayAttack() => PlaySFX(sfxAttack);
    public void PlayJump()   => PlaySFX(sfxJump);
    public void PlayCoin()   => PlaySFX(sfxCoin);

    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
    }
}