﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using WeaverCore.Attributes;
using WeaverCore.Implementations;
using WeaverCore.Interfaces;

namespace WeaverCore.Editor.Implementations
{
    public class E_AudioMixer_I : AudioMixer_I
	{
		static bool assetsLoaded = false;

		static System.Collections.Generic.List<AudioMixer> Mixers;
		static System.Collections.Generic.List<AudioMixerSnapshot> Snapshots;
		static System.Collections.Generic.List<AudioMixerGroup> Groups;

		public override MusicCue ActiveMusicCue
		{
			get
			{
				if (EditorMusic.Instance != null)
				{
					return EditorMusic.Instance.ActiveMusicCue;
                }
				else
				{
					return null;
				}
			}
		}

        public override AudioMixerGroup GetGroupForMixer(AudioMixer mixer, string groupName)
		{
			LoadAssets();
			for (int i = 0; i < Groups.Count; i++)
			{
				if (Groups[i].audioMixer == mixer && Groups[i].name == groupName)
				{
					return Groups[i];
				}
			}
			return null;
		}

		public override AudioMixer GetMixer(string mixerName)
		{
			LoadAssets();
			for (int i = 0; i < Mixers.Count; i++)
			{
				if (Mixers[i].name == mixerName)
				{
					return Mixers[i];
				}
			}
			return null;
		}

		public override AudioMixerSnapshot GetSnapshotForMixer(AudioMixer mixer, string snapshotName)
		{
			LoadAssets();
			for (int i = 0; i < Snapshots.Count; i++)
			{
				if (Snapshots[i].audioMixer == mixer && Snapshots[i].name == snapshotName)
				{
					return Snapshots[i];
				}
			}
			return null;
		}

		[OnRuntimeInit]
		static void LoadAssets()
		{
			if (assetsLoaded)
			{
				return;
			}

			assetsLoaded = true;

			var mixers = AssetDatabase.FindAssets("t:AudioMixer");

            Mixers = new System.Collections.Generic.List<AudioMixer>();
            Snapshots = new System.Collections.Generic.List<AudioMixerSnapshot>();
            Groups = new System.Collections.Generic.List<AudioMixerGroup>();

			foreach (var guid in mixers)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (!path.Contains("WeaverCore.Editor"))
				{
					continue;
				}
				var assets = AssetDatabase.LoadAllAssetsAtPath(path);

				foreach (var asset in assets)
				{
					if (asset is AudioMixer)
					{
						Mixers.Add(asset as AudioMixer);
					}
					else if (asset is AudioMixerGroup)
					{
						Groups.Add(asset as AudioMixerGroup);
					}
					else if (asset is AudioMixerSnapshot)
					{
						Snapshots.Add(asset as AudioMixerSnapshot);
					}
				}
			}
		}

		public override void PlayMusicPack(MusicPack pack, float delayTime, float snapshotTransitionTime, bool applySnapshot = true)
		{
			EditorMusic.Instance.PlayMusicPack(pack, delayTime, snapshotTransitionTime, applySnapshot);
		}

		public override void ApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime)
		{
			EditorMusic.Instance.ApplyMusicSnapshot(snapshot, delayTime, transitionTime);
		}

		public override void ApplyAtmosSnapshot(Atmos.SnapshotType snapshot, float transitionTime, Atmos.AtmosSources enabledSources)
		{
			EditorMusic.Instance.ApplyAtmosPack(snapshot, transitionTime, enabledSources);
		}

        public override void PlayMusicCue(MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
        {
			EditorMusic.Instance.PlayMusicCue(musicCue, delayTime, transitionTime, applySnapshot);
        }
    }
}
