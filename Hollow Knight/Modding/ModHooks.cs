﻿using Modding.Delegates;
using System;
﻿using GlobalEnums;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Modding
{
	public class ModHooks
	{
        internal static void OnDrawBlackBorders(List<GameObject> borders)
        {
            if (ModHooks.DrawBlackBordersHook == null)
            {
                return;
            }
            foreach (Action<List<GameObject>> action in ModHooks.DrawBlackBordersHook.GetInvocationList())
            {
                try
                {
                    action(borders);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static void OnAttack(AttackDirection dir)
        {
            if (ModHooks.AttackHook == null)
            {
                return;
            }
            foreach (Action<AttackDirection> action in ModHooks.AttackHook.GetInvocationList())
            {
                try
                {
                    action(dir);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static void AfterAttack(AttackDirection dir)
        {
            if (ModHooks.AfterAttackHook == null)
            {
                return;
            }
            foreach (Action<AttackDirection> action in ModHooks.AfterAttackHook.GetInvocationList())
            {
                try
                {
                    action(dir);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static Vector2 DashVelocityChange(Vector2 change)
        {
            if (ModHooks.DashVectorHook == null)
            {
                return change;
            }
            foreach (Func<Vector2, Vector2> func in ModHooks.DashVectorHook.GetInvocationList())
            {
                try
                {
                    change = func(change);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return change;
        }

        internal static int OnTakeDamage(ref int hazardType, int damage)
        {
            if (ModHooks.TakeDamageHook == null)
            {
                return damage;
            }
            foreach (TakeDamageProxy takeDamageProxy in ModHooks.TakeDamageHook.GetInvocationList())
            {
                try
                {
                    damage = takeDamageProxy(ref hazardType, damage);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return damage;
        }

        internal static int AfterTakeDamage(int hazardType, int damageAmount)
        {
            if (ModHooks.AfterTakeDamageHook == null)
            {
                return damageAmount;
            }
            foreach (AfterTakeDamageHandler afterTakeDamageHandler in ModHooks.AfterTakeDamageHook.GetInvocationList())
            {
                try
                {
                    damageAmount = afterTakeDamageHandler(hazardType, damageAmount);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return damageAmount;
        }

        /// <summary>
        ///     Called by the game in PlayerData.GetBool
        /// </summary>
        /// <param name="target">Target Field Name</param>
        internal static bool GetPlayerBool(string target)
        {
            bool flag = PlayerData.instance.GetBoolInternal(target);
            if (ModHooks.GetPlayerBoolHook == null)
            {
                return flag;
            }
            foreach (GetBoolProxy getBoolProxy in ModHooks.GetPlayerBoolHook.GetInvocationList())
            {
                try
                {
                    flag = getBoolProxy(target, flag);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return flag;
        }

        /// <summary>
        ///     Called by the game in PlayerData.GetFloat
        /// </summary>
        /// <param name="target">Target Field Name</param>
        internal static float GetPlayerFloat(string target)
        {
            float num = PlayerData.instance.GetFloatInternal(target);
            if (ModHooks.GetPlayerFloatHook == null)
            {
                return num;
            }
            foreach (GetFloatProxy getFloatProxy in ModHooks.GetPlayerFloatHook.GetInvocationList())
            {
                try
                {
                    num = getFloatProxy(target, num);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return num;
        }

        /// <summary>
        ///     Called by the game in PlayerData.GetInt
        /// </summary>
        /// <param name="target">Target Field Name</param>
        internal static int GetPlayerInt(string target)
        {
            int num = PlayerData.instance.GetIntInternal(target);
            if (ModHooks.GetPlayerIntHook == null)
            {
                return num;
            }
            foreach (GetIntProxy getIntProxy in ModHooks.GetPlayerIntHook.GetInvocationList())
            {
                try
                {
                    num = getIntProxy(target, num);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return num;
        }

        /// <summary>
        ///     Called by the game in PlayerData.GetString
        /// </summary>
        /// <param name="target">Target Field Name</param>
        internal static string GetPlayerString(string target)
        {
            string text = PlayerData.instance.GetStringInternal(target);
            if (ModHooks.GetPlayerStringHook == null)
            {
                return text;
            }
            foreach (GetStringProxy getStringProxy in ModHooks.GetPlayerStringHook.GetInvocationList())
            {
                try
                {
                    text = getStringProxy(target, text);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return text;
        }

        /// <summary>
        ///     Called whenever localization specific strings are requested
        /// </summary>
        /// <remarks>N/A</remarks>
        internal static string LanguageGet(string key, string sheet)
        {
            string res = "";
            //string res = Language.GetInternal(key, sheet);

            if (LanguageGetHook == null)
                return res;

            Delegate[] invocationList = LanguageGetHook.GetInvocationList();

            foreach (LanguageGetProxy toInvoke in invocationList)
            {
                try
                {
                    res = toInvoke(key, sheet, res);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }

            return res;
        }

        /// <summary>
        ///     Called by the game in PlayerData.GetVariable
        /// </summary>
        /// <param name="target">Target Field Name</param>
        internal static T GetPlayerVariable<T>(string target)
        {
            Type typeFromHandle = typeof(T);
            if (typeFromHandle == typeof(bool))
            {
                return (T)((object)ModHooks.GetPlayerBool(target));
            }
            if (typeFromHandle == typeof(int))
            {
                return (T)((object)ModHooks.GetPlayerInt(target));
            }
            if (typeFromHandle == typeof(float))
            {
                return (T)((object)ModHooks.GetPlayerFloat(target));
            }
            if (typeFromHandle == typeof(string))
            {
                return (T)((object)ModHooks.GetPlayerString(target));
            }
            if (typeFromHandle == typeof(Vector3))
            {
                return (T)((object)ModHooks.GetPlayerVector3(target));
            }
            T t = PlayerData.instance.GetVariableInternal<T>(target);
            if (ModHooks.GetPlayerVariableHook == null)
            {
                return t;
            }
            foreach (GetVariableProxy getVariableProxy in ModHooks.GetPlayerVariableHook.GetInvocationList())
            {
                try
                {
                    t = (T)((object)getVariableProxy(typeFromHandle, target, t));
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return t;
        }

        /// <summary>
        ///     Called by the game in PlayerData.GetVector3
        /// </summary>
        /// <param name="target">Target Field Name</param>
        internal static Vector3 GetPlayerVector3(string target)
        {
            Vector3 vector = PlayerData.instance.GetVector3Internal(target);
            if (ModHooks.GetPlayerVector3Hook == null)
            {
                return vector;
            }
            foreach (GetVector3Proxy getVector3Proxy in ModHooks.GetPlayerVector3Hook.GetInvocationList())
            {
                try
                {
                    vector = getVector3Proxy(target, vector);
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return vector;
        }

        /// <summary>
        ///     Called by the game in PlayerData.SetBool
        /// </summary>
        /// <param name="target">Target Field Name</param>
        /// <param name="val">Value to set</param>
        internal static void SetPlayerBool(string target, bool val)
        {
            bool flag = val;
            if (ModHooks.SetPlayerBoolHook != null)
            {
                foreach (SetBoolProxy setBoolProxy in ModHooks.SetPlayerBoolHook.GetInvocationList())
                {
                    try
                    {
                        flag = setBoolProxy(target, flag);
                    }
                    catch (Exception message)
                    {
                        Logger.LogError(message);
                    }
                }
            }
            PlayerData.instance.SetBoolInternal(target, flag);
        }

        /// <summary>
        ///     Called by the game in PlayerData.SetFloat
        /// </summary>
        /// <param name="target">Target Field Name</param>
        /// <param name="val">Value to set</param>
        internal static void SetPlayerFloat(string target, float val)
        {
            float value = val;
            if (ModHooks.SetPlayerFloatHook != null)
            {
                foreach (SetFloatProxy setFloatProxy in ModHooks.SetPlayerFloatHook.GetInvocationList())
                {
                    try
                    {
                        val = setFloatProxy(target, val);
                    }
                    catch (Exception message)
                    {
                        Logger.LogError(message);
                    }
                }
            }
            PlayerData.instance.SetFloatInternal(target, value);
        }

        /// <summary>
        ///     Called by the game in PlayerData.SetInt
        /// </summary>
        /// <param name="target">Target Field Name</param>
        /// <param name="val">Value to set</param>
        internal static void SetPlayerInt(string target, int val)
        {
            int num = val;
            if (ModHooks.SetPlayerIntHook != null)
            {
                foreach (SetIntProxy setIntProxy in ModHooks.SetPlayerIntHook.GetInvocationList())
                {
                    try
                    {
                        num = setIntProxy(target, num);
                    }
                    catch (Exception message)
                    {
                        Logger.LogError(message);
                    }
                }
            }
            PlayerData.instance.SetIntInternal(target, num);
        }

        /// <summary>
        ///     Called by the game in PlayerData.SetString
        /// </summary>
        /// <param name="target">Target Field Name</param>
        /// <param name="val">Value to set</param>
        internal static void SetPlayerString(string target, string val)
        {
            string text = val;
            if (ModHooks.SetPlayerStringHook != null)
            {
                foreach (SetStringProxy setStringProxy in ModHooks.SetPlayerStringHook.GetInvocationList())
                {
                    try
                    {
                        text = setStringProxy(target, text);
                    }
                    catch (Exception message)
                    {
                        Logger.LogError(message);
                    }
                }
            }
            PlayerData.instance.SetStringInternal(target, text);
        }

        /// <summary>
        ///     Called by the game in PlayerData.SetVariable
        /// </summary>
        /// <param name="target">Target Field Name</param>
        /// <param name="orig">Value to set</param>
        internal static void SetPlayerVariable<T>(string target, T orig)
        {
            Type typeFromHandle = typeof(T);
            if (typeFromHandle == typeof(bool))
            {
                ModHooks.SetPlayerBool(target, (bool)((object)orig));
                return;
            }
            if (typeFromHandle == typeof(int))
            {
                ModHooks.SetPlayerInt(target, (int)((object)orig));
                return;
            }
            if (typeFromHandle == typeof(float))
            {
                ModHooks.SetPlayerFloat(target, (float)((object)orig));
                return;
            }
            if (typeFromHandle == typeof(string))
            {
                ModHooks.SetPlayerString(target, (string)((object)orig));
                return;
            }
            if (typeFromHandle == typeof(Vector3))
            {
                ModHooks.SetPlayerVector3(target, (Vector3)((object)orig));
                return;
            }
            T t = orig;
            if (ModHooks.SetPlayerVariableHook != null)
            {
                foreach (SetVariableProxy setVariableProxy in ModHooks.SetPlayerVariableHook.GetInvocationList())
                {
                    try
                    {
                        t = (T)((object)setVariableProxy(typeFromHandle, target, t));
                    }
                    catch (Exception message)
                    {
                        Logger.LogError(message);
                    }
                }
            }
            PlayerData.instance.SetVariableInternal<T>(target, t);
        }

        /// <summary>
        ///     Called by the game in PlayerData.SetVector3
        /// </summary>
        /// <param name="target">Target Field Name</param>
        /// <param name="orig">Value to set</param>
        internal static void SetPlayerVector3(string target, Vector3 orig)
        {
            Vector3 vector = orig;
            if (ModHooks.SetPlayerVector3Hook != null)
            {
                foreach (SetVector3Proxy setVector3Proxy in ModHooks.SetPlayerVector3Hook.GetInvocationList())
                {
                    try
                    {
                        vector = setVector3Proxy(target, vector);
                    }
                    catch (Exception message)
                    {
                        Logger.LogError(message);
                    }
                }
            }
            PlayerData.instance.SetVector3Internal(target, vector);
        }

        internal static bool OnDashPressed()
        {
            Logger.LogFine("OnDashPressed Invoked");
            if (ModHooks.DashPressedHook == null)
            {
                return false;
            }
            bool flag = false;
            foreach (Func<bool> func in ModHooks.DashPressedHook.GetInvocationList())
            {
                try
                {
                    flag |= func();
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
            return flag;
        }

        internal static void OnDoAttack()
        {
            Logger.LogFine("OnDoAttack Invoked");
            if (ModHooks.DoAttackHook == null)
            {
                return;
            }
            foreach (Action action in ModHooks.DoAttackHook.GetInvocationList())
            {
                try
                {
                    action();
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static void OnApplicationQuit()
        {
            Logger.LogFine("OnApplicationQuit Invoked");
            if (ModHooks.ApplicationQuitHook == null)
            {
                return;
            }
            foreach (Action action in ModHooks.ApplicationQuitHook.GetInvocationList())
            {
                try
                {
                    action();
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static void OnBeforePlayerDead()
        {
            Logger.LogFine("OnBeforePlayerDead Invoked");
            if (ModHooks.BeforePlayerDeadHook == null)
            {
                return;
            }
            foreach (Action action in ModHooks.BeforePlayerDeadHook.GetInvocationList())
            {
                try
                {
                    action();
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static void OnAfterPlayerDead()
        {
            Logger.LogFine("OnAfterPlayerDead Invoked");
            if (ModHooks.AfterPlayerDeadHook == null)
            {
                return;
            }
            foreach (Action action in ModHooks.AfterPlayerDeadHook.GetInvocationList())
            {
                try
                {
                    action();
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        internal static void OnNewGame()
        {
            Logger.LogFine("OnNewGame Invoked");
            if (ModHooks.NewGameHook == null)
            {
                return;
            }
            foreach (Action action in ModHooks.NewGameHook.GetInvocationList())
            {
                try
                {
                    action();
                }
                catch (Exception message)
                {
                    Logger.LogError(message);
                }
            }
        }

        /// <summary>
        ///     Called whenever blue health is updated
        /// </summary>
        internal static int OnBlueHealth()
        {
            Logger.LogFine("OnBlueHealth Invoked");

            int result = 0;
            if (BlueHealthHook == null)
            {
                return result;
            }

            Delegate[] invocationList = BlueHealthHook.GetInvocationList();

            foreach (Func<int> toInvoke in invocationList)
            {
                try
                {
                    result = toInvoke.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }

            return result;
        }

        /// <summary>
        ///     Called whenever nail strikes something
        /// </summary>
        internal static void OnSlashHit(Collider2D otherCollider, GameObject gameObject)
        {
            Logger.LogFine("OnSlashHit Invoked");

            if (otherCollider == null)
            {
                return;
            }

            if (SlashHitHook == null)
            {
                return;
            }

            Delegate[] invocationList = SlashHitHook.GetInvocationList();

            foreach (SlashHitHandler toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(otherCollider, gameObject);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called after player values for charms have been set
        /// </summary>
        /// <remarks>HeroController.CharmUpdate</remarks>
        internal static void OnCharmUpdate(PlayerData pd, HeroController hc)
        {
            Logger.LogFine("OnCharmUpdate Invoked");

            if (CharmUpdateHook == null)
            {
                return;
            }

            Delegate[] invocationList = CharmUpdateHook.GetInvocationList();

            foreach (CharmUpdateHandler toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(pd, hc);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called directly after a save has been loaded
        /// </summary>
        /// <remarks>GameManager.LoadGame</remarks>
        public static void OnSavegameLoad(int id)
        {
            Logger.LogFine("OnSavegameLoad Invoked");

            if (SavegameLoadHook == null)
            {
                return;
            }

            Delegate[] invocationList = SavegameLoadHook.GetInvocationList();

            foreach (Action<int> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(id);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called whenever the hero updates
        /// </summary>
        /// <remarks>HeroController.Update</remarks>
        public static event Action HeroUpdateHook;

        /// <summary>
        ///     Called whenever the hero updates
        /// </summary>
        /// <remarks>HeroController.Update</remarks>
        public static void OnHeroUpdate()
        {
            //Logger.APILogger.LogFine("OnHeroUpdate Invoked");

            if (HeroUpdateHook == null)
            {
                return;
            }

            Delegate[] invocationList = HeroUpdateHook.GetInvocationList();

            foreach (Action toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Gets if the mod is currently enabled.
        /// </summary>
        /// <param name="mod">The togglable mod to check.</param>
        /// <returns></returns>
        public static bool ModEnabled(
            ITogglableMod mod
        ) => ModEnabled(mod.GetType());

        /// <summary>
        /// Gets if a mod is currently enabled.
        /// </summary>
        /// <param name="name">The name of the mod to check.</param>
        /// <returns></returns>
        public static bool ModEnabled(
            string name
        ) => GetAllMods(true, false).Any(m => m.GetName() == name);

        /// <summary>
        /// Gets if a mod is currently enabled.
        /// </summary>
        /// <param name="type">The type of the mod to check.</param>
        /// <returns></returns>
        public static bool ModEnabled(
            Type type
        ) => GetAllMods(true, false).Any(m => type.IsAssignableFrom(m.GetType()));

        internal static void OnFinishedLoadingMods()
        {
            if (_finishedLoadingModsHook == null)
            {
                return;
            }

            Delegate[] invocationList = _finishedLoadingModsHook.GetInvocationList();

            foreach (Action toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }

            finishedLoadingMods = true;
        }

        private static event Action _finishedLoadingModsHook;
        static bool finishedLoadingMods = false;

        /// <summary>
        /// Event invoked when mods have finished loading. If modloading has already finished, subscribers will be invoked immediately.
        /// </summary>
        public static event Action FinishedLoadingModsHook
        {
            add
            {
                _finishedLoadingModsHook += value;

                //if (!ModLoader.LoadState.HasFlag(ModLoader.ModLoadState.Loaded))
                //return;

                if (!finishedLoadingMods)
                {
                    return;
                }

                try
                {
                    value.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
            remove => _finishedLoadingModsHook -= value;
        }

        /// <summary>
        ///     Called when Hero recovers Soul from hitting enemies
        /// </summary>
        /// <returns>The amount of soul to recover</returns>
        public static event Func<int, int> SoulGainHook;

        /// <summary>
        ///     Called when Hero recovers Soul from hitting enemies
        /// </summary>
        internal static int OnSoulGain(int num)
        {
            //Logger.LogFine("OnSoulGain Invoked");

            if (SoulGainHook == null)
            {
                return num;
            }

            Delegate[] invocationList = SoulGainHook.GetInvocationList();

            foreach (Func<int, int> toInvoke in invocationList)
            {
                try
                {
                    num = toInvoke.Invoke(num);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }

            return num;
        }

        /// <summary>
        ///     Called directly before save has been saved to allow for changes to the data before persisted.
        /// </summary>
        /// <remarks>GameManager.SaveGame</remarks>
        public static event Action<SaveGameData> BeforeSavegameSaveHook;

        /// <summary>
        ///     Called directly before save has been saved to allow for changes to the data before persisted.
        /// </summary>
        /// <remarks>GameManager.SaveGame</remarks>
        internal static void OnBeforeSaveGameSave(SaveGameData data)
        {
            Logger.LogFine("OnBeforeSaveGameSave Invoked");

            if (BeforeSavegameSaveHook == null)
            {
                return;
            }

            Delegate[] invocationList = BeforeSavegameSaveHook.GetInvocationList();

            foreach (Action<SaveGameData> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(data);
                }

                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called directly after a save has been loaded.  Allows for accessing SaveGame instance.
        /// </summary>
        /// <remarks>GameManager.LoadGame</remarks>
        public static event Action<SaveGameData> AfterSavegameLoadHook;

        /// <summary>
        ///     Called directly after a save has been loaded.  Allows for accessing SaveGame instance.
        /// </summary>
        /// <remarks>GameManager.LoadGame</remarks>
        internal static void OnAfterSaveGameLoad(SaveGameData data)
        {
            Logger.LogFine("OnAfterSaveGameLoad Invoked");

            if (AfterSavegameLoadHook == null)
            {
                return;
            }

            Delegate[] invocationList = AfterSavegameLoadHook.GetInvocationList();

            foreach (Action<SaveGameData> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(data);
                }

                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called after a game has been cleared from a slot.
        /// </summary>
        public static event Action<int> AfterSaveGameClearHook;

        /// <summary>
        ///     Called after a game has been cleared from a slot.
        /// </summary>
        internal static void OnAfterSaveGameClear(int saveSlot)
        {
            Logger.LogFine("OnAfterSaveGameClear Invoked");

            if (AfterSaveGameClearHook == null)
            {
                return;
            }

            Delegate[] invocationList = AfterSaveGameClearHook.GetInvocationList();

            foreach (Action<int> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(saveSlot);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }


        /// <summary>
        ///     Called before a save file is deleted
        /// </summary>
        /// <remarks>GameManager.ClearSaveFile</remarks>
        public static event Action<int> SavegameClearHook;

        /// <summary>
        ///     Called before a save file is deleted
        /// </summary>
        /// <remarks>GameManager.ClearSaveFile</remarks>
        internal static void OnSavegameClear(int id)
        {
            Logger.LogFine("OnSavegameClear Invoked");

            if (SavegameClearHook == null)
            {
                return;
            }

            Delegate[] invocationList = SavegameClearHook.GetInvocationList();

            foreach (Action<int> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(id);
                }

                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }


        /// <summary>
        ///     Called directly after a save has been saved
        /// </summary>
        /// <remarks>GameManager.SaveGame</remarks>
        public static event Action<int> SavegameSaveHook;

        /// <summary>
        ///     Called directly after a save has been saved
        /// </summary>
        /// <remarks>GameManager.SaveGame</remarks>
        public static void OnSavegameSave(int id)
        {
            Logger.LogFine("OnSavegameSave Invoked");

            if (SavegameSaveHook == null)
            {
                return;
            }

            Delegate[] invocationList = SavegameSaveHook.GetInvocationList();

            foreach (Action<int> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(id);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called whenever the player heals
        /// </summary>
        /// <remarks>PlayerData.health</remarks>
        internal static int BeforeAddHealth(int amount)
        {
            Debug.Log("BeforeAddHealth Invoked");

            if (BeforeAddHealthHook == null)
            {
                return amount;
            }

            Delegate[] invocationList = BeforeAddHealthHook.GetInvocationList();

            foreach (Func<int, int> toInvoke in invocationList)
            {
                try
                {
                    amount = toInvoke.Invoke(amount);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            return amount;
        }

        /// <summary>
        ///     Called whenever focus cost is calculated
        /// </summary>
        internal static float OnFocusCost()
        {
            Logger.LogFine("OnFocusCost Invoked");

            float result = 1f;

            if (FocusCostHook == null)
            {
                return result;
            }

            Delegate[] invocationList = FocusCostHook.GetInvocationList();

            foreach (Func<float> toInvoke in invocationList)
            {
                try
                {
                    result = toInvoke.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }

            return result;
        }

        /// <summary>
        ///     Called right before a scene gets loaded, can change which scene gets loaded
        /// </summary>
        /// <remarks>N/A</remarks>
        internal static string BeforeSceneLoad(string sceneName)
        {
            //Debug.Log("BeforeSceneLoad Invoked");

            if (BeforeSceneLoadHook == null)
            {
                return sceneName;
            }

            Delegate[] invocationList = BeforeSceneLoadHook.GetInvocationList();

            foreach (Func<string, string> toInvoke in invocationList)
            {
                try
                {
                    sceneName = toInvoke.Invoke(sceneName);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            return sceneName;
        }

        /// <summary>
        ///     Called after a new Scene has been loaded
        /// </summary>
        /// <remarks>N/A</remarks>
        internal static void OnSceneChanged(string targetScene)
        {
            Debug.Log("OnSceneChanged Invoked");

            if (SceneChanged == null)
            {
                return;
            }

            Delegate[] invocationList = SceneChanged.GetInvocationList();

            foreach (Action<string> toInvoke in invocationList)
            {
                try
                {
                    toInvoke.Invoke(targetScene);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///     Called after a new Scene has been loaded
        /// </summary>
        /// <remarks>N/A</remarks>
        public static event Action<string> SceneChanged;

        /// <summary>
        ///     Called right before a scene gets loaded, can change which scene gets loaded
        /// </summary>
        /// <remarks>N/A</remarks>
        public static event Func<string, string> BeforeSceneLoadHook;

        /// <summary>
        ///     Called whenever focus cost is calculated, allows a focus cost multiplier.
        /// </summary>
        public static event Func<float> FocusCostHook;

        /// <summary>
        ///     Called whenever the player heals, overrides health added.
        /// </summary>
        /// <remarks>PlayerData.health</remarks>
        public static event Func<int, int> BeforeAddHealthHook;

        /// <summary>
        ///     Called directly after a save has been loaded
        /// </summary>
        /// <remarks>GameManager.LoadGame</remarks>
        public static event Action<int> SavegameLoadHook;

        /// <summary>
        ///     Called after player values for charms have been set
        /// </summary>
        /// <see cref="CharmUpdateHandler"/>
        /// <remarks>HeroController.CharmUpdate</remarks>
        public static event CharmUpdateHandler CharmUpdateHook;

        /// <summary>
        ///     Called whenever nail strikes something
        /// </summary>
        /// <see cref="SlashHitHandler"/>
        public static event SlashHitHandler SlashHitHook;

        /// <summary>
        ///     Called whenever blue health is updated
        /// </summary>
        public static event Func<int> BlueHealthHook;

        /// <summary>
		///     Called when a SceneManager calls DrawBlackBorders and creates boarders for a scene. You may use or modify the
		///     bounds of an area of the scene with these.
		/// </summary>
		/// <remarks>SceneManager.DrawBlackBorders</remarks>
        public static event Action<List<GameObject>> DrawBlackBordersHook;


        /// <summary>
        ///     Called whenever the player attacks
        /// </summary>
        /// <remarks>HeroController.Attack</remarks>
        public static event Action<AttackDirection> AttackHook;

        /// <summary>
        ///     Called at the end of the attack function
        /// </summary>
        /// <remarks>HeroController.Attack</remarks>
        public static event Action<AttackDirection> AfterAttackHook;

        /// <summary>
        ///     Called during dash function to change velocity
        /// </summary>
        /// <returns>A changed vector.</returns>
        /// <remarks>HeroController.Dash</remarks>
        public static event Func<Vector2, Vector2> DashVectorHook;

        /// <summary>
        ///     Called when damage is dealt to the player
        /// </summary>
        /// <see cref="T:Modding.TakeDamageProxy" />
        /// <remarks>HeroController.TakeDamage</remarks>
        public static event TakeDamageProxy TakeDamageHook;

        /// <summary>
        ///     Called at the end of the take damage function
        /// </summary>
        /// <see cref="T:Modding.AfterTakeDamageHandler" />
        public static event AfterTakeDamageHandler AfterTakeDamageHook;

        /// <summary>
        ///     Called whenever the dash key is pressed.
        ///     Returns whether or not to override normal dash functionality - if true, preventing a normal dash
        /// </summary>
        /// <remarks>HeroController.LookForQueueInput</remarks>
        public static event Func<bool> DashPressedHook;

        /// <summary>
        ///     Called at the start of the DoAttack function
        /// </summary>
        public static event Action DoAttackHook;

        /// <summary>
        ///     Called when the game is fully closed
        /// </summary>
        /// <remarks>GameManager.OnApplicationQuit</remarks>
        public static event Action ApplicationQuitHook;

        /// <summary>
        ///     Called when the player dies
        /// </summary>
        /// <remarks>GameManager.PlayerDead</remarks>
        public static event Action BeforePlayerDeadHook;

        /// <summary>
        ///     Called after the player dies
        /// </summary>
        /// <remarks>GameManager.PlayerDead</remarks>
        public static event Action AfterPlayerDeadHook;

        /// <summary>
        ///     Called whenever a new game is started
        /// </summary>
        /// <remarks>GameManager.LoadFirstScene</remarks>
        public static event Action NewGameHook;
        /// <summary>
        ///     Called whenever localization specific strings are requested
        /// </summary>
        /// <remarks>N/A</remarks>
        public static event LanguageGetProxy LanguageGetHook;
		/// <summary>
		///     Called when anything in the game tries to set a bool in player data
		/// </summary>
		/// <remarks>PlayerData.SetBool</remarks>
		/// <see cref="SetBoolProxy" />
		public static event SetBoolProxy SetPlayerBoolHook;
		/// <summary>
		///     Called when anything in the game tries to get a bool from player data
		/// </summary>
		/// <remarks>PlayerData.GetBool</remarks>
		public static event GetBoolProxy GetPlayerBoolHook;
		/// <summary>
		///     Called when anything in the game tries to set an int in player data
		/// </summary>
		/// <remarks>PlayerData.SetInt</remarks>
		public static event SetIntProxy SetPlayerIntHook;
		/// <summary>
		///     Called when anything in the game tries to get an int from player data
		/// </summary>
		/// <remarks>PlayerData.GetInt</remarks>
		public static event GetIntProxy GetPlayerIntHook;
		/// <summary>
		///     Called when anything in the game tries to set a float in player data
		/// </summary>
		/// <remarks>PlayerData.SetFloat</remarks>
		public static event SetFloatProxy SetPlayerFloatHook;
		/// <summary>
		///     Called when anything in the game tries to get a float from player data
		/// </summary>
		/// <remarks>PlayerData.GetFloat</remarks>
		public static event GetFloatProxy GetPlayerFloatHook;
		/// <summary>
		///     Called when anything in the game tries to set a string in player data
		/// </summary>
		/// <remarks>PlayerData.SetString</remarks>
		public static event SetStringProxy SetPlayerStringHook;
		/// <summary>
		///     Called when anything in the game tries to get a string from player data
		/// </summary>
		/// <remarks>PlayerData.GetString</remarks>
		public static event GetStringProxy GetPlayerStringHook;
		/// <summary>
		///     Called when anything in the game tries to set a Vector3 in player data
		/// </summary>
		/// <remarks>PlayerData.SetVector3</remarks>
		public static event SetVector3Proxy SetPlayerVector3Hook;
		/// <summary>
		///     Called when anything in the game tries to get a Vector3 from player data
		/// </summary>
		/// <remarks>PlayerData.GetVector3</remarks>
		public static event GetVector3Proxy GetPlayerVector3Hook;
		/// <summary>
		///     Called when anything in the game tries to set a generic variable in player data
		/// </summary>
		/// <remarks>PlayerData.SetVariable</remarks>
		public static event SetVariableProxy SetPlayerVariableHook;
		/// <summary>
		///     Called when anything in the game tries to get a generic variable from player data
		/// </summary>
		/// <remarks>PlayerData.GetVariable</remarks>
		public static event GetVariableProxy GetPlayerVariableHook;
		/// <summary>
		///     Called when health is taken from the player
		/// </summary>
		/// <remarks>HeroController.TakeHealth</remarks>
		public static event TakeHealthProxy TakeHealthHook;

        static PropertyInfo loadedModsF;
        /// <summary>
        /// Returns an iterator over all mods.
        /// </summary>
        /// <param name="onlyEnabled">Should the iterator only contain enabled mods.</param>
        /// <param name="allowLoadError">Should the iterator contain mods which have load errors.</param>
        /// <returns></returns>
        public static IEnumerable<IMod> GetAllMods(bool onlyEnabled = false, bool allowLoadError = false)
        {
            if (loadedModsF == null)
            {
                loadedModsF = GetWeaverModType().GetProperty("LoadedMods", BindingFlags.Public | BindingFlags.Static);
                /*foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.GetName().Name == "WeaverCore")
                    {
                        var weaverModT = asm.GetType("WeaverCore.WeaverMod");
                        if (weaverModT != null)
                        {
                            loadedModsF = weaverModT.GetProperty("LoadedMods", BindingFlags.Public | BindingFlags.Static);
                            break;
                        }
                    }
                }*/
            }
            if (loadedModsF != null)
            {
                return (IEnumerable<IMod>)loadedModsF.GetValue(null);
            }
            else
            {
                return Enumerable.Empty<IMod>();
            }
        }

        static Assembly _weaverCoreAssembly;
        static Type _weaverModType;

        static Assembly GetWeaverCoreAssembly()
        {
            if (_weaverCoreAssembly == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.GetName().Name == "WeaverCore")
                    {
                        _weaverCoreAssembly = asm;
                        break;
                    }
                }
            }

            return _weaverCoreAssembly;
        }

        static Type GetWeaverModType()
        {
            if (_weaverModType == null)
            {
                _weaverModType = GetWeaverCoreAssembly().GetType("WeaverCore.WeaverMod");
                /*if (weaverModT != null)
                {
                    loadedModsF = weaverModT.GetProperty("LoadedMods", BindingFlags.Public | BindingFlags.Static);
                }*/
            }

            return _weaverModType;
        }
    }

    namespace Delegates
    {
        /// <summary>
        ///     Called after player values for charms have been set
        /// </summary>
        /// <param name="data">Current PlayerData</param>
        /// <param name="controller">Current HeroController</param>
        public delegate void CharmUpdateHandler(PlayerData data, HeroController controller);

        /// <summary>
        ///     Called whenever localization specific strings are requested
        /// </summary>
        /// <param name="key">The key within the sheet</param>
        /// <param name="sheetTitle">The title of the sheet</param>
        /// <param name="orig">The original localized value</param>
        /// <returns>The modified localization, return *current* to keep as-is.</returns>
        public delegate string LanguageGetProxy(string key, string sheetTitle, string orig);

        /// <summary>
        ///     Called when anything in the game tries to get a bool
        /// </summary>
        /// <param name="name">The field being gotten</param>
        /// <param name="orig">The original value of the bool</param>
        /// <returns>The bool, if you are overriding it, otherwise orig.</returns>
        public delegate bool GetBoolProxy(string name, bool orig);

        /// <summary>
        ///     Called when anything in the game tries to set a bool
        /// </summary>
        /// <param name="name">The field being set</param>
        /// <param name="orig">The original value the bool was being set to</param>
        /// <returns>The bool, if overridden, else orig.</returns>
        public delegate bool SetBoolProxy(string name, bool orig);

        /// <summary>
        ///     Called when anything in the game tries to get an int
        /// </summary>
        /// <param name="name">The field being gotten</param>
        /// <param name="orig">The original value of the field</param>
        /// <returns>The int, if overridden, else orig.</returns>
        public delegate int GetIntProxy(string name, int orig);

        /// <summary>
        ///     Called when anything in the game tries to set an int
        /// </summary>
        /// <param name="name">The field which is being set</param>
        /// <param name="orig">The original value</param>
        /// <returns>The int if overrode, else null</returns>
        public delegate int SetIntProxy(string name, int orig);

        /// <summary>
        ///     Called when anything in the game tries to get a float
        /// </summary>
        /// <param name="name">The field being set</param>
        /// <param name="orig">The original value</param>
        /// <returns>The value, if overrode, else null.</returns>
        public delegate float GetFloatProxy(string name, float orig);

        /// <summary>
        ///     Called when anything in the game tries to set a float
        /// </summary>
        /// <param name="name">The field being set</param>
        /// <param name="orig">The original value the float was being set to</param>
        /// <returns>The modified value of the set</returns>
        public delegate float SetFloatProxy(string name, float orig);

        /// <summary>
        ///     Called when anything in the game tries to get a string
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="res">The original value of the field</param>
        /// <returns>The modified value of the get</returns>
        public delegate string GetStringProxy(string name, string res);

        /// <summary>
        ///     Called when anything in the game tries to set a string
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="res">The original set value of the string</param>
        /// <returns>The modified value of the set</returns>
        public delegate string SetStringProxy(string name, string res);

        /// <summary>
        ///     Called when anything in the game tries to get a Vector3
        /// </summary>
        /// <param name="name">The name of the Vector3 field</param>
        /// <param name="orig">The original value of the field</param>
        /// <returns>The value to override the vector to</returns>
        public delegate Vector3 GetVector3Proxy(string name, Vector3 orig);

        /// <summary>
        ///     Called when anything in the game tries to set a Vector3
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="orig">The original value the field was being set to</param>
        /// <returns>The value to override the set to</returns>
        public delegate Vector3 SetVector3Proxy(string name, Vector3 orig);

        /// <summary>
        ///     Called when anything in the game tries to get a generic variable
        /// </summary>
        /// <param name="type">The type of the variable</param>
        /// <param name="name">The field being gotten</param>
        /// <param name="value">The original value of the field</param>
        /// <returns>The modified value</returns>
        public delegate object GetVariableProxy(Type type, string name, object value);

        /// <summary>
        ///     Called when anything in the game tries to set a generic variable
        /// </summary>
        /// <param name="type">The type of the variable</param>
        /// <param name="name">The name of the field being set</param>
        /// <param name="value">The original value the field was being set to</param>
        /// <returns>The new value of the field</returns>
        public delegate object SetVariableProxy(Type type, string name, object value);

        /// <summary>
        ///     Called when health is taken from the player
        /// </summary>
        /// <param name="damage">Amount of Damage</param>
        /// <returns>Modified Damaged</returns>
        public delegate int TakeHealthProxy(int damage);

        /// <summary>
        ///     Called when damage is dealt to the player
        /// </summary>
        /// <param name="hazardType">The type of hazard that caused the damage.</param>
        /// <param name="damage">Amount of Damage</param>
        /// <returns>Modified Damage</returns>
        public delegate int TakeDamageProxy(ref int hazardType, int damage);

        /// <summary>
        ///     Called at the end of the take damage function
        /// </summary>
        /// <param name="hazardType"></param>
        /// <param name="damageAmount"></param>
        public delegate int AfterTakeDamageHandler(int hazardType, int damageAmount);

        /// <summary>
        ///     Called whenever nail strikes something
        /// </summary>
        /// <param name="otherCollider">What the nail is colliding with</param>
        /// <param name="slash">The NailSlash gameObject</param>
        public delegate void SlashHitHandler(Collider2D otherCollider, GameObject slash);
    }
}
