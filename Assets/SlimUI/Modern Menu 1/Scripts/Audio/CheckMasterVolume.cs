using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace SlimUI.ModernMenu{
    public class CheckMasterVolume : MonoBehaviour
    {

        public AudioMixer audioMixer;
        public void  Start (){
            // remember volume level from last time
            UpdateVolume();
        }

        public void UpdateVolume (){
            if (PlayerPrefs.GetFloat("MasterVolume") < 0.001f)
            {
                audioMixer.SetFloat("MasterVolume", -80f);
            }
            else
            {
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MasterVolume", GameConstants.defaultMasterVolume), 0.0001f, 1f)) * 20);
            }
            //Debug.Log("Updated MusicVolume: " + PlayerPrefs.GetFloat("MusicVolume"));
        }
    }
}