using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace FSR
{
    [RequireComponent(typeof(AudioSource))]
    public class FSR_Player : MonoBehaviour
    {
        private AudioSource m_AudioSource;
        public Transform foot;
        public float raycastSize = 10;
        [SerializeField] private FSR_Data data;


        public void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            if (foot == null)
            {
                Debug.Log("unassigned foot ");
            }
            foot = GameObject.FindWithTag("PlayerHitBox").transform;
        }


        public void Walk()
        {
            if (foot == null) return;
            RaycastHit hit;
            if (Physics.Raycast(foot.position, -foot.up, out hit, raycastSize))
            {
                try {

                   FSR_SimpleSurface surface =  hit.transform.GetComponent<FSR_SimpleSurface>();
                    foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                    {
                        if (surfaceData.name.Equals(surface.GetSurface()))
                        {
                            playWalkSound(surfaceData);
                        }
                    }
                }
                catch
                {
                    try
                    {
                        FSR_TagedSurface surface = hit.transform.GetComponent<FSR_TagedSurface>();
                        foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                        {
                            if (surfaceData.name.Equals(surface.GetSurface()))
                            {
                                playWalkSound(surfaceData);
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            FSR_TerrainSurface surface = hit.transform.GetComponent<FSR_TerrainSurface>();
                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals(surface.GetSurface(transform.position)))
                                {
                                    playWalkSound(surfaceData);
                                }
                            }

                        }
                        catch {

                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals("GENERIC"))
                                {
                                    playWalkSound(surfaceData);
                                }
                            }

                        }
                    }


                }

            }
        }
        
        public void Run()
        {
            if (foot == null) return;
            RaycastHit hit;
            if (Physics.Raycast(foot.position, -foot.up, out hit, raycastSize))
            {
                try {

                   FSR_SimpleSurface surface =  hit.transform.GetComponent<FSR_SimpleSurface>();
                    foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                    {
                        if (surfaceData.name.Equals(surface.GetSurface()))
                        {
                            playRunSound(surfaceData);
                        }
                    }
                }
                catch
                {
                    try
                    {
                        FSR_TagedSurface surface = hit.transform.GetComponent<FSR_TagedSurface>();
                        foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                        {
                            if (surfaceData.name.Equals(surface.GetSurface()))
                            {
                                playRunSound(surfaceData);
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            FSR_TerrainSurface surface = hit.transform.GetComponent<FSR_TerrainSurface>();
                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals(surface.GetSurface(transform.position)))
                                {
                                    playRunSound(surfaceData);
                                }
                            }

                        }
                        catch {

                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals("GENERIC"))
                                {
                                    playRunSound(surfaceData);
                                }
                            }

                        }
                    }


                }

            }
        }
        
        public void Jump()
        {
            if (foot == null) return;
            RaycastHit hit;
            if (Physics.Raycast(foot.position, -foot.up, out hit, raycastSize))
            {
                try {

                   FSR_SimpleSurface surface =  hit.transform.GetComponent<FSR_SimpleSurface>();
                    foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                    {
                        if (surfaceData.name.Equals(surface.GetSurface()))
                        {
                            playJumpSound(surfaceData);
                        }
                    }
                }
                catch
                {
                    try
                    {
                        FSR_TagedSurface surface = hit.transform.GetComponent<FSR_TagedSurface>();
                        foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                        {
                            if (surfaceData.name.Equals(surface.GetSurface()))
                            {
                                playJumpSound(surfaceData);
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            FSR_TerrainSurface surface = hit.transform.GetComponent<FSR_TerrainSurface>();
                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals(surface.GetSurface(transform.position)))
                                {
                                    playJumpSound(surfaceData);
                                }
                            }

                        }
                        catch {

                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals("GENERIC"))
                                {
                                    playJumpSound(surfaceData);
                                }
                            }

                        }
                    }


                }

            }
        }
        
        public void Land()
        {
            if (foot == null) return;
            RaycastHit hit;
            if (Physics.Raycast(foot.position, -foot.up, out hit, raycastSize))
            {
                try {

                   FSR_SimpleSurface surface =  hit.transform.GetComponent<FSR_SimpleSurface>();
                    foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                    {
                        if (surfaceData.name.Equals(surface.GetSurface()))
                        {
                            playLandSound(surfaceData);
                        }
                    }
                }
                catch
                {
                    try
                    {
                        FSR_TagedSurface surface = hit.transform.GetComponent<FSR_TagedSurface>();
                        foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                        {
                            if (surfaceData.name.Equals(surface.GetSurface()))
                            {
                                playLandSound(surfaceData);
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            FSR_TerrainSurface surface = hit.transform.GetComponent<FSR_TerrainSurface>();
                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals(surface.GetSurface(transform.position)))
                                {
                                    playLandSound(surfaceData);
                                }
                            }

                        }
                        catch {

                            foreach (FSR_Data.SurfaceType surfaceData in data.surfaces)
                            {
                                if (surfaceData.name.Equals("GENERIC"))
                                {
                                    playLandSound(surfaceData);
                                }
                            }

                        }
                    }


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
        
        

        private void OnEnable()
        {
            GameEvents.OnPlayerWalking += Walk;
            GameEvents.OnPlayerRunning += Run;
            GameEvents.OnPlayerJumped += Jump;
            GameEvents.OnPlayerLanded += Land;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWalking -= Walk;
            GameEvents.OnPlayerRunning -= Run;
            GameEvents.OnPlayerJumped -= Jump;
            GameEvents.OnPlayerLanded -= Land;
        }
    }
}
