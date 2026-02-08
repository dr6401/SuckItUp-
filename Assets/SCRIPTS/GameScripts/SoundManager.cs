using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    [Header("-------Audio Mixer-------")]
    [SerializeField] private AudioMixer audioMixer;
    [Header("-------Audio Sources-------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] public AudioSource generalSFXSource;
    [SerializeField] private AudioSource dustPickupSFXSource;
    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip[] musicThemes;
    [SerializeField] private AudioClip[] dustParticlesCrumblings;
    [SerializeField] private AudioClip[] shootingSFX;
    [SerializeField] private AudioClip[] vacuumingSounds;
    [SerializeField] private AudioClip[] mouseClicks;
    [SerializeField] private AudioClip[] hitMarkerSFX;
    [SerializeField] private AudioClip enemyDeathSFX;
    [SerializeField] private AudioClip dustIncreaseSFX;
    [SerializeField] private AudioClip noAmmoLeftSFX;
    private AudioClip[][] SFXSounds;
    private static SoundManager instance;
    private readonly float fadeDuration = 0.1f;
    private float currentMusicVolume;
    private float currentSFXVolume;
    private float currentSuctionSFXTimer = 0f;
    private float maxSuctionSFXTimer = 0.5f;

    private float lastShootSfxPlayedTime = 0;
    private float lastDustIncreaseSfxPlayedTime = 0;
    private float shootSfxTimeInterval = 0.06f;
    private float dustIncreaseSfxTimeInterval = 0.02f;

    private float dustPickupSFXSourceBasePitch = 0.5f;
    private int dustPickupComboStreak = 0;
    private float dustIncreaseToStreakInterval = 0.5f;
    private float dustIncreasePitchIncrease = 0.05f;
    private float maxDustIncreasePitch = 2.5f;
    
    // Pooling
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    private int sfxPoolSize = 20;
    private int poolIndex = 0;
    [SerializeField] private AudioSource[] sfxAudioSourcePool;

    
    private void Awake()
    {
        sfxAudioSourcePool = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            sfxAudioSourcePool[i] = gameObject.AddComponent<AudioSource>();
            sfxAudioSourcePool[i].playOnAwake = false;
            sfxAudioSourcePool[i].loop = false;
            sfxAudioSourcePool[i].outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    private void Start()
    {
        SetupAudio();
    }
    
    public void PlayMainTheme()
    {
        musicSource.clip = musicThemes[0];
        musicSource.Play();
    }

    void Update()
    {
        currentSuctionSFXTimer += Time.deltaTime;
    }

    public void PlaySFX(int clipNum)
    {
        generalSFXSource.pitch = 1f;
        generalSFXSource.PlayOneShot(SFXSounds[clipNum][Random.Range(0, SFXSounds[clipNum].Length - 1)]);
    }
    
    public void PlayStartVacuuming()
    {
        generalSFXSource.pitch = 1f;
        generalSFXSource.PlayOneShot(vacuumingSounds[0]);
        generalSFXSource.clip = vacuumingSounds[1];
        generalSFXSource.PlayDelayed(vacuumingSounds[0].length - 0.25f);
        //SFX.PlayOneShot(vacuumingSounds[1]);
    }
    public void PlayVacuuming()
    {
        generalSFXSource.pitch = 1f;
        generalSFXSource.PlayOneShot(vacuumingSounds[1]);
    }
    
    public void PlayEndVacuuming()
    {
        generalSFXSource.pitch = 1f;
        generalSFXSource.PlayOneShot(vacuumingSounds[2]);
        StartCoroutine(StopPlayingAfterDelay(0.03f));
    }

    public IEnumerator PlaySFXAfterDelay(AudioClip audioClip, float time)
    {
        yield return new WaitForSeconds(time);
        generalSFXSource.pitch = 1f;
        generalSFXSource.PlayOneShot(audioClip);
    }
    public IEnumerator StopPlayingAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        generalSFXSource.clip = null;
    }

    public void PlayDustSuction()
    {
        if (currentSuctionSFXTimer >= maxSuctionSFXTimer)
        {
            generalSFXSource.pitch = 1f;
            generalSFXSource.PlayOneShot(dustParticlesCrumblings[Random.Range(0, dustParticlesCrumblings.Length)]);
            currentSuctionSFXTimer = 0;
        }
    }

    public float GetMusicVolume()
    {
        return currentMusicVolume;
    }

    public void ChangeMusicVolume(float value)
    {
        StartCoroutine(FadeAudio(musicSource, value, fadeDuration));
        currentMusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public float GetSFXVolume()
    {
        return currentSFXVolume;
    }

    public void ChangeSFXVolume(float value)
    {
        StartCoroutine(FadeAudio(generalSFXSource, value, fadeDuration));
        currentSFXVolume = value;
        PlayerPrefs.SetFloat("GeneralSFXVolume", value);
    }

    public string GetMusicName()
    {
        return musicSource.clip.name;
    }

    private void SetupAudio()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MusicVolume", GameConstants.defaultMusicVolume), 0.0001f, 1f)) * 20 - 10);
        audioMixer.SetFloat("GeneralSFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("GeneralSFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);
        audioMixer.SetFloat("DustPickupSFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("GeneralSFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);        
        if (PlayerPrefs.GetFloat("MusicVolume") < 0.001f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        if (PlayerPrefs.GetFloat("GeneralSFXVolume") < 0.001f)
        {
            audioMixer.SetFloat("GeneralSFXVolume", -80f);
            audioMixer.SetFloat("DustPickupSFXVolume", -80f);
        }
        musicSource.volume = 1;
        generalSFXSource.volume = 1;
        dustPickupSFXSource.volume = 1;
        musicSource.loop = true;
        PlayMainTheme();
        SFXSounds = new AudioClip[][] { dustParticlesCrumblings, mouseClicks, vacuumingSounds };
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    private void OnEnable()
    {
        GameEvents.OnShoot += PlayRandomShootSFX;
        GameEvents.OnHit += PlayRandomHitMarkerSFX;
        WeaponHandler.OnNoAmmoLeft += PlayNoAmmoLeftSFX;
        GameEvents.OnEnemyDeath += PlayEnemyDeathSFX;
        GameEvents.OnSuckDust += PlayDustIncreaseSFX;
    }
    
    private void OnDisable()
    {
        GameEvents.OnShoot -= PlayRandomShootSFX;
        GameEvents.OnHit -= PlayRandomHitMarkerSFX;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeftSFX;
        GameEvents.OnEnemyDeath -= PlayEnemyDeathSFX;
        GameEvents.OnSuckDust -= PlayDustIncreaseSFX;
    }

    private void PlayDustIncreaseSFX()
    {
        if (dustIncreaseSFX == null) return;
        if (Time.time - lastDustIncreaseSfxPlayedTime < dustIncreaseSfxTimeInterval) return;
        if (Time.time - lastDustIncreaseSfxPlayedTime < dustIncreaseToStreakInterval)
        {
            dustPickupComboStreak++;
        }
        else dustPickupComboStreak = 0;
        lastDustIncreaseSfxPlayedTime = Time.time;
        dustPickupSFXSource.pitch = dustPickupSFXSourceBasePitch + dustPickupComboStreak * dustIncreasePitchIncrease + Random.Range(0, 0.05f);
        dustPickupSFXSource.pitch = Mathf.Clamp(dustPickupSFXSource.pitch, dustPickupSFXSource.pitch, maxDustIncreasePitch);
        dustPickupSFXSource.PlayOneShot(dustIncreaseSFX);
    }

    private void PlayNoAmmoLeftSFX()
    {
        if (noAmmoLeftSFX != null)
        {
            //SFX.pitch = 1f;
            //SFX.PlayOneShot(noAmmoLeftSFX);
        }
    }
    
    private void PlayRandomShootSFX()
    {
        if (Time.time - lastShootSfxPlayedTime < shootSfxTimeInterval) return;
        lastShootSfxPlayedTime = Time.time;
        if (sfxPoolSize > 0)
        {
            var source = sfxAudioSourcePool[poolIndex];
            source.pitch = 1f;
            source.PlayOneShot(shootingSFX[Random.Range(0, shootingSFX.Length)]);
            poolIndex = (poolIndex + 1) % sfxPoolSize;
        }
        else
        {
            generalSFXSource.pitch = 1f;
            generalSFXSource.PlayOneShot(shootingSFX[Random.Range(0, shootingSFX.Length)]);
            //sfxAudioSource.pitch = 1f;
        }
    }
    
    private void PlayRandomHitMarkerSFX()
    {
        generalSFXSource.pitch = 1f;
        generalSFXSource.PlayOneShot(hitMarkerSFX[Random.Range(0, hitMarkerSFX.Length)]);
    }

    private void PlayEnemyDeathSFX()
    {
        generalSFXSource.pitch = Random.Range(0.9f, 1.3f);
        generalSFXSource.PlayOneShot(enemyDeathSFX);
    }
}
