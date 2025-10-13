using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace SlimUI.ModernMenu{
	public class CheckMusicVolume : MonoBehaviour
	{

		public AudioMixer audioMixer;
		public void  Start (){
			// remember volume level from last time
			UpdateVolume();
		}

		public void UpdateVolume (){
			if (PlayerPrefs.GetFloat("MusicVolume") < 0.001f)
			{
				audioMixer.SetFloat("MusicVolume", -80f);
			}
			else
			{
				audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MusicVolume", 0.05f), 0.0001f, 1f)) * 20 - 10); // -10 so music can never reach -0dB since it would be too loud
			}
			//Debug.Log("Updated MusicVolume: " + PlayerPrefs.GetFloat("MusicVolume"));
		}
	}
}