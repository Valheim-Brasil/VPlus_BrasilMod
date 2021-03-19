using BepInEx;
using HarmonyLib;
using ValheimPlus.Configurations;
using ValheimPlus.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using BepInEx.Logging;
using UnityEngine;

namespace ValheimPlus
{
    // COPYRIGHT 2021 KEVIN "nx#8830" J. // http://n-x.xyz
    // GITHUB REPOSITORY https://github.com/valheimPlus/ValheimPlus
    

    [BepInPlugin("org.bepinex.plugins.valheim_plus", "Valheim Plus", version)]
    public class ValheimPlusPlugin : BaseUnityPlugin
    {
            //VARIÁVEIS COMUNS
        
        public const string version = "0.1.8";
        public static string newestVersion = "";
        public static bool isUpToDate = false;
        Harmony harmony = new Harmony("mod.valheim_plus");
        public const int HashLength = 32;
        public static string PluginsHash = "huehue";
        public static ManualLogSource logger;
        public static List<ZNetPeer> toKick = new List<ZNetPeer>();
        public static Dictionary<ZNetPeer, Vector3> posMap;
        public static float starvingsys_damage = 1f;
        
        //public static string ServerVaultPath;
        //public static string ServerSafeZonePath;
        //public static int ServerSaveInterval;

        // Project Repository Info
        public static string Repository = "https://github.com/Valheim-Brasil/VPlus-Brasil";
        public static string ApiRepository = "https://api.github.com/repos/Valheim-Brasil/VPlus-Brasil/tags";

        
            // ========================= VALHEIM ANTI-MODS
        
        /*
        public static void LogZLog(object o, BepInEx.Logging.LogLevel level = BepInEx.Logging.LogLevel.Info)
        {
            string str = string.Format("[{0}]: {1}", (object) nameof (ValheimPlus), o);
            switch (level)
            {
                case BepInEx.Logging.LogLevel.Error:
                    ZLog.LogError((object) str);
                    break;
                case BepInEx.Logging.LogLevel.Warning:
                    ZLog.LogWarning((object) str);
                    break;
                case BepInEx.Logging.LogLevel.Debug:
                    ZLog.DevLog((object) str);
                    break;
                default:
                    ZLog.Log((object) str);
                    break;
            }
        } */

            // ========================= VALHEIM ANTI-CHEAT
        
        public static bool ban_on_trigger = true;
        public static bool admins_bypass = true;
        public static bool anti_fly = false;
        public static bool anti_debug_mode = true;
        public static bool anti_god_mode = true;
        public static bool anti_damage_boost = true;
        public static bool anti_health_boost = true;
        
            // ========================= FPS Booost
            
        internal readonly ManualLogSource log;
        internal readonly Assembly assembly;
        public readonly string modFolder;

        public ValheimPlusPlugin()
        {
            log = this.Logger;
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(this.assembly.Location);
        }
        
        private void InternalGraphicSettings()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.lodBias = 0.3f;
            QualitySettings.masterTextureLimit = 1;
            QualitySettings.maximumLODLevel = 0;
            QualitySettings.maxQueuedFrames = 1;
            QualitySettings.particleRaycastBudget = 1;
            QualitySettings.pixelLightCount = 0;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0.0f;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.skinWeights = SkinWeights.OneBone;
            QualitySettings.softParticles = false;
            QualitySettings.softVegetation = false;
        }
        
        private void Update()
        {
            // STARVING SYSTEM
            if (!APIs.GLOBAL.starvationSystem.checkUpdate())
                return;
            APIs.GLOBAL.starvationSystem.update();
        }

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            // ========================= FPS BOOST
            InternalGraphicSettings();
            
            // ========================= ANTI-CHEAT
            
            logger = this.Logger;
            posMap = new Dictionary<ZNetPeer, Vector3>();
            toKick = new List<ZNetPeer>();

            // ========================= ANTI-CHARS

            /*ServerVaultPath = Path.Combine(Utils.GetSaveDataPath(), "characters_vault");
            ServerSafeZonePath = Path.Combine(Utils.GetSaveDataPath(), "safe_zones.txt");
            ZLog.LogError(ServerSafeZonePath);
            ZLog.LogError(ServerVaultPath);
            ServerSaveInterval = 600;
            if (!File.Exists(ServerSafeZonePath))
            {
                ZLog.Log(string.Format("Criação de arquivo de zona segura em {0}", (object) ServerSafeZonePath));
                string contents = "# format: name x z radius\nDefaultSpawnSafeZone 0.0 0.0 50.0";
                File.WriteAllText(ServerSafeZonePath, contents);
            }
            foreach (string readAllLine in File.ReadAllLines(ServerSafeZonePath))
            {
                if (!string.IsNullOrWhiteSpace(readAllLine) && readAllLine[0] != '#')
                {
                  string[] strArray = readAllLine.Split();
                  if (strArray.Length != 4)
                  {
                    ZLog.Log(string.Format("Safe zone {0} não está formatado corretamente.", (object) readAllLine));
                  }
                  else
                  {
                    AntiChars.ServerState.SafeZone safeZone;
                    safeZone.name = strArray[0];
                    safeZone.position.x = float.Parse(strArray[1]);
                    safeZone.position.y = float.Parse(strArray[2]);
                    safeZone.radius = float.Parse(strArray[3]);
                    ZLog.Log(string.Format("Safezone carregada {0} ({1}, {2}) radius {3}", (object) safeZone.name, (object) safeZone.position.x, (object) safeZone.position.y, (object) safeZone.radius));
                    AntiChars.ServerState.SafeZones.Add(safeZone);
                  }
                }
            }*/
            
            // ========================= Anti Mods
            
            PluginsHash = AntiAnyMods.HashAlgorithmExtensions.CreateMd5ForFolder(Paths.PluginPath);
              ZLog.Log((object) ("[AntiAnyMods]: Computed hash: " + PluginsHash));
              this.harmony.PatchAll();
              if (Paths.ProcessName.Equals("valheim_server", StringComparison.OrdinalIgnoreCase))
              {
                MethodInfo methodInfo = AccessTools.Method(typeof (ZNet), "RPC_PeerInfo", new System.Type[2]
                {
                  typeof (ZRpc),
                  typeof (ZPackage)
                });
                if ((object) methodInfo == null)
                {
                  ZLog.LogError((object) "[AntiAnyMods] Não foi possível encontrar ZNet:RPC_PeerInfo");
                  return;
                }
                this.harmony.Patch((MethodBase) methodInfo, new HarmonyMethod(AccessTools.Method(typeof (AntiAnyMods.ZNet_RPC_PeerInfoPatch), "Prefix", new System.Type[3]
                {
                  typeof (ZNet).MakeByRefType(),
                  typeof (ZRpc),
                  typeof (ZPackage)
                })));
                ZLog.Log((object) "[AntiAnyMods] Patched server!");
              }
              else
              {
                MethodInfo methodInfo1 = AccessTools.Method(typeof (ZNet), "RPC_PeerInfo", new System.Type[2]
                {
                  typeof (ZRpc),
                  typeof (ZPackage)
                });
                if ((object) methodInfo1 == null)
                {
                  ZLog.LogError((object) "[AntiAnyMods] Não foi possível encontrar ZNet:RPC_PeerInfo");
                  return;
                }
                MethodInfo methodInfo2 = AccessTools.Method(typeof (ZNet), "SendPeerInfo", new System.Type[2]
                {
                  typeof (ZRpc),
                  typeof (string)
                });
                if ((object) methodInfo2 == null)
                {
                  ZLog.LogError((object) "[AntiAnyMods] Não foi possível encontrar ZNet:SendPeerInfo");
                  return;
                }
                this.harmony.Patch((MethodBase) methodInfo2, transpiler: new HarmonyMethod(AccessTools.Method(typeof (AntiAnyMods.ILPatches), "SendPeerInfo_Transpile", new System.Type[1]
                {
                  typeof (IEnumerable<CodeInstruction>)
                })));
                this.harmony.Patch((MethodBase) methodInfo1, postfix: new HarmonyMethod(AccessTools.Method(typeof (AntiAnyMods.ZNet_RPC_PeerInfoPatch), "Postfix")));
                ZLog.Log((object) "[AntiAnyMods] Patched client!");
              }
              this.Logger.LogInfo((object) "[AntiAnyMods] v0.1.0 loaded.");
            
              // ========================= Origem Valheim Plus
            
            Logger.LogInfo("Tentando carregar o arquivo de configuração");

            if (ConfigurationExtra.LoadSettings() != true)
            {
                Logger.LogError("Erro ao carregar o arquivo de configuração.");
            }
            else
            {
                Logger.LogInfo("Arquivo de configuração carregado com sucesso.");

                
                harmony.PatchAll();

                isUpToDate = !Settings.isNewVersionAvailable();
                if (!isUpToDate)
                {
                    Logger.LogError("Há uma versão mais recente disponível do ValheimPlus (Brasil Mod)");
                    Logger.LogWarning("Por favor, visite " + ValheimPlusPlugin.Repository + ".");
                }
                else
                {
                    Logger.LogInfo("ValheimPlus (Brasil Mod) [" + version + "] está atualizado.");
                }

                //Logo
                VPlusMainMenu.Load();
            }
        }
    }
}
