﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace WeaverCore.Utilities
{
    public class ResourceMetaData
    {
        public bool compressed;
        public string hash;

        public ResourceMetaData(bool Compressed, string Hash)
        {
            compressed = Compressed;
            hash = Hash;
        }

        public MemoryStream ToStream()
        {
            var stream = new MemoryStream();
            if (compressed)
            {
                stream.WriteByte(1);
            }
            else
            {
                stream.WriteByte(0);
            }
            foreach (var character in hash)
            {
                stream.WriteByte((byte)character);
            }
            stream.Position = 0;
            return stream;
        }

        public static ResourceMetaData FromStream(Stream stream)
        {
            long oldPosition = stream.Position;
            stream.Position = 0;
            int compressed = stream.ReadByte();

            ResourceMetaData meta = new ResourceMetaData(compressed == 1, "");

            for (int i = 0; i < stream.Length - 1; i++)
            {
                meta.hash += (char)stream.ReadByte();
            }
            stream.Position = oldPosition;
            return meta;
        }
    }

    /// <summary>
    /// Used for loading resources from an assembly, and other related actions
    /// </summary>
    public static class ResourceUtilities
    {
        /// <summary>
        /// Loads an assembly from a resource path in an existing assembly
        /// </summary>
        /// <param name="resourcePath">The path of the assembly resource</param>
        /// <param name="assembly">The assembly to load the resource from</param>
        /// <returns>Returns the loaded assembly</returns>
        public static Assembly LoadAssembly(string resourcePath,Assembly assembly = null)
        {
            if (!HasResource(resourcePath,assembly))
            {
                return null;
            }
            using (Stream stream = Retrieve(resourcePath, assembly))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return Assembly.Load(reader.ReadBytes((int)stream.Length));
                }
            }
        }

        /// <summary>
        /// Checks if the assembly has the specified resource path
        /// </summary>
        /// <param name="resourcePath">The path to check if it exists</param>
        /// <param name="assembly">The assembly to check in</param>
        /// <returns>Returns whether the specified resource path exists in the assembly</returns>
        public static bool HasResource(string resourcePath,Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = typeof(ResourceUtilities).Assembly;
            }
            foreach (var path in assembly.GetManifestResourceNames())
            {
                if (path == resourcePath)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves a stream of data from the resource path in the assembly
        /// </summary>
        /// <param name="resourcePath">The path of the resource to load</param>
        /// <param name="assembly">The assembly to load from</param>
        /// <returns>Returns a stream containing the data of the resource</returns>
        public static Stream Retrieve(string resourcePath,Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = typeof(ResourceUtilities).Assembly;
            }

            if (!HasResource(resourcePath, assembly))
            {
                return null;
            }

            bool compressed = false;
            if (HasResource(resourcePath + "_meta", assembly))
            {
                using (var metaStream = assembly.GetManifestResourceStream(resourcePath + "_meta"))
                {
                    int compressedByte = metaStream.ReadByte();
                    compressed = compressedByte == 1;
                }
            }

            if (!compressed)
            {
                return assembly.GetManifestResourceStream(resourcePath);
            }
            else
            {
                MemoryStream finalStream = new MemoryStream();
                using (Stream compressedStream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (var decompressionStream = new GZipStream(compressedStream,CompressionMode.Decompress))
                    {
                        StreamUtilities.CopyTo(decompressionStream, finalStream);
                    }
                }
                return finalStream;
            }
        }

        /// <summary>
        /// Retrieves a stream of data from the resource path in the assembly
        /// </summary>
        /// <param name="resourcePath">The path of the resource to load</param>
        /// <param name="outputStream">The stream to write the data to</param>
        /// <param name="assembly">The assembly to load from</param>
        /// <returns>Returns a stream containing the data of the resource</returns>
        public static bool Retrieve(string resourcePath, Stream outputStream, Assembly assembly = null)
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (assembly == null)
            {
                assembly = typeof(ResourceUtilities).Assembly;
            }

            if (!HasResource(resourcePath, assembly))
            {
                return false;
            }

            bool compressed = false;
            if (HasResource(resourcePath + "_meta", assembly))
            {
                using (var metaStream = assembly.GetManifestResourceStream(resourcePath + "_meta"))
                {
                    int compressedByte = metaStream.ReadByte();
                    compressed = compressedByte == 1;
                }
            }

            if (!compressed)
            {
                //outputStream = assembly.GetManifestResourceStream(resourcePath);

                using (var sourceStream = assembly.GetManifestResourceStream(resourcePath))
                {
                    sourceStream.CopyTo(outputStream);
                }
                return true;
            }
            else
            {
                //MemoryStream finalStream = new MemoryStream();
                using (Stream compressedStream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        StreamUtilities.CopyTo(decompressionStream, outputStream);
                    }
                }


                return true;
                //return finalStream;
            }
        }

        public static ResourceMetaData GetMetadataForResource(string resourcePath, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = typeof(ResourceUtilities).Assembly;
            }

            if (!HasResource($"{resourcePath}_meta", assembly))
            {
                return null;
            }

            using (Stream compressedStream = assembly.GetManifestResourceStream(resourcePath))
            {
                return ResourceMetaData.FromStream(compressedStream);
            }
        }
    }
}
