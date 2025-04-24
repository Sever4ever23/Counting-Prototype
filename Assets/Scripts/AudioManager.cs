using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton для глобального доступа

    public AudioSource musicSource; // Источник музыки
    public AudioSource sfxSource;   // Источник звуков
    public Slider volumeSlider; // Ссылка на ползунок громкости

    // Музыка
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    // Эффекты
    public AudioClip buttonClickSFX;
    public AudioClip successSFX;
    public AudioClip failSFX;
    public AudioClip gameOver;

    private float musicBalance = 0.55f; // Баланс музыки (по умолчанию 55%)
    private float sfxBalance = 0.45f;   // Баланс эффектов (по умолчанию 45%)
    private float masterVolume = 1.0f;  // Общая громкость (по умолчанию 100%)

    private void Start()
    {
        // Загружаем сохранённые настройки громкости, если они есть
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.6f); // По умолчанию 0.6
        musicBalance = PlayerPrefs.GetFloat("MusicBalance", 0.55f); // По умолчанию 55%
        sfxBalance = PlayerPrefs.GetFloat("SFXBalance", 0.45f); // По умолчанию 45%

        // Устанавливаем ползунок в соответствии с сохранённым значением громкости
        if (volumeSlider != null)
        {
            volumeSlider.value = masterVolume; // Присваиваем значение ползунка
        }

        ApplyVolume(); // Применяем громкость при старте
    }

    // Устанавливаем баланс громкости (настраивается вручную)
    public void SetBalance(float music, float sfx)
    {
        float total = music + sfx;
        musicBalance = music / total;
        sfxBalance = sfx / total;

        // Сохраняем значения баланса в PlayerPrefs
        PlayerPrefs.SetFloat("MusicBalance", musicBalance);
        PlayerPrefs.SetFloat("SFXBalance", sfxBalance);

        ApplyVolume();
    }

    // Устанавливаем общую громкость (ползунок)
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume); // Сохраняем громкость

        ApplyVolume();
    }

    // Применяем громкость с учетом баланса
    private void ApplyVolume()
    {
        musicSource.volume = masterVolume * musicBalance;
        sfxSource.volume = masterVolume * sfxBalance;
    }

    // Метод для проигрывания звука эффекта
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSFX);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        // Если используешь AudioSource для музыки:
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        // Если музыка уже играет, продолжаем её
        if (!musicSource.isPlaying)
            musicSource.UnPause();
    }
}
