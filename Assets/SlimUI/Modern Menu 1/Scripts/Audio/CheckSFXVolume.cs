using UnityEngine;
using UnityEngine.Audio;

namespace SlimUI.ModernMenu{
	public class CheckSFXVolume : MonoBehaviour
	{
		public AudioMixer audioMixer;
		public void  Start (){
			// remember volume level from last time
			UpdateVolume();
		}

		public void UpdateVolume (){
			if (PlayerPrefs.GetFloat("GeneralSFXVolume") < 0.001f)
			{
				audioMixer.SetFloat("GeneralSFXVolume", -80f);
			}
			else
			{
				audioMixer.SetFloat("GeneralSFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("GeneralSFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);
			}
			//Debug.Log("Updated SFX Volume: " + PlayerPrefs.GetFloat("GeneralSFXVolume"));
		}
	}
}