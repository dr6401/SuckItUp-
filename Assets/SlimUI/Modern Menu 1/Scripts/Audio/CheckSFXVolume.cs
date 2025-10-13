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
			if (PlayerPrefs.GetFloat("SFXVolume") < 0.001f)
			{
				audioMixer.SetFloat("SFXVolume", -80f);
			}
			else
			{
				audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("SFXVolume", 0.75f), 0.0001f, 1f)) * 20);
			}
			//Debug.Log("Updated SFX Volume: " + PlayerPrefs.GetFloat("SFXVolume"));
		}
	}
}