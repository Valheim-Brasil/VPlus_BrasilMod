using HarmonyLib;
using System;

namespace ValheimPlus.AntiAnyMods
{
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    [HarmonyPatch(typeof (ZRoutedRpc), "GetServerPeerID")]
    public static class HookedZRoutedRpc
    {
      public static long GetServerPeerID(object instance) => throw new NotImplementedException();
    }
}