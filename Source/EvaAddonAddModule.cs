using UnityEngine;

namespace MSD.EvaFollower
{
    /// <summary>
    /// Add the module to all kerbals available. 
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class EvaAddonAddModule : MonoBehaviour
    {
        public void Awake()
        {
            EvaDebug.DebugLog("Loaded AddonAddModule.");

            ConfigNode EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");

            try
            {
                PartLoader.getPartInfoByName("kerbalEVA").partPrefab.AddModule(EVA);
            }
            catch { }

            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("kerbalEVAVintage").partPrefab.AddModule(EVA);
            }
            catch { }

            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");

            try
            {
                PartLoader.getPartInfoByName("kerbalEVAfemale").partPrefab.AddModule(EVA);
            }
            catch { }
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("kerbalEVAfemaleVintage").partPrefab.AddModule(EVA);
            }
            catch { }

            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("kerbalEVA_RD_Exp").partPrefab.AddModule(EVA);
            }
            catch { }
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("kerbalEVA_female_Exp").partPrefab.AddModule(EVA);
            }
            catch { }
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("kerbalEVA_RD_Future").partPrefab.AddModule(EVA);
            }
            catch { }
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("kerbalEVA_female_Future").partPrefab.AddModule(EVA);
            }
            catch { }
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("maleEVA").partPrefab.AddModule(EVA);
            }
            catch { }
            EVA = new ConfigNode("MODULE");
            EVA.AddValue("name", "EvaModule");
            try
            {
                PartLoader.getPartInfoByName("femaleEVA").partPrefab.AddModule(EVA);
            }
            catch { }

        }

    }
}
