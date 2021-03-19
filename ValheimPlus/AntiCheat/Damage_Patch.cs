using System.Net.PeerToPeer.Collaboration;
using HarmonyLib;

namespace ValheimPlus.AntiCheat
{
    public class Damage_Regra_Patch
    {
        public static bool Execute(HitData hit)
        {
            if (!ValheimPlusPlugin.anti_debug_mode && !ValheimPlusPlugin.anti_damage_boost)
                return true;

            Character senderChar = hit.GetAttacker();
            
            if(senderChar != null)
                ZLog.LogError("Send Char" + senderChar);

            if (senderChar != null && senderChar.IsPlayer())
            {
                ZNetPeer peer = ZNet.instance.GetPeer(senderChar.GetInstanceID());
                //ZLog.LogError("Jogador Detectado.");
                //ZLog.LogError("Dano = " + hit.GetTotalDamage());

                float damage = hit.GetTotalDamage();
                if( peer != null && ( !ValheimPlusPlugin.admins_bypass || !ZNet.instance.m_adminList.Contains(peer.m_rpc.GetSocket().GetHostName()) ) && damage > 1000f)
                {
                    ZLog.LogError("Jogador Detectado com Damage Boost.");
                    ValheimPlusPlugin.toKick.Add(peer);
                }    
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof (Character), "RPC_Damage")]
    public class DamageCharacter_Patch
    {
        private static bool Prefix(ref Character __instance, ref long sender, ref HitData hit)
        {
            ZLog.LogWarning("Damage to Character");
            return Damage_Regra_Patch.Execute(hit);
        }
    }
    
    [HarmonyPatch(typeof (WearNTear), "RPC_Damage")]
    public class DamageWearNTear_Patch
    {
        private static bool Prefix(ref WearNTear __instance,ref long sender, ref HitData hit)
        {
            ZLog.LogWarning("Damage to WearNTear");
            return Damage_Regra_Patch.Execute(hit);
        }
    }
    
    [HarmonyPatch(typeof (TreeBase), "RPC_Damage")]
    public class DamageTreeBase_Patch
    {
        private static bool Prefix(ref TreeBase __instance,ref long sender, ref HitData hit)
        {
            ZLog.LogWarning("Damage to TreeBase");
            return Damage_Regra_Patch.Execute(hit);
        }
    }
    
    [HarmonyPatch(typeof (TreeLog), "RPC_Damage")]
    public class DamageTreeLog_Patch
    {
        private static bool Prefix(ref TreeLog __instance,ref long sender, ref HitData hit)
        {
            ZLog.LogWarning("Damage to TreeBase");
            return Damage_Regra_Patch.Execute(hit);
        }
    }
        
    [HarmonyPatch(typeof (MineRock5), "RPC_Damage")]
    public class DamageMineRock5_Patch
    {
        private static bool Prefix(ref MineRock5 __instance,ref long sender, ref HitData hit)
        {
            ZLog.LogWarning("Damage to MineRock5");
            return Damage_Regra_Patch.Execute(hit);
        }
    }
    
    [HarmonyPatch(typeof (Destructible), "RPC_Damage")]
    public class DamageDestructible_Patch
    {
        private static bool Prefix(ref Destructible __instance,ref long sender, ref HitData hit)
        {
            ZLog.LogWarning("Damage to MineRock5");
            return Damage_Regra_Patch.Execute(hit);
        }
    }
}