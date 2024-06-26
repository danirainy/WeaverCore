﻿using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using WeaverCore.Attributes;
using WeaverCore.Internal;
using WeaverCore.Utilities;

namespace WeaverCore.Editor
{
	/// <summary>
	/// Contains misc utilties regarding registries
	/// </summary>
	public static class RegistryTools
	{
		/// <summary>
		/// Gets all registries in the project
		/// </summary>
		public static System.Collections.Generic.List<Registry> GetAllRegistries()
		{
            System.Collections.Generic.List<Registry> registries = new System.Collections.Generic.List<Registry>();
			var guids = AssetDatabase.FindAssets($"t:{nameof(Registry)}");
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				registries.Add(AssetDatabase.LoadAssetAtPath<Registry>(path));
			}
			return registries;
		}

		static System.Collections.Generic.List<Type> modsCached;

        /// <summary>
        /// Gets all mod types in the project
        /// </summary>
        public static System.Collections.Generic.List<Type> GetAllMods()
        {
			//modsCached = null;
            if (modsCached == null)
            {
                //WeaverLog.Log("BEGINNING SEARCH FOR MODS");
                modsCached = new System.Collections.Generic.List<Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        //if (typeof(IMod).IsAssignableFrom(type))
                        //{
                            //WeaverLog.LogWarning("Maybe found mod = " + type);

                            if (typeof(IMod).IsAssignableFrom(type) && !type.IsGenericType && !type.IsAbstract)
                            {
                                //WeaverLog.Log("Added mod = " + type);
                                modsCached.Add(type);
                            }
                            //else
                            //{
                            //    WeaverLog.LogError("Didn't add following mod = " + type);
                            //}
                        //}
                    }
                }
                //WeaverLog.Log("ENDING SEARCH FOR MODS");
            }
            return modsCached;
        }

        static string[] modNamesCached;

		/// <summary>
		/// Gets the names of all mods in the project
		/// </summary>
		public static string[] GetAllModNames()
		{
			if (modNamesCached == null)
			{
				var mods = GetAllMods();
				modNamesCached = new string[mods.Count];
				for (int i = 0; i < mods.Count; i++)
				{
                    if (mods[i] == typeof(WeaverCore_ModClass))
                    {
						modNamesCached[i] = "WeaverCore";
					}
					else
                    {
						modNamesCached[i] = StringUtilities.Prettify(mods[i].Name);
					}
				}
			}
			
			return modNamesCached;
		}

		static System.Collections.Generic.List<Type> featuresCached;

		/// <summary>
		/// Gets all feature types that can be added to a registry
		/// </summary>
		public static System.Collections.Generic.List<Type> GetAllFeatures()
		{
			if (featuresCached == null)
			{
                featuresCached = new System.Collections.Generic.List<Type>();
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (var type in assembly.GetTypes())
					{
						if (type.IsDefined(typeof(ShowFeatureAttribute), false))
						{
							featuresCached.Add(type);
						}
					}
				}
			}
			
			return featuresCached;
		}

		static string[] featureNamesCached;

		/// <summary>
		/// Gets the names of all feature types that can be added to a registry
		/// </summary>
		public static string[] GetAllFeatureNames()
		{
			if (featureNamesCached == null)
			{
				var features = GetAllFeatures();
				featureNamesCached = new string[features.Count];
				for (int i = 0; i < features.Count; i++)
				{
					featureNamesCached[i] = StringUtilities.Prettify(features[i].Name);
				}
			}
			return featureNamesCached;
		}
	}
}
