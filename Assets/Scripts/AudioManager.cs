using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton для глобального доступа

    public AudioSource musicSource; // Источник музыки
    public AudioSource sfxSource;   // Источник звуков
    //Музыка
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    //Эффекты
    public AudioClip buttonClickSFX;
    public AudioClip successSFX;
    public AudioClip failSFX;
    public AudioClip gameOver;

    private float musicBalance = 0.55f; // Баланс музыки (по умолчанию 70%)
    private float sfxBalance = 0.45f;   // Баланс эффектов (по умолчанию 30%)
    private float masterVolume = 1.0f; // Общая громкость (ползунок)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyVolume(); // Применяем громкость при старте
    }

    // Устанавливаем баланс громкости (настраивается вручную)
    public void SetBalance(float music, float sfx)
    {
        float total = music + sfx;
        musicBalance = music / total;
        sfxBalance = sfx / total;
        ApplyVolume();
    }

    // Устанавливаем общую громкость (ползунок)
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
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



}
