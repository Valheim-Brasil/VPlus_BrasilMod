using BepInEx;

namespace ValheimPlus.AntiAnyMods
{
    public static class ZNet_RPC_PeerInfoPatch
    {
        private static bool Prefix(ref ZNet __instance, ZRpc rpc, ZPackage pkg)
        {
            if (__instance.IsServer())
            {
                string self = "";
                if (pkg.Size() > 32)
                {
                    pkg.SetPos(pkg.Size() - 32 - 1);
                    if (pkg.ReadByte() == (byte) 32)
                    {
                        pkg.SetPos(pkg.GetPos() - 1);
                        self = pkg.ReadString();
                    }
                }
                ZLog.Log((object) ("[AntiCheat]: Got client hash: " + self + "\nmine: " + ValheimPlusPlugin.PluginsHash));
                pkg.SetPos(0);
                if (!self.Equals(ValheimPlusPlugin.PluginsHash))
                {
                    int num = self.IsNullOrWhiteSpace() ? 3 : 99;
                    ZLog.Log((object) ("[AntiCheat]: Expulsando cliente " + rpc.GetSocket().GetEndPointString() + " por mods incompatíveis"));
                    rpc.Invoke("Error", (object) num);
                    return false;
                }
                ZLog.Log((object) ("[AntiCheat]: Aceitando o cliente " + rpc.GetSocket().GetEndPointString()));
            }
            return true;
        }

        private static void Postfix()
        {
            if (ZNet.instance.IsServer())
                return;
            ZLog.Log((object) string.Format("[{0}]: Send AcRoutedHandshake to {1} from client", (object) "AntiCheat", (object) HookedZRoutedRpc.GetServerPeerID(ZRoutedRpc.instance)));
            ZRoutedRpc.instance.InvokeRoutedRPC(HookedZRoutedRpc.GetServerPeerID(ZRoutedRpc.instance), "AcRoutedHandshake", (object) new ZPackage());
        }
    }
}