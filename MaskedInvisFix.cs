using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;

namespace MaskedInvisFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    //[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]
    //[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Patch)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }


        //disable these bools by using code like this in your plugin AWAKE so that when your mod is installed, you can force that patch to not run.
        //      if (Chainloader.PluginInfos.Keys.Any((string k) => k == "VirusTLNR.MaskedFixes"))
        //      {
        //          MaskedFixes.Plugin.[ThePatchName] = false;
        //      }
        //so, if you set MaskedFixes.Plugin.MaskedInvisibilityFix = false;, it will disable the MaskedInvisibilityFix
        public static bool MaskedInvisibilityFix = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Logger = base.Logger;

            Patch();

            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(PluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
