using System.Collections.Generic;
using UnityEngine;
namespace AudioSystem
{
	/// <summary>
	/// Variant of AudioManager that plays music instead.
	/// </summary>
	public class MusicManager : AudioManager
	{
		#region Variables
		public bool loopTracks = false;
		public bool shuffle = true;
		private List<int> usedIndexes;
		#endregion

		#region MonoBehaviour Methods
		void Start()
		{
			audio.bypassReverbZones = true;
			audio.bypassListenerEffects = true;
			audio.bypassEffects = true;
			audio.priority = 255;
			if (shuffle && clips != null && clips.Length > 0)
			{
				audio.Stop();
				index = GetRandomTrack();
				PlayClip();
			}
		}
		#endregion

		#region AudioManager Methods
		public override void PlayClip(AudioClip clip, bool stopCurrent)
		{
			if (stopCurrent)
			{
				audio.Stop();
			}
			volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
			volume *= initialVolume;
			audio.volume = volume * maxVolume * globalVolume;
			audio.PlayOneShot(clip);
			current = clip;
			Console.Log(CurrentTrackInfo(), false);
			if (loopTracks)
			{
				Invoke("NextTrack", clip.length);
			}
		}
		#endregion

		#region MusicManager Methods
		protected void NextTrack()
		{
			if (!loopTracks || clips == null || clips.Length == 0)
			{
				return;
			}
			if (shuffle)
			{
				index = GetRandomTrack();
			}
			else
			{
				if (index < clips.Length - 1)
				{
					index++;
				}
				else
				{
					index = 0;
				}
			}
			PlayClip();
		}

		/// <summary>
		/// Gets a random index for a track.
		/// </summary>
		/// <returns>An int representing a track index, or 0 if shuffle is turned off.</returns>
		protected int GetRandomTrack()
		{
			if (!shuffle || clips == null || clips.Length == 0)
			{
				return 0;
			}
			if (usedIndexes == null)
			{
				usedIndexes = new List<int>();
			}
			// Get random track
			int track = Random.Range(0, clips.Length);
			// Make sure we haven't used it before
			if (usedIndexes.Contains(track))
			{
				if (usedIndexes.Count == clips.Length)
				{
					// We have used it before, but we've used all of them before
					usedIndexes.Clear();
				}
				else
				{
					// Try again
					return GetRandomTrack();
				}
			}
			// Add to the list
			usedIndexes.Add(track);
			return track;
		}

		public LocalizedString CurrentTrackInfo()
		{
			if (current == null)
			{
				return new LocalizedString(GlobalConsts.ID_NOT_PLAYING_TRACK, "Not currently playing a track.");
			}
			return new LocalizedString(GlobalConsts.ID_CURRENTLY_PLAYING_TRACK, "Playing track %s, %d seconds long.", new object[] { current.name, current.length });
		}

		public void RestartTrack()
		{
			Stop();
			PlayClip();
		}

		public void Stop()
		{
			CancelInvoke();
			audio.Stop();
		}
		#endregion
	}
}