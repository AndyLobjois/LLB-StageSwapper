using StageBackground;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace LLB_StageSwapper
{
    public class StageSwapper : MonoBehaviour
    {
        public AssetBundle sceneBundle;
        public string[] normalNames;
        public string[] eclipseNames;
        public CamShot[] camShots;
        public bool active = false;

        void Start()
        {
            if (DNPFJHMAIBP.HHMOGKIMBNM() == JOFJHDJHJGI.AOEDPBCIAEP) return;
            if (SceneManager.GetActiveScene().name == "title") return;
            
            Plugin.Log.LogInfo(SceneManager.GetActiveScene().name);
            string stageName = JOMBNFKIHIC.GIGAKBJGFDI.OOEPDFABFIP.ToString();
            sceneBundle = Plugin.GetBundle(stageName);
            if (sceneBundle != null)
            {
                active = true;

                // try load settings
                String configPath = Path.Combine(Plugin.PluginDir.ToString(), stageName + ".xml");
                StageSettings settings = Plugin.LoadXML(configPath);
                if (settings != null)
                {
                    QualitySettings.shadowDistance = settings.ShadowDistance;
                    BG.instance.fixedCamPosY = settings.CameraPositionY;
                    BG.instance.maxPosY = BG.instance.fixedCamPosY + 0.5f;

                    normalNames = settings.normal;
                    eclipseNames = settings.eclipse;
                    camShots = settings.shots;
                }
                else
                {
                    normalNames = new string[] { "StageSwapperNormal" };
                    eclipseNames = new string[] { "StageSwapperEclipse" };
                }

                StartCoroutine(ReplaceStage());
            }
            else
            {
                QualitySettings.shadowDistance = 40; // reset shadow distance for non-modded stages
            }
        }

        private IEnumerator ReplaceStage()
        {
            //yield return new WaitForSeconds(0.1f);
            yield return new WaitForEndOfFrame();

            /* nuke the old stage, taken from basicstages */
            BG.instance.obsEclipse = null;
            BG.instance.obsNormal = null;
            BG.instance.eclipseMaterials = null;
            BG.instance.replaceMaterials = null;
            BG.instance.eclipseColorRenderers = null;
            BG.instance.introCamShots = null;
            BG.instance.eclipseEvents.RemoveAllListeners();

            ClearStage(GameObject.Find("Background"));
            ClearStage(GameObject.Find("Eclipse"));
            ClearStage(GameObject.Find("BGLayer2_Animated"));
            ClearStage(GameObject.Find("PedestriansLayer"));
            ClearStage(GameObject.Find("Blimp_Animated 1"));
            ClearStage(GameObject.Find("Blimp_Animated 2"));
            ClearStage(GameObject.Find("Blimp_Animated 3"));

            // Change receiveShadowsPlane to a custom one
            StartCoroutine("ChangeShadowsPlane");

            Plugin.Log.LogInfo("Cleared stage objects");


            string[] scenePath = sceneBundle.GetAllScenePaths();
            SceneManager.LoadScene(scenePath[0], LoadSceneMode.Additive);

            StartCoroutine(SetupScene());
        }

        private IEnumerator SetupScene()
        {
            yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();

            /*
             * handle non-eclipse / eclipse objects
             */
            List<GameObject> normal = new List<GameObject>();
            List<GameObject> eclipse = new List<GameObject>();
            foreach (string objName in normalNames)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj == null)
                {
                    Plugin.Log.LogWarning("Could not find obj with name '" + objName + "' for normal objects");
                }
                else
                {
                    normal.Add(obj);
                }
            }
            foreach (string objName in eclipseNames)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj == null)
                {
                    Plugin.Log.LogWarning("Could not find obj with name '" + objName + "' for eclipse objects");
                }
                else
                {
                    eclipse.Add(obj);
                }
            }


            BG.instance.obsNormal = normal.ToArray<GameObject>();
            BG.instance.obsEclipse = eclipse.ToArray<GameObject>();
            Plugin.Log.LogInfo(BG.instance.obsNormal.Length.ToString() + " elements in normal");
            Plugin.Log.LogInfo(BG.instance.obsEclipse.Length.ToString() + " elements in eclipse");
            foreach (GameObject ele in BG.instance.obsEclipse)
            {
                ele.SetActive(false);
            }

            /*
             * turn off shadows
             */
            GameObject light = GameObject.Find("shadowCastLight");
            light.GetComponent<Light>().shadows = LightShadows.None;

            foreach (GameObject obj in BG.instance.obsEclipse)
            {
                obj.SetActive(false);
            }
            

            /*
             *  start intro shots, since it is cancelled by bgpatch
             */
            //Plugin.Log.LogInfo("cam shots length: " + BG.instance.introCamShots.Length.ToString());
           // BG.instance.StartKoroutine(BG.instance.KStageIntro(), out BG.kStageIntro);
        }
        /*
         * Method is taken from Daioutzu's Basic stages
         */
        static void ClearStage(GameObject gameObject)
        {
            if (gameObject != null)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    bool flag = gameObject.transform.GetChild(i).name.Contains("Positions");
                    bool flag2 = gameObject.transform.GetChild(i).name.Contains("Light");
                    if (flag == false && flag2 == false)
                    {
                        gameObject.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }                          

        IEnumerator ChangeShadowsPlane() {
            yield return new WaitForSeconds(0.1f); // Dirty hack I know

            if (GameObject.Find("CustomReceiveShadowsPlane")) {
                GameObject originalPlane = GameObject.Find("receiveShadowsPlane");
                GameObject customPlane = GameObject.Find("CustomReceiveShadowsPlane");

                originalPlane.GetComponent<MeshFilter>().mesh = customPlane.GetComponent<MeshFilter>().mesh;
                originalPlane.transform.position = customPlane.transform.position;
                originalPlane.transform.rotation = customPlane.transform.rotation;
                originalPlane.transform.localScale = customPlane.transform.localScale;
                customPlane.SetActive(false);
            } else {
                Plugin.Log.LogMessage("There is no 'CustomReceiveShadowsPlane' gameObject in the scene, if it's intentional, ignore this message.\n" +
                    "If you want to create a custom ReceiveShadowsPlane, please create a gameObject with the exact name 'CustomReceiveShadowsPlane'.\n" +
                    "Add a MeshFilter Component on this gameObject and assign your mesh.\n" +
                    "Make sure to let this gameObject active before building your stage bundle.");
            }
        }
    }
}
