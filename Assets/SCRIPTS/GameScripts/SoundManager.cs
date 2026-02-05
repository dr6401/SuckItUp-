using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("-------Audio Mixer-------")]
    [SerializeField] private AudioMixer audioMixer;
    [Header("-------Audio Sources-------")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource SFX;
    [SerializeField] private AudioSource loudSFX;
    [Header("-------Audio Clips-------")]
    [SerializeField] private AudioClip[] musicThemes;
    [SerializeField] private AudioClip[] dustParticlesCrumblings;
    [SerializeField] private AudioClip[] shootingSFX;
    [SerializeField] private AudioClip[] vacuumingSounds;
    [SerializeField] private AudioClip[] mouseClicks;
    [SerializeField] private AudioClip[] hitMarkerSFX;
    [SerializeField] private AudioClip enemyDeathSFX;
    [SerializeField] private AudioClip noAmmoLeftSFX;
    private AudioClip[][] SFXSounds;
    private static SoundManager instance;
    private readonly float fadeDuration = 0.1f;
    private float currentMusicVolume;
    private float currentSFXVolume;
    private float currentSuctionSFXTimer = 0f;
    private float maxSuctionSFXTimer = 0.5f;

    private float lastShootSfxPlayedTime = 0;
    private float shootSfxTimeInterval = 0.06f;
    
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
        music.clip = musicThemes[0];
        music.Play();
    }

    void Update()
    {
        currentSuctionSFXTimer += Time.deltaTime;
    }

    public void PlaySFX(int clipNum)
    {
        SFX.pitch = 1f;
        SFX.PlayOneShot(SFXSounds[clipNum][Random.Range(0, SFXSounds[clipNum].Length - 1)]);
    }
    
    public void PlayStartVacuuming()
    {
        SFX.pitch = 1f;
        SFX.PlayOneShot(vacuumingSounds[0]);
        SFX.clip = vacuumingSounds[1];
        SFX.PlayDelayed(vacuumingSounds[0].length - 0.25f);
        //SFX.PlayOneShot(vacuumingSounds[1]);
    }
    public void PlayVacuuming()
    {
        SFX.pitch = 1f;
        SFX.PlayOneShot(vacuumingSounds[1]);
    }
    
    public void PlayEndVacuuming()
    {
        SFX.pitch = 1f;
        SFX.PlayOneShot(vacuumingSounds[2]);
        StartCoroutine(StopPlayingAfterDelay(0.03f));
    }

    public IEnumerator PlaySFXAfterDelay(AudioClip audioClip, float time)
    {
        yield return new WaitForSeconds(time);
        SFX.pitch = 1f;
        SFX.PlayOneShot(audioClip);
    }
    public IEnumerator StopPlayingAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        SFX.clip = null;
    }

    public void PlayDustSuction()
    {
        if (currentSuctionSFXTimer >= maxSuctionSFXTimer)
        {
            SFX.pitch = 1f;
            loudSFX.PlayOneShot(dustParticlesCrumblings[Random.Range(0, dustParticlesCrumblings.Length)]);
            currentSuctionSFXTimer = 0;
        }
    }

    public float GetMusicVolume()
    {
        return currentMusicVolume;
    }

    public void ChangeMusicVolume(float value)
    {
        StartCoroutine(FadeAudio(music, value, fadeDuration));
        currentMusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public float GetSFXVolume()
    {
        return currentSFXVolume;
    }

    public void ChangeSFXVolume(float value)
    {
        StartCoroutine(FadeAudio(SFX, value, fadeDuration));
        currentSFXVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public string GetMusicName()
    {
        return music.clip.name;
    }

    private void SetupAudio()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MusicVolume", GameConstants.defaultMusicVolume), 0.0001f, 1f)) * 20 - 10);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("SFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);
        if (PlayerPrefs.GetFloat("MusicVolume") < 0.001f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        if (PlayerPrefs.GetFloat("SFXVolume") < 0.001f)
        {
            audioMixer.SetFloat("SFXVolume", -80f);
        }
        music.volume = 1;
        SFX.volume = 1;
        music.loop = true;
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
    }
    
    private void OnDisable()
    {
        GameEvents.OnShoot -= PlayRandomShootSFX;
        GameEvents.OnHit -= PlayRandomHitMarkerSFX;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeftSFX;
        GameEvents.OnEnemyDeath -= PlayEnemyDeathSFX;
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
            SFX.pitch = 1f;
            SFX.PlayOneShot(shootingSFX[Random.Range(0, shootingSFX.Length)]);
            //sfxAudioSource.pitch = 1f;
        }
    }
    
    private void PlayRandomHitMarkerSFX()
    {
        SFX.pitch = 1f;
        SFX.PlayOneShot(hitMarkerSFX[Random.Range(0, hitMarkerSFX.Length)]);
    }

    private void PlayEnemyDeathSFX()
    {
        SFX.pitch = Random.Range(0.9f, 1.3f);
        SFX.PlayOneShot(enemyDeathSFX);
    }
}
