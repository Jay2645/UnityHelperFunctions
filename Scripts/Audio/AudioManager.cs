using System.Collections.Generic;
using UnityEngine;
namespace HelperFunctions
{
namespace AudioSystem
{
	/// <summary>
	/// Plays custom audio clips. Replacement for the AudioSource class.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : MonoBehaviour
	{
		#region Variables
		public AudioClip[] clips;
		public int index = 0;
		public bool playOnAwake = false;
		public static float globalVolume = 1.0f;
		public float maxVolume = 1.0f;
		protected float volume = 1.0f;
		protected float initialVolume = 1.0f;
		protected static List<AudioManager> allManagers = new List<AudioManager>();
		protected static Dictionary<string, AudioClip> clipsByName = new Dictionary<string, AudioClip>();
		public bool fadeOut = false;
		protected AudioClip current = null;
		#endregion

		#region MonoBehaviour Methods
		void Awake()
		{
			if (clips != null)
			{
				foreach (AudioClip clip in clips)
				{
					RegisterClip(clip);
				}
			}
			audio.Stop();
			initialVolume = audio.volume;
			audio.playOnAwake = false;
			if (playOnAwake)
			{
				PlayClip();
			}
			allManagers.Add(this);
		}

		void LateUpdate()
		{
			if (fadeOut)
			{
				volume = Mathf.Lerp(volume, 0.0f, Time.deltaTime * GlobalConsts.SCENE_TRANSITION_TIME);
				audio.volume = volume * maxVolume * globalVolume;
			}
		}
		#endregion

		#region AudioManager Methods
		public void PlayClip(int clipIndex)
		{
			index = clipIndex;
			PlayClip();
		}

		public void PlayClip()
		{
			if (clips == null || index >= clips.Length)
			{
				return;
			}
			PlayClip(clips[index]);
		}

		public void PlayClip(AudioClip clip)
		{
			if (current != null && audio.isPlaying && current.name == clip.name)
			{
				return;
			}
			RegisterClip(clip);
			PlayClip(clip, true);
		}

		public virtual void PlayClip(AudioClip clip, bool stopCurrent)
		{
			if (stopCurrent)
			{
				audio.Stop();
			}
			volume = PlayerPrefs.GetFloat("AudioVolume", 1.0f);
			volume *= initialVolume;
			audio.volume = volume * maxVolume * globalVolume;
			audio.PlayOneShot(clip);
			current = clip;
		}

		public static void FadeOut()
		{
			AudioManager[] managers = allManagers.ToArray();
			foreach (AudioManager manager in managers)
			{
				manager.fadeOut = true;
			}
		}

		private static void RegisterClip(AudioClip clip)
		{
			string clipName = clip.name;
			if (!clipsByName.ContainsKey(clipName))
			{
				clipsByName.Add(clipName, clip);
			}
		}

		public static AudioClip GetClip(string name)
		{
			if (clipsByName.ContainsKey(name))
			{
				return clipsByName[name];
			}
			else
			{
				return null;
			}
		}

		public void SetPitch(float pitch)
		{
			audio.pitch = pitch;
		}
		#endregion
	}
}
}