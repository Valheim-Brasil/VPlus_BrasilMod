using System;
using HarmonyLib;
using ValheimPlus.RPC;
using ValheimPlus.Configurations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ValheimPlus.APIs;

namespace ValheimPlus
{
    /// <summary>
    /// Sync server config to clients
    /// </summary>
    [HarmonyPatch(typeof(Game), "Start")]
    public static class GameStartPatch
    {
        private static void Prefix()
        {
            ZRoutedRpc.instance.Register("VPlusConfigSync", new Action<long, ZPackage>(VPlusConfigSync.RPC_VPlusConfigSync));
            ZLog.Log((object) "Register AcRoutedHandshake RPC");
            ZRoutedRpc.instance.Register<ZPackage>("AcRoutedHandshake", new Action<long, ZPackage>(AntiAnyMods.Rpc.AcRoutedHandshake));
        }
    }


    /// <summary>
    /// Alter game difficulty damage scale
    /// </summary>
    [HarmonyPatch(typeof(Game), "GetDifficultyDamageScale")]
    public static class ChangeDifficultyScaleDamage
    {
        private static bool Prefix(ref Game __instance, ref Vector3 pos, ref float __result)
        {
            if (Configuration.Current.Game.IsEnabled)
            {
                int playerDifficulty = __instance.GetPlayerDifficulty(pos);
                __result = 1f + (float)(playerDifficulty - 1) * Configuration.Current.Game.gameDifficultyDamageScale;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Alter game difficulty health scale
    /// </summary>
    [HarmonyPatch(typeof(Game), "GetDifficultyHealthScale")]
    public static class ChangeDifficultyScaleHealth
    {
        private static bool Prefix(ref Game __instance, ref Vector3 pos, ref float __result)
        {
            if (Configuration.Current.Game.IsEnabled)
            {
                int playerDifficulty = __instance.GetPlayerDifficulty(pos);
                __result = 1f + (float)(playerDifficulty - 1) * Configuration.Current.Game.gameDifficultyHealthScale;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Alter player difficulty scale
    /// </summary>
    [HarmonyPatch(typeof(Game), "GetPlayerDifficulty")]
    public static class ChangePlayerDifficultyCount
    {
        private static bool Prefix(ref Game __instance, ref Vector3 pos, ref int __result)
        {
            if (Configuration.Current.Game.IsEnabled)
            {
                if (Configuration.Current.Game.setFixedPlayerCountTo > 0)
                {
                    __result = Configuration.Current.Game.setFixedPlayerCountTo + Configuration.Current.Game.extraPlayerCountNearby;
                    return false;
                }

                int num = Player.GetPlayersInRangeXZ(pos, Configuration.Current.Game.difficultyScaleRange);

                if (num < 1)
                {
                    num = 1;
                }

                __result = num + Configuration.Current.Game.extraPlayerCountNearby;
                return false;
            }

            return true;
        }
    }
    
    /// <summary>
    /// Remove Bird Arrival
    /// </summary>
    [HarmonyPatch(typeof (Game), "UpdateRespawn")]
    public static class RemoveBirdArrival
    {
        private static MethodInfo func_sendText = AccessTools.Method(typeof (Chat), "SendText");

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index].Calls(RemoveBirdArrival.func_sendText))
                {
                    list.RemoveRange(index - 3, 4);
                    break;
                }
            }
            return list.AsEnumerable<CodeInstruction>();
        }
    }
    
    /// <summary>
    /// Player API - Update Player
    /// </summary>
    [HarmonyPatch(typeof (Game), "SpawnPlayer")]
    internal class PlayerSpawnPatch
    {
        private static bool Prefix() => true;

        private static void Postfix()
        {
            APIs.GLOBAL.playerApi = new Player_API(ref Player.m_localPlayer);
            APIs.GLOBAL.starvationSystem.setPlayerInstance(ref Player.m_localPlayer);
        }
    }
}