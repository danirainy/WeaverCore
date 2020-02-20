﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using WeaverCore.Internal;

namespace WeaverCore.Helpers
{
    public static class ImplementationFinder
    {
        public static RunningState State { get; private set; }


        static Dictionary<Type, Type> Cache = new Dictionary<Type, Type>();
        static List<Type> FoundImplementations;
        static Assembly ImplAssembly;

        static Assembly UnityEditorAssembly;

        static Func<string, bool> DeleteAsset;
        static Action<string> ImportAsset;

        static RunningState GetState()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "UnityEditor")
                {
                    if (UnityEditorAssembly == null)
                    {
                        UnityEditorAssembly = assembly;
                        LoadAssetFunctions();
                    }
                    return RunningState.Editor;
                }
            }
            return RunningState.Game;
        }

        static void LoadAssetFunctions()
        {
            var adb = UnityEditorAssembly.GetType("UnityEditor.AssetDatabase");
            DeleteAsset = Methods.GetFunction<Func<string, bool>>(adb.GetMethod("DeleteAsset"));
            ImportAsset = Methods.GetFunction<Action<string>>(adb.GetMethod("ImportAsset", new Type[] { typeof(string) }));
        }

        static void WriteAssembly(string directory, string filePath, string fileName, Stream data)
        {
            string fullPath = directory + "/" + filePath;
            if (File.Exists(fullPath))
            {
                bool deleted = DeleteAsset(filePath);
            }

            var tempPath = Path.GetTempPath();
            if (File.Exists(tempPath + fileName))
            {
                File.Delete(tempPath + fileName);
            }
            using (var file = File.Create(tempPath + fileName))
            {
                using (var reader = new BinaryReader(data))
                {
                    using (var writer = new BinaryWriter(file))
                    {
                        byte[] buffer = new byte[1024];
                        int amount = 0;
                        do
                        {
                            amount = reader.Read(buffer, 0, buffer.Length);
                            if (amount > 0)
                            {
                                writer.Write(buffer, 0, amount);
                            }

                        } while (amount != 0);
                    }
                }
            }
            File.Move(tempPath + fileName, fullPath);
            ImportAsset(filePath);
        }

        static string GetHash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var oldPosition = stream.Position;
                var result = BitConverter.ToString(md5.ComputeHash(stream));
                stream.Position = oldPosition;
                return result;
            }
        }

        static string GetHash(string filePath)
        {
            using (var openStream = File.OpenRead(filePath))
            {
                return GetHash(openStream);
            }
        }

        static void LoadEditorAssembly()
        {
            ImplAssembly = ResourceLoader.LoadAssembly($"{nameof(WeaverCore)}.Editor");
            Stream resourceStream = ResourceLoader.Retrieve($"{nameof(WeaverCore)}.Editor.Visual"); //Gets disposed in the Initializer below
            var resourceHash = GetHash(resourceStream);
            var directory = Directory.CreateDirectory($"Assets/{nameof(WeaverCore)}/Editor");
            string filePath = directory.FullName + $"/{nameof(WeaverCore)}.Editor.Visual.dll";

            if (!File.Exists(filePath) || resourceHash != GetHash(filePath))
            {
                EditorInitializer.AddInitializer(() =>
                {
                    WriteAssembly(new DirectoryInfo("Assets").Parent.FullName, $"Assets/{nameof(WeaverCore)}/Editor/{nameof(WeaverCore)}.Editor.Visual.dll", $"{nameof(WeaverCore)}.Editor.Visual.dll", resourceStream);
                    resourceStream.Dispose();
                    using (var internalStream = ResourceLoader.Retrieve($"{nameof(WeaverCore)}.Resources.InternalClasses.txt"))
                    {
                        using (var output = File.Create($"Assets/{nameof(WeaverCore)}/Internal.cs"))
                        {
                            using (var writer = new StreamWriter(output))
                            {
                                using (var reader = new StreamReader(internalStream))
                                {
                                    writer.Write(reader.ReadToEnd());
                                }
                            }
                        }
                    }
                    ImportAsset($"Assets/{nameof(WeaverCore)}/Internal.cs");
                });
            }
        }

        static void LoadImplementations()
        {
            if (FoundImplementations == null)
            {
                FoundImplementations = new List<Type>();
                State = GetState();
                if (State == RunningState.Editor)
                {
                    LoadEditorAssembly();
                }
                else
                {
                    ImplAssembly = ResourceLoader.LoadAssembly($"{nameof(WeaverCore)}.Game");
                }
                foreach (var type in ImplAssembly.GetTypes())
                {
                    if (typeof(IImplementation).IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                    {
                        FoundImplementations.Add(type);
                    }
                }
            }
        }

        public static Type GetImplementationType<T>() where T : IImplementation
        {
            var type = typeof(T);
            Type implType = null;
            LoadImplementations();
            if (Cache.ContainsKey(type))
            {
                implType = Cache[type];
            }
            else
            {
                foreach (var foundType in FoundImplementations)
                {
                    if (typeof(T).IsAssignableFrom(foundType))
                    {
                        implType = foundType;
                        Cache.Add(type, implType);
                        break;
                    }
                }
            }
            if (implType == null)
            {
                throw new Exception($"Implementation for {typeof(T).FullName} does not exist in {ImplAssembly.FullName}");
            }
            else
            {
                return implType;
            }
        }

        public static T GetImplementation<T>() where T : IImplementation
        {
            return (T)Activator.CreateInstance(GetImplementationType<T>());
        }
    }
}
