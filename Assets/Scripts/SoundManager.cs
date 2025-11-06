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
    [SerializeField] private AudioClip noAmmoLeftSFX;
    private AudioClip[][] SFXSounds;
    private static SoundManager instance;
    private readonly float fadeDuration = 0.1f;
    private float currentMusicVolume;
    private float currentSFXVolume;
    private float currentSuctionSFXTimer = 0f;
    private float maxSuctionSFXTimer = 0.5f;

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
        SFX.PlayOneShot(SFXSounds[clipNum][Random.Range(0, SFXSounds[clipNum].Length - 1)]);
    }
    
    public void PlayStartVacuuming()
    {
        SFX.PlayOneShot(vacuumingSounds[0]);
        SFX.clip = vacuumingSounds[1];
        SFX.PlayDelayed(vacuumingSounds[0].length - 0.25f);
        //SFX.PlayOneShot(vacuumingSounds[1]);
    }
    public void PlayVacuuming()
    {
        SFX.PlayOneShot(vacuumingSounds[1]);
    }
    
    public void PlayEndVacuuming()
    {
        SFX.PlayOneShot(vacuumingSounds[2]);
        StartCoroutine(StopPlayingAfterDelay(0.03f));
    }

    public IEnumerator PlaySFXAfterDelay(AudioClip audioClip, float time)
    {
        yield return new WaitForSeconds(time);
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetupAudio();
    }

    private void SetupAudio()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MusicVolume", GameConstants.defaultMusicVolume), 0.0001f, 1f)) * 20 - 10);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("SFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);
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
    }
    
    private void OnDisable()
    {
        GameEvents.OnShoot -= PlayRandomShootSFX;
        GameEvents.OnHit -= PlayRandomHitMarkerSFX;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeftSFX;
    }

    private void PlayNoAmmoLeftSFX()
    {
        if (noAmmoLeftSFX != null)
        {
            //SFX.PlayOneShot(noAmmoLeftSFX);
        }
    }

    private void PlayRandomShootSFX()
    {
        SFX.PlayOneShot(shootingSFX[Random.Range(0, shootingSFX.Length)]);
    }
    
    private void PlayRandomHitMarkerSFX()
    {
        SFX.PlayOneShot(hitMarkerSFX[Random.Range(0, hitMarkerSFX.Length)]);
    }
}
