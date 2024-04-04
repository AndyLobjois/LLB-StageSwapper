using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LLHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace LLB_StageSwap
{
    [BepInPlugin(PluginInfos.PLUGIN_ID, PluginInfos.PLUGIN_NAME, PluginInfos.PLUGIN_VERSION)]
    [BepInDependency(LLBML.PluginInfos.PLUGIN_ID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("no.mrgentle.plugins.llb.modmenu", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static DirectoryInfo PluginDir => LLBML.Utils.ModdingFolder.GetModSubFolder(instance.Info);
        internal static ManualLogSource Log { get; private set; }

        private static IDictionary<string, AssetBundle> StageBundles = new Dictionary<string, AssetBundle>();
        void Awake()
        {
            instance = this;
            Log = this.Logger;

            var harmony = new Harmony(PluginInfos.PLUGIN_NAME);
            harmony.PatchAll();

            Log.LogInfo(PluginDir);
        }

        void Start()
        {
            StartCoroutine(LoadSwaps());
        }

        IEnumerator LoadSwaps()
        {
            yield return new WaitForSeconds(4f);

            if (StageHandler.stagesAll == null)
            {
                Plugin.Log.LogError("Stage handler wasn't loaded yet - please report this bug");
            }
            foreach (Stage stage in StageHandler.stagesAll)
            {
                String stagePath = Path.Combine(PluginDir.ToString(), stage.ToString().ToLower());
                
                if (File.Exists(stagePath))
                {
                    Plugin.Log.LogInfo("Loading assetbundle: " + stagePath);
                    AssetBundle bund = AssetBundle.LoadFromFile(stagePath);
                    Plugin.StageBundles[stage.ToString()] = bund;
                }

            }
        }
        /*
         * Ensures the same assetbundle isn't loaded twice.
         */
        public static AssetBundle GetBundle(String name)
        {
            if (StageBundles.ContainsKey(name))
            {
                return StageBundles[name];
            }
            return null;
        }

        /*
         * Check if assetbundle of name has been loaded
         */
        public static bool HasSwap(String name)
        {
            return StageBundles.ContainsKey(name);
        }

        /*
         * Load stage settings from xml file 
         */
        public static StageSettings LoadXML(String path)
        {
            if (File.Exists(path))
            {
                XmlSerializer des = new XmlSerializer(typeof(StageSettings));
                using (XmlReader reader = XmlReader.Create(path))
                {
                    return (StageSettings)des.Deserialize(reader);
                }
            }
            return null;
        }

        /*
         * Convert the stage settings camera transform to a unity one
         */
        public static Transform FromCamTransform(CamTransform cam)
        {
            Log.LogInfo(cam.ToString());
            Transform trans = new GameObject().transform;
            trans.localPosition = new Vector3(cam.x, cam.y, cam.z);
            trans.localEulerAngles = new Vector3(cam.rotX, cam.rotY, cam.rotZ);
            return trans;
        }
    }
}
