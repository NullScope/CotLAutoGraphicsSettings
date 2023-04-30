using HarmonyLib;
using Lamb.UI;
using Lamb.UI.MainMenu;

namespace CotLAutoGraphicsSettings.Patches
{
    [HarmonyPatch]
    class GraphicsSettingsPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.Start))]
        public static void UIManager_Start()
        {
            Plugin.SetupConfig();
            Plugin.ApplySettings();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMainMenuController), nameof(UIMainMenuController.Start))]
        public static void LoadMainMenu_Start()
        {
            Plugin.ApplySettings();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.SaveSettings))]
        public static void SettingsManager_SaveSettings()
        { 
            if (!LocationManager._Instance)
			{
                Plugin.SaveDefaultGraphics();
                return;
			}

            switch(LocationManager._Instance.Location)
			{
                case FollowerLocation.Base:
                case FollowerLocation.DoorRoom:
                    Plugin.SaveBaseGraphics();
                    break;
                default:
                    Plugin.SaveDefaultGraphics();
                    break;
			}
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LocationManager), nameof(LocationManager.EndLoadLocation))]
        public static void LocationManager_EndLoadLocation(FollowerLocation location)
        {
            switch(location)
			{
                // These are all called during loading of the Cult Base,
                // so we can ignore these in favor of FollowerLocation.Base which is called last.
                case FollowerLocation.Church:
                case FollowerLocation.Lumberjack:
                case FollowerLocation.DoorRoom:
                    return;

                default:
                    Plugin.ApplySettings(location);
                    break;

            }
        }
    }

}
