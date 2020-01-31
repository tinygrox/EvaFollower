using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using EvaFuel;

namespace MSD.EvaFollower
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class EVAFuelGlobals : MonoBehaviour
    {
        public static bool changeEVAPropellent;
        void Start()
        {
            changeEVAPropellent = false;
            if (!modsFound)
            {
                buildModList();
                modsFound = true;
            }
        }

        bool modsFound = false;
        static List<String> installedMods = new List<String>();
        void buildModList()
        {
            //https://github.com/Xaiier/Kreeper/blob/master/Kreeper/Kreeper.cs#L92-L94 <- Thanks Xaiier!
            foreach (AssemblyLoader.LoadedAssembly a in AssemblyLoader.loadedAssemblies)
            {
                string name = a.name;
                //Log.Info(string.Format("Loading assembly: {0}", name));
                installedMods.Add(name);
            }
        }
        static public bool hasMod(string modIdent)
        {
            return installedMods.Contains(modIdent);
        }

    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    class SelectEVAFuelType : MonoBehaviour
    {
        public static SelectEVAFuelType Instance;

        public enum Answer
        {
            inActive,
            notAnswered,
            cancel,
            answered
        };
        public Answer answer = Answer.inActive;
        public float lastTimeTic = 0;

        private Rect settingsRect;
        int curResIndex = -1;
        Vector2 scrollPosition1 = Vector2.zero;
        static List<PartResourceDefinition> allResources = null;
        static List<string> allResourcesDisplayNames = null;
        static List<string> fuelResources = null;
        static List<string> bannedResources = null;

        public string selectedFuel;


        GUIStyle smallButtonStyle, smallScrollBar;

        public static String ROOT_PATH;
        public static string MOD = null;
        static string EVA_FUELRESOURCES = "FUELRESOURCES";
        static string BANNED_RESOURCES = "BANNED";
        public static String EVAFUEL_NODE = MOD;


        void Start()
        {
            Log.Info("SelectEVAFuelType.Start");
            Instance = this;
            smallButtonStyle = new GUIStyle(HighLogic.Skin.button);
            smallButtonStyle.stretchHeight = false;
            smallButtonStyle.fixedHeight = 20f;

            smallScrollBar = new GUIStyle(HighLogic.Skin.verticalScrollbar);
            smallScrollBar.fixedWidth = 8f;

            // The follopwing
            if (EVAFuelGlobals.hasMod("EvaFuel"))
                MOD = GetEvaFuel();
            else
                MOD = null;
            settingsRect = new Rect(200, 200, 275, 400);
            ROOT_PATH = KSPUtil.ApplicationRootPath;

        }

        string GetEvaFuel()
        {
            return  Assembly.GetAssembly(typeof(EvaFuelManager)).GetName().Name;
        }
        void OnGUI()
        {
            if (answer == Answer.inActive)
                return;
            if (Time.realtimeSinceStartup - lastTimeTic > 0.25)
            {
                answer = Answer.inActive;
                return;
            }
#if false
            DifficultyOptionsMenu dom = (DifficultyOptionsMenu)FindObjectOfType(typeof(DifficultyOptionsMenu));
            List<GameObject> GameObjects = new List<GameObject>(FindObjectsOfType<GameObject>());
            foreach (var go in GameObjects)
                if (go.name == "GameDifficulty dialog handler")
                {
                    //   go.SetActive(false);
                    var o = go.GetComponents<DialogGUIToggleButton>();
                    foreach (var o1 in o)
                        Log.Info("o1: " + o1.label);
                    
                }
            
            if (dom)
            {
                Log.Info("DifficultyOptionsMenu found");
                dom.enabled = false;


            }
#endif
            Draw();
        }

        string setLabel()
        {
            return "xxx";
        }
        public void Draw()
        {
            if (MOD == null || MOD == "")
                return;

            Log.Info("SelectEVAFuelType.Draw");
            if (allResources == null)
                getAllResources();

            // The settings are only available in the space center
            GUI.skin = HighLogic.Skin;
            settingsRect = GUILayout.Window("EVAFuelSettings".GetHashCode(),
                                            settingsRect,
                                            SettingsWindowFcn,
                                            "EVA Follower Settings",
                                            GUILayout.ExpandWidth(true),
                                            GUILayout.ExpandHeight(true));
        }


        public List<String> getFuelResources(bool banned = false)
        {
            List<string> fr = new List<String>();
            if (MOD == null || MOD == "")
                return fr;
            ConfigNode configFile = new ConfigNode();
            ConfigNode configFileNode = new ConfigNode();
            ConfigNode configDataNode;
            
            string fname = ROOT_PATH + "GameData/" + MOD + "/PluginData/fuelResources.cfg";

            configFile = ConfigNode.Load(fname);
            if (configFile != null)
            {
                configFileNode = configFile.GetNode(EVAFUEL_NODE);

                if (configFileNode != null)
                {
                    if (banned)
                        configDataNode = configFileNode.GetNode(BANNED_RESOURCES);
                    else
                        configDataNode = configFileNode.GetNode(EVA_FUELRESOURCES);
                    if (configDataNode != null)
                        fr = configDataNode.GetValuesList("resource");
                }
                else
                    Log.Error("NODENAME not found: " + EVAFUEL_NODE);
            }
            else
                Log.Error("File not found: " + fname);

            return fr;
        }


        void fillResourceDisplayNames()
        {
            if (fuelResources == null || allResources == null)
                getAllResources();
            allResourcesDisplayNames = new List<string>();
            int cnt = 0;
            if (fuelRes && fuelResources.Count > 0)
            {

                foreach (var s in fuelResources)
                {
                    try
                    {
                        var ar = allResources.Find(o => o.name == s);
                        if (ar.displayName != null)
                            allResourcesDisplayNames.Add(ar.displayName);
                        else
                            allResourcesDisplayNames.Add(ar.name);
                        if (ar.name == HighLogic.CurrentGame.Parameters.CustomParams<EVAFuelSettings>().ShipPropellantName)
                            curResIndex = cnt;
                        cnt++;

                    }
                    catch
                    {
                        Log.Error("Can't find resource: " + s + " in allResources");
                    }
                }
            }
            else
            {
                foreach (var ar in allResources)
                {
                    if (bannedResources.Contains(ar.name))
                        continue;
                    if (ar.displayName != null)
                        allResourcesDisplayNames.Add(ar.displayName);
                    else
                        allResourcesDisplayNames.Add(ar.name);
                    if (ar.name == HighLogic.CurrentGame.Parameters.CustomParams<EVAFuelSettings>().ShipPropellantName)
                        curResIndex = cnt;
                    cnt++;
                }

            }
        }

        void getAllResources()
        {
            allResources = new List<PartResourceDefinition>();

            foreach (PartResourceDefinition rs in PartResourceLibrary.Instance.resourceDefinitions)
            {
                allResources.Add(rs);
            }
            allResources = allResources.OrderBy(o => o.displayName).ToList();
            fuelResources = getFuelResources();
            bannedResources = getFuelResources(true);

            fillResourceDisplayNames();

        }
        bool allRes = false;
        bool fuelRes = true;
        void SettingsWindowFcn(int windowID)
        {

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select EVA Propellent from list below");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            var newallRes = GUILayout.Toggle(allRes, "All resources");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            var newfuelRes = GUILayout.Toggle(fuelRes, "Fuel resources");
            GUILayout.EndVertical();
            if (newfuelRes && allRes)
            {
                allRes = false;
                fuelRes = true;
                fillResourceDisplayNames();
            }
            else
                if (newallRes & fuelRes)
            {
                allRes = true;
                fuelRes = false;
                fillResourceDisplayNames();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1);

            curResIndex = GUILayout.SelectionGrid(curResIndex, allResourcesDisplayNames.ToArray(), 1, smallButtonStyle);

            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                answer = Answer.answered;
                if (allRes)
                {
                    selectedFuel = allResources[curResIndex].name;
                }
                else
                {
                    int cnt = 0;
                    for (int i = 0; i < fuelResources.Count; i++)
                    {
                        try
                        {
                            var ar = allResources.Find(o => fuelResources[i] == o.name);

                            if (cnt == curResIndex)
                            {
                                selectedFuel = ar.name;
                                break;
                            }
                            cnt++;
                        }
                        catch
                        { }
                    }
                }
            }
            if (GUILayout.Button("Cancel"))
                answer = Answer.cancel;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            // This call allows the user to drag the window around the screen
            GUI.DragWindow();
        }
    }



    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    // HighLogic.CurrentGame.Parameters.CustomParams<EVAFuelSettings>()

    public class EvaFollowerMiscSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "EVA Follower"; } }
        public override string DisplaySection { get { return "EVA Follower"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Show Debug Lines?")]
        public bool displayDebugLinesSetting = false;

        [GameParameters.CustomParameterUI("Show Loading Kerbals?")]
        public bool displayLoadingKerbals = true;

        [GameParameters.CustomParameterUI("Enable Helmet Toggle?")]
        public bool displayToggleHelmet = false;

        [GameParameters.CustomParameterUI("Target Vessel By Selection?")]
        public bool targetVesselBySelection = true;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return !EVAFuelGlobals.changeEVAPropellent;
        }
    }
}

