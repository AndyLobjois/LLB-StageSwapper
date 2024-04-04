using HarmonyLib;
using StageBackground;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace LLB_StageSwap
{
    [HarmonyPatch(typeof(BG), nameof(BG.StartUp))]
    //[HarmonyPatch(typeof(OGONAGCFDPK), nameof(OGONAGCFDPK.DLIEBHKPBGP))]
    public class StartPatch
    {
        public static void Postfix()
        {
            Plugin.Log.LogInfo(QualitySettings.shadowDistance);
            Plugin.Log.LogInfo(BG.instance.fixedCamPosY);
            // outskirts 1.76
            // sewers 1.68
            // junktown 1.95
            // construction 1.89
            // factory 1.78
            // subway 1.77
            // stadium 1.89
            // streets 1.84
            // pool 2.02
            // room 2.01
            // retro 0
            StageSwapper swap = BG.instance.gameObject.GetComponent<StageSwapper>();
            if (swap != null)
            {
                BG.DestroyImmediate(swap);
            }
            BG.instance.gameObject.AddComponent<StageSwapper>();
        }
    }
    
    [HarmonyPatch(typeof(BG), nameof(BG.StartMatchIntro))]
    public class IntroCamPatch
    {
        public static void Prefix()
        {
            Plugin.Log.LogInfo("Start match intro called!");
            // TODO: some duplicate with StageSwapper, should fix
            string stageName = JOMBNFKIHIC.GIGAKBJGFDI.OOEPDFABFIP.ToString().ToLower();
            String configPath = Path.Combine(Plugin.PluginDir.ToString(), stageName + ".xml");
            StageSettings settings = Plugin.LoadXML(configPath);
            if (settings == null) return;
                /*
                 * load intro shots
                 */
                List<BG.IntroCamShot> shotList = new List<BG.IntroCamShot>();
                foreach (CamShot shot in settings.shots)
                {
                    BG.IntroCamShot ishot = new BG.IntroCamShot();
                    ishot.StartPoint = Plugin.FromCamTransform(shot.StartCam);
                    ishot.EndPoint = Plugin.FromCamTransform(shot.EndCam);
                    ishot.Duration = shot.Duration;
                    ishot.isConstantSpeed = !shot.Smooth;
                    shotList.Add(ishot);
                }

            BG.instance.introCamShots = shotList.ToArray<BG.IntroCamShot>().ToArray();
        }
    }
}
