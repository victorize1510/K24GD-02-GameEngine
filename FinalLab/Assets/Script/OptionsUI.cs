using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Text musicValueText;   // optional
    public TMP_Text sfxValueText;     // optional

    const string KEY_MUSIC = "opt_music";
    const string KEY_SFX   = "opt_sfx";

    void OnEnable()
    {
        float defM = SoundManager.Instance ? SoundManager.Instance.musicVolume : 0.8f;
        float defS = SoundManager.Instance ? SoundManager.Instance.sfxVolume   : 1.0f;

        float m = PlayerPrefs.GetFloat(KEY_MUSIC, defM);
        float s = PlayerPrefs.GetFloat(KEY_SFX,   defS);

        // set slider theo volume hiện tại mà KHÔNG kích hoạt OnValueChanged
        if (musicSlider) { musicSlider.SetValueWithoutNotify(m); UpdateMusicLabel(m); }
        if (sfxSlider)   { sfxSlider.SetValueWithoutNotify(s);   UpdateSfxLabel(s);   }

        // đảm bảo SoundManager dùng đúng giá trị đã lưu
        SoundManager.Instance?.SetMusicVolume(m);
        SoundManager.Instance?.SetSfxVolume(s);
    }

    public void OnMusicChanged(float v)
    {
        SoundManager.Instance?.SetMusicVolume(v);
        PlayerPrefs.SetFloat(KEY_MUSIC, v);
        UpdateMusicLabel(v);
    }

    public void OnSfxChanged(float v)
    {
        SoundManager.Instance?.SetSfxVolume(v);
        PlayerPrefs.SetFloat(KEY_SFX, v);
        UpdateSfxLabel(v);

        // feedback nhỏ
        if (Application.isPlaying) SoundManager.Instance?.PlayCoin();
    }

    void UpdateMusicLabel(float v){ if (musicValueText) musicValueText.text = Mathf.RoundToInt(v*100)+"%"; }
    void UpdateSfxLabel(float v){ if (sfxValueText) sfxValueText.text = Mathf.RoundToInt(v*100)+"%"; }
}