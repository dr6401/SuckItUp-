using UnityEngine;
using UnityEngine.Serialization;

namespace FSR
{
    [CreateAssetMenu]
    public class FSR_Data : ScriptableObject
    {


        public SurfaceType[] surfaces;
         

        [System.Serializable]
        public class SurfaceType
        {
            public string name;
            public AudioClip[] walkSoundEffects;
            public AudioClip[] runSoundEffects;
            public AudioClip[] jumpSoundEffects;
            public AudioClip[] landSoundEffects;
        }
    }
}
