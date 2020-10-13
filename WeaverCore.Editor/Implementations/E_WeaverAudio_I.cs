﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using WeaverCore.Enums;
using WeaverCore.Implementations;

namespace WeaverCore.Editor.Implementations
{
	public class E_WeaverAudio_I : WeaverAudio_I
	{
		public override AudioMixer MainMixer
		{
			get
			{
				return null;
			}
		}

		public override AudioMixerGroup MainMusic
		{
			get
			{
				return null;
			}
		}

		public override AudioMixerGroup Master
		{
			get
			{
				return null;
			}
		}

		public override AudioMixerGroup Sounds
		{
			get
			{
				return null;
			}
		}

		/*public override AudioChannel GetChannel(WeaverAudioPlayer audioObject)
		{
			return AudioChannel.None;
		}*/

		public override AudioMixerGroup GetMixerForChannel(AudioChannel channel)
		{
			return null;
		}

		/*public WeaverAudioPlayer Play(AudioClip clip, Vector3 position, float volume, AudioChannel channel, bool autoPlay, bool deleteWhenDone)
		{
			//GameObject audioObject = new GameObject("__AUDIO_OBJECT__", typeof(AudioSource), typeof(WeaverAudioPlayer));
			var audioObject = WeaverAudioPlayer.Create(position);

			return PlayReuse(audioObject, clip, position, volume, channel, autoPlay, deleteWhenDone);
		}

		public WeaverAudioPlayer PlayReuse(WeaverAudioPlayer audioObject, AudioClip clip, Vector3 position, float volume, AudioChannel channel, bool autoPlay, bool deleteWhenDone)
		{
			var audioSource = audioObject.AudioSource;//audioObject.GetComponent<AudioSource>();

			audioSource.Stop();
			audioSource.clip = clip;
			audioObject.gameObject.name = "(Sound) " + clip.name;
			audioObject.transform.position = position;
			audioSource.volume = volume;
			if (autoPlay)
			{
				audioSource.Play();
			}

			//var hollowAudio = audioObject.GetComponent<WeaverAudioPlayer>();

			if (deleteWhenDone)
			{
				audioObject.Delete(clip.length);
			}

			return audioObject;
		}*/

		/*public override void SetChannel(WeaverAudioPlayer audioObject, AudioChannel channel)
		{
			audioObject.AudioSource.outputAudioMixerGroup = null;
		}*/
	}
}
