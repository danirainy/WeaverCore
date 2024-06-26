﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeaverCore.DataTypes;
using WeaverCore.Utilities;

namespace WeaverCore
{
    /// <summary>
    /// When attached to an object, this object can now be used in a <see cref="ObjectPool"/>
    /// </summary>
    public sealed class PoolableObject : MonoBehaviour
    {
        /// <summary>
        /// Is the object currently in a pool?
        /// </summary>
        [field: NonSerialized]
        public bool InPool { get; internal set; }

        /// <summary>
        /// The pool the object was instantiated from
        /// </summary>
        [field: NonSerialized]
        public ObjectPool SourcePool { get; internal set; }

        //static System.Collections.Generic.List<Component> cacheList = new System.Collections.Generic.List<Component>();
        static Type ComponentType = typeof(Component);
        //static bool cacheIsCurrentlyUsed = false;

        [NonSerialized]
        Transform _transform;
        public new Transform transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GetComponent<Transform>();
                }
                return _transform;
            }
        }

        [NonSerialized]
        internal bool startCalledAutomatically = false;


        [NonSerialized]
        ComponentPath[] unserializedComponents = null;

        [NonSerialized]
        internal object CommonlyUsedComponent = null;

        internal T GetCommonComponent<T>()
        {
            if (CommonlyUsedComponent != null && CommonlyUsedComponent is T)
            {
                return (T)CommonlyUsedComponent;
            }
            else
            {
                var c = GetComponent<T>();
                CommonlyUsedComponent = c;
                return c;
            }
        }

        void Awake()
        {
            GetAllComponents();
        }

        /// <summary>
        /// Gets all components on the object, including all children
        /// </summary>
        internal ComponentPath[] GetAllComponents()
        {
            if (unserializedComponents == null)
            {
                unserializedComponents = RecursiveGetComponents(transform).ToArray();
            }
            return unserializedComponents;
        }

        /// <summary>
        /// Recursively gets all components on a transform, including all children
        /// </summary>
        /// <param name="t">The transform to get the components on</param>
        /// <returns>Returns a list of all the components on the object's hierarchy</returns>
        internal IEnumerable<ComponentPath> RecursiveGetComponents(Transform t)
        {
            //bool isUsingCache = false;
            //try
            //{
                System.Collections.Generic.List<Component> list = new System.Collections.Generic.List<Component>();
                /*if (cacheIsCurrentlyUsed)
                {
                    currentCache = 
                }
                else
                {
                    currentCache = cacheList;
                    cacheIsCurrentlyUsed = true;
                    isUsingCache = true;
                }*/
                return RecursiveGetComponents(0, t, list);
            /*}
            finally
            {
                if (!isUsingCache)
                {
                    cacheIsCurrentlyUsed = false;
                }
            }*/
        }

        /// <summary>
        /// Recursively gets the components on an object's hiearchy
        /// </summary>
        /// <param name="SiblingHash">The hash to make it easier to identify components in the hierarchy</param>
        /// <param name="t">The transform to traverse</param>
        /// <param name="reusableList">A reusable list to help cache the result</param>
        /// <returns></returns>
        IEnumerable<ComponentPath> RecursiveGetComponents(int SiblingHash, Transform t, System.Collections.Generic.List<Component> reusableList)
        {
            reusableList.Clear();
            t.GetComponents(ComponentType, reusableList);
            for (int i = 0; i < reusableList.Count; i++)
            {
                if (reusableList[i] != null)
                {
                    yield return new ComponentPath(SiblingHash, reusableList[i]);
                }
            }
            for (int i = 0; i < t.childCount; i++)
            {
                foreach (var item in RecursiveGetComponents(Utilities.HashUtilities.CombineHashCodes(SiblingHash, i), t.GetChild(i), reusableList))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns the object to the pool is came from
        /// </summary>
        public void ReturnToPool()
        {
            UnboundCoroutine.Start(ReturnToPoolRoutine(this, 0f));
        }

        static IEnumerator ReturnToPoolRoutine(PoolableObject obj, Func<bool> isDone)
        {
            while (true)
            {
                yield return null;

                if (obj == null || obj.gameObject == null || obj.InPool)
                {
                    yield break;
                }

                if (!isDone())
                {
                    continue;
                }

                if (obj.SourcePool != null)
                {
                    obj.SourcePool.ReturnToPool(obj);
                }
                else
                {
                    Destroy(obj.gameObject);
                }

                break;
            }
        }

        static IEnumerator ReturnToPoolRoutine(PoolableObject obj, float time)
        {
            yield return null;

            if (obj == null || obj.gameObject == null || obj.InPool)
            {
                yield break;
            }

            if (time > 0f)
            {
                if (obj.SourcePool != null)
                {
                    obj.SourcePool.ReturnToPool(obj, time);
                }
                else
                {
                    Destroy(obj.gameObject, time);
                }
            }
            else
            {
                if (obj.SourcePool != null)
                {
                    obj.SourcePool.ReturnToPool(obj);
                }
                else
                {
                    Destroy(obj.gameObject);
                }
            }
        }

        /// <summary>
        /// Returns the object to the pool it came from
        /// </summary>
        /// <param name="time">A time delay before it's returned to the pool</param>
        public void ReturnToPool(float time)
        {
            UnboundCoroutine.Start(ReturnToPoolRoutine(this, time));
        }

        /// <summary>
        /// Returns the object to the pool only when isDone returns true
        /// </summary>
        /// <param name="time">The delegate that, when it returns true, will cause the object to get deleted</param>
        public void ReturnToPool(Func<bool> isDone)
        {
            UnboundCoroutine.Start(ReturnToPoolRoutine(this, isDone));
        }

    }
}
