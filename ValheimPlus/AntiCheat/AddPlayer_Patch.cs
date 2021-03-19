using HarmonyLib;
using UnityEngine;
using ValheimPlus.Configurations;

namespace ValheimPlus.AntiCheat
{
    [HarmonyPatch(typeof (ZNet), "OnNewConnection")]
    public class AddPlayer_Patch
    {
        public static void Postfix(ZNet __instance, ZNetPeer peer)
        {
            if (ValheimPlusPlugin.posMap.ContainsKey(peer))
                return;
            ValheimPlusPlugin.posMap.Add(peer, Vector3.zero);
        }
    }
}