using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using ValheimPlus.Configurations;

namespace ValheimPlus.AntiCheat
{
  [HarmonyPatch(typeof (ZNet), "UpdatePlayerList")]
  public class UpdatePlayerList_Patch
  {
    private static void Postfix()
    {
      if ( !( (UnityEngine.Object) ZNet.instance != (UnityEngine.Object) null ) )
        return;
      if (ZNet.instance.IsServer() && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null && (Player.m_players != null && Player.m_players.Count > 0))
      {
        foreach (Player player in Player.m_players)
        {
          ZNetPeer peerByPlayerName = ZNet.instance.GetPeerByPlayerName(player.name);
          if ((player.m_debugFly || player.m_noPlacementCost && ValheimPlusPlugin.anti_debug_mode) && (peerByPlayerName != null && !ZNet.instance.m_adminList.Contains(peerByPlayerName.m_rpc.GetSocket().GetHostName())))
            ValheimPlusPlugin.toKick.Add(peerByPlayerName);
          if (player.m_godMode && ValheimPlusPlugin.anti_god_mode && (peerByPlayerName != null && !ZNet.instance.m_adminList.Contains(peerByPlayerName.m_rpc.GetSocket().GetHostName())))
            ValheimPlusPlugin.toKick.Add(peerByPlayerName);
        }
      }
      
      if (ZNet.instance.m_peers.Count > 0)
      {
        foreach (ZNetPeer peer in ZNet.instance.m_peers)
        {
          if (ValheimPlusPlugin.posMap[peer] == Vector3.zero)
            ValheimPlusPlugin.posMap[peer] = peer.m_refPos;
          else if (ValheimPlusPlugin.anti_fly)
          {
            if ((double) Math.Abs(peer.m_refPos.x - ValheimPlusPlugin.posMap[peer].x) > 70.0 || (double) Math.Abs(peer.m_refPos.y - ValheimPlusPlugin.posMap[peer].y) > 35.0 || (double) Math.Abs(peer.m_refPos.y - ValheimPlusPlugin.posMap[peer].y) > 15.0)
              ValheimPlusPlugin.toKick.Add(peer);
            else
              ValheimPlusPlugin.posMap[peer] = peer.m_refPos;
          }
          if (peer.IsReady() && !peer.m_characterID.IsNone() && (ZNet.instance.m_zdoMan.GetZDO(peer.m_characterID).GetBool("DebugFly") && !ZNet.instance.m_adminList.Contains(peer.m_rpc.GetSocket().GetHostName())))
            ValheimPlusPlugin.toKick.Add(peer);
        }
      }
      
      if (ValheimPlusPlugin.toKick.Count <= 0)
        return;
      foreach (ZNetPeer znetPeer in ValheimPlusPlugin.toKick)
      {
        if (!ValheimPlusPlugin.admins_bypass || !ZNet.instance.m_adminList.Contains(znetPeer.m_rpc.GetSocket().GetHostName()))
        {
          if (ValheimPlusPlugin.ban_on_trigger)
          {
            ZNet.instance.Ban(znetPeer.m_playerName);
            ZLog.LogError("Jogador: " + znetPeer.m_playerName + znetPeer.m_uid + znetPeer.m_characterID + " Banido por uso de Cheats.");
          }
          else
          {
            ZNet.instance.Kick(znetPeer.m_playerName);
            ZLog.LogError("Jogador" + znetPeer.m_playerName + znetPeer.m_uid + znetPeer.m_characterID + " Kickado por uso de Cheats.");
          }
        }
      }
      ValheimPlusPlugin.toKick.Clear();
    }
  }
}