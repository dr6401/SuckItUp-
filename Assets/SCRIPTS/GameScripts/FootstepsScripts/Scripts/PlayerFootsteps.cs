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
        private float slideInterval = 1f;
        private float stepTimer;
        private float slideTimer;

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
                    playWalkSound(surfaceData);
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
                    playRunSound(surfaceData);
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
                    playJumpSound(surfaceData);
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
                    playLandSound(surfaceData);
                    return;
                }
            }
        }



        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        private void playWalkSound(FSR_Data.SurfaceType surfaceType)
        {
            AudioClip[] soundEffects= surfaceType.walkSoundEffects;

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
        }
        private void playRunSound(FSR_Data.SurfaceType surfaceType)
        {
            AudioClip[] soundEffects= surfaceType.runSoundEffects;

            int n = Random.Range(0, soundEffects.Length);
            m_AudioSource.clip = soundEffects[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            soundEffects[n] = soundEffects[0];
            soundEffects[0] = m_AudioSource.clip;
        }
        private void playJumpSound(FSR_Data.SurfaceType surfaceType)
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
        private void playLandSound(FSR_Data.SurfaceType surfaceType)
        {
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
