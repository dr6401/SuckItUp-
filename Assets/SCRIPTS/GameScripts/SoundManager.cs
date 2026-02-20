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
    [SerializeField] public AudioSource footstepsSFXSource;
    [SerializeField] public AudioSource vacuumingSFXSource;
    [SerializeField] private AudioSource dustPickupSFXSource;
    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip[] musicThemes;
    [SerializeField] private AudioClip[] dustParticlesCrumblings;
    [SerializeField] private AudioClip[] shootingSFX;
    [SerializeField] private AudioClip[] vacuumingSounds;
    [SerializeField] private AudioClip[] mouseClicks;
    [SerializeField] private AudioClip[] hitMarkerSFX;
    [SerializeField] private AudioClip[] takeBludgeoingDamageSFX;
    [SerializeField] private AudioClip[] takeProjectileDamageSFX;
    [SerializeField] private AudioClip[] playerDeathSFX;
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
        if (musicSource == null || musicThemes.Length <= 0) return;
        musicSource.clip = musicThemes[0];
        musicSource.Play();
    }

    void Update()
    {
        currentSuctionSFXTimer += Time.deltaTime;
    }
    
    
    public void PlayStartVacuuming()
    {
        vacuumingSFXSource.pitch = 1f;
        vacuumingSFXSource.PlayOneShot(vacuumingSounds[0]);
        vacuumingSFXSource.clip = vacuumingSounds[1];
        vacuumingSFXSource.PlayDelayed(vacuumingSounds[0].length - 0.25f);
        //SFX.PlayOneShot(vacuumingSounds[1]);
    }
    
    public void PlayEndVacuuming()
    {
        vacuumingSFXSource.pitch = 1f;
        vacuumingSFXSource.PlayOneShot(vacuumingSounds[2]);
        StartCoroutine(StopPlayingVacuumingAfterDelay(0.03f));
    }

    public IEnumerator StopPlayingVacuumingAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        vacuumingSFXSource.clip = null;
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
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MasterVolume", GameConstants.defaultMusicVolume), 0.0001f, 1f)) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MusicVolume", GameConstants.defaultMusicVolume), 0.0001f, 1f)) * 20 - 5);
        audioMixer.SetFloat("GeneralSFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("GeneralSFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);
        if(PlayerPrefs.GetFloat("MasterVolume") < 0.001f)
        {
            audioMixer.SetFloat("MasterVolume", -80f);
        }
        if(PlayerPrefs.GetFloat("MusicVolume") < 0.001f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        if(PlayerPrefs.GetFloat("GeneralSFXVolume") < 0.001f)
        {
            audioMixer.SetFloat("GeneralSFXVolume", -80f);
        }
        musicSource.volume = 1;
        generalSFXSource.volume = 1;
        footstepsSFXSource.volume = 1;
        vacuumingSFXSource.volume = 0.65f; // When player is parkouring/moving around I don't want the vacuum to overshadow all other sfx
        dustPickupSFXSource.volume = 0.5f; // Dust pickup sfx is a bit loud so just half this lol
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

    private void PlayTakeDamageSFX(int dmg, DamageTypes.DamageType damageType)
    {
        generalSFXSource.pitch = Random.Range(0.95f, 1.2f);
        if (damageType == DamageTypes.DamageType.Bludgeoning)
        {
            generalSFXSource.PlayOneShot(takeBludgeoingDamageSFX[Random.Range(0, takeBludgeoingDamageSFX.Length)]);   
        }
        else if (damageType == DamageTypes.DamageType.Projectile)
        {
            generalSFXSource.PlayOneShot(takeProjectileDamageSFX[Random.Range(0, takeProjectileDamageSFX.Length)]);
        }
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

    private void PlayPlayedDeathSFX()
    {
        generalSFXSource.pitch = Random.Range(0.95f, 1.05f);
        generalSFXSource.PlayOneShot(playerDeathSFX[Random.Range(0, playerDeathSFX.Length)]);
    }
    
    
    private void OnEnable()
    {
        GameEvents.OnShoot += PlayRandomShootSFX;
        GameEvents.OnHit += PlayRandomHitMarkerSFX;
        WeaponHandler.OnNoAmmoLeft += PlayNoAmmoLeftSFX;
        GameEvents.OnEnemyDeath += PlayEnemyDeathSFX;
        GameEvents.OnSuckDust += PlayDustIncreaseSFX;
        GameEvents.OnDamageTaken += PlayTakeDamageSFX;
        GameEvents.OnPlayerDeath += PlayPlayedDeathSFX;
        //GameEvents.OnFTUETriggered += PlayEndVacuuming;
    }
    
    private void OnDisable()
    {
        GameEvents.OnShoot -= PlayRandomShootSFX;
        GameEvents.OnHit -= PlayRandomHitMarkerSFX;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeftSFX;
        GameEvents.OnEnemyDeath -= PlayEnemyDeathSFX;
        GameEvents.OnSuckDust -= PlayDustIncreaseSFX;
        GameEvents.OnDamageTaken -= PlayTakeDamageSFX;
        GameEvents.OnPlayerDeath -= PlayPlayedDeathSFX;
        //GameEvents.OnFTUETriggered -= PlayEndVacuuming;
    }
}
