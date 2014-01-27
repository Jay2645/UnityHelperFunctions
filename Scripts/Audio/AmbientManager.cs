using UnityEngine;
namespace AudioSystem
{
	/// <summary>
	/// Manages ambient sounds.
	/// </summary>
	public class AmbientManager : AudioManager
	{
		#region AudioManager Methods
		public override void PlayClip(AudioClip clip, bool stopCurrent)
		{
			if (clips == null || index >= clips.Length)
			{
				return;
			}
			if (stopCurrent)
			{
				audio.Stop();
			}
			volume = initialVolume * PlayerPrefs.GetFloat("AmbientVolume", 1.0f);
			audio.volume = volume * maxVolume * globalVolume;
			audio.PlayOneShot(clip);
			current = clip;
		}
		#endregion
	}
}