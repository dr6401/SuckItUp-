using System;
using FSR;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootsteps : MonoBehaviour
    {
        private AudioSource m_AudioSource;
        public Transform foot;
        public float raycastSize = 10;
        [SerializeField] private FSR_Data data;

        private float walkStepInterval = 0.5f;
        private float runStepInterval = 0.3f;
        private float landInterval = GameConstants.playerLandedInterval;
        private float stepTimer;
        private float landTimer;

        public void Start()
        {
            if (foot == null)
            {
                foot = GameObject.FindWithTag("PlayerHitBox").transform;
            }
            m_AudioSource = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().footstepsSFXSource;
            if (m_AudioSource == null) Debug.Log("PlayerFootsteps didn't find an audio source");
        }

        private void Update()
        {
            stepTimer += Time.deltaTime;
            landTimer += Time.deltaTime;
        }


        public void Walk()
        {
            if (stepTimer < walkStepInterval) return;
            stepTimer = 0;
            if (foot == null || data == null || data.surfaces == null) return;
            
            if (!Physics.Raycast(foot.position, Vector3.down, out RaycastHit hit, raycastSize))
                return;
            
            string surfaceName = "GENERIC";

            if (hit.transform.TryGetComponent(out FSR_SimpleSurface simple))
            {
                surfaceName = simple.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TagedSurface tagged))
            {
                surfaceName = tagged.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TerrainSurface terrain))
            {
                surfaceName = terrain.GetSurface(transform.position);
            }

            foreach (var surfaceData in data.surfaces)
            {
                if (surfaceData != null && surfaceData.name == surfaceName)
                {
                    PlayWalkSound(surfaceData);
                    return;
                }
            }
        }
        
        public void Run()
        {
            if (stepTimer < runStepInterval) return;
            stepTimer = 0;
            if (foot == null || data == null || data.surfaces == null) return;
            
            if (!Physics.Raycast(foot.position, Vector3.down, out RaycastHit hit, raycastSize))
                return;
            
            string surfaceName = "GENERIC";

            if (hit.transform.TryGetComponent(out FSR_SimpleSurface simple))
            {
                surfaceName = simple.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TagedSurface tagged))
            {
                surfaceName = tagged.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TerrainSurface terrain))
            {
                surfaceName = terrain.GetSurface(transform.position);
            }

            foreach (var surfaceData in data.surfaces)
            {
                if (surfaceData != null && surfaceData.name == surfaceName)
                {
                    PlayRunSound(surfaceData);
                    return;
                }
            }
        }
        
        public void Jump()
        {
            if (foot == null || data == null || data.surfaces == null) return;
            
            if (!Physics.Raycast(foot.position, Vector3.down, out RaycastHit hit, raycastSize))
                return;
            
            string surfaceName = "GENERIC";

            if (hit.transform.TryGetComponent(out FSR_SimpleSurface simple))
            {
                surfaceName = simple.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TagedSurface tagged))
            {
                surfaceName = tagged.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TerrainSurface terrain))
            {
                surfaceName = terrain.GetSurface(transform.position);
            }

            foreach (var surfaceData in data.surfaces)
            {
                if (surfaceData != null && surfaceData.name == surfaceName)
                {
                    PlayJumpSound(surfaceData);
                    return;
                }
            }
        }
        
        public void Land()
        {
            if (foot == null || data == null || data.surfaces == null) return;
            
            if (!Physics.Raycast(foot.position, Vector3.down, out RaycastHit hit, raycastSize))
                return;
            
            string surfaceName = "GENERIC";

            if (hit.transform.TryGetComponent(out FSR_SimpleSurface simple))
            {
                surfaceName = simple.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TagedSurface tagged))
            {
                surfaceName = tagged.GetSurface();
            }
            else if (hit.transform.TryGetComponent(out FSR_TerrainSurface terrain))
            {
                surfaceName = terrain.GetSurface(transform.position);
            }

            foreach (var surfaceData in data.surfaces)
            {
                if (surfaceData != null && surfaceData.name == surfaceName)
                {
                    PlayLandSound(surfaceData);
                    return;
                }
            }
        }



        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        private void PlayWalkSound(FSR_Data.SurfaceType surfaceType)
        {
            AudioClip[] soundEffects= surfaceType.walkSoundEffects;

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
        }
        private void PlayRunSound(FSR_Data.SurfaceType surfaceType)
        {
            AudioClip[] soundEffects= surfaceType.runSoundEffects;

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
        }
        private void PlayJumpSound(FSR_Data.SurfaceType surfaceType)
        {
            AudioClip[] soundEffects= surfaceType.jumpSoundEffects;

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
            Debug.Log("Played Jump sfx");
        }
        private void PlayLandSound(FSR_Data.SurfaceType surfaceType)
        {
            if (landTimer < landInterval) return;
            landTimer = 0;
            AudioClip[] soundEffects= surfaceType.landSoundEffects;

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
        }

        private void PlaySlideSound() // Same slide sfx no matter what surface we are on
        {
            if (data == null) return;
            AudioClip[] soundEffects= data.slideSoundEffects;
            stepTimer = 0; // Disable walking/running sfx for a bit after sliding

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
        }
        

        private void OnEnable()
        {
            GameEvents.OnPlayerWalking += Walk;
            GameEvents.OnPlayerRunning += Run;
            GameEvents.OnPlayerJumped += Jump;
            GameEvents.OnPlayerLanded += Land;
            GameEvents.OnPlayerSlided += PlaySlideSound;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWalking -= Walk;
            GameEvents.OnPlayerRunning -= Run;
            GameEvents.OnPlayerJumped -= Jump;
            GameEvents.OnPlayerLanded -= Land;
            GameEvents.OnPlayerSlided -= PlaySlideSound;
        }
    }
