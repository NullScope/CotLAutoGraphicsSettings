using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;

namespace CotLAutoGraphicsSettings
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "io.github.NullScope.CotLAutoGraphicsSettings";
        public const string PluginName = "CotLAutoGraphicsSettings";
        public const string PluginVer = "1.0.0";

        internal static ManualLogSource Log;
        internal readonly static Harmony Harmony = new(PluginGuid);

        internal static string PluginPath;
        internal static ConfigFile Cfg;

        internal static ConfigEntry<string> DefaultGraphicsConfig = null!;
        internal static ConfigEntry<string> CultBaseGraphicsConfig = null!;

        internal static SettingsData.GraphicsSettings DefaultGraphics = null!;
        internal static SettingsData.GraphicsSettings CultBaseGraphics = null!;

        private void Awake()
        {
            Plugin.Log = base.Logger;
            Plugin.Cfg = base.Config;

            Plugin.PluginPath = Path.GetDirectoryName(Info.Location);
        }

        private void OnEnable()
        {
            Harmony.PatchAll();
            Logger.LogInfo($"Loaded {PluginName}!");
        }

        private void OnDisable()
        {
            Harmony.UnpatchSelf();
            Logger.LogInfo($"Unloaded {PluginName}!");
        }

        public static SettingsData.GraphicsSettings CloneSettings (SettingsData.GraphicsSettings settings)
		{
			var newSettings = new SettingsData.GraphicsSettings(settings)
			{
				// For some reason the constructor does not copy over the AntiAliasing option, so we set it here.
				AntiAliasing = settings.AntiAliasing
			};

			return newSettings;
        }

        public static void SetupConfig()
		{
            DefaultGraphicsConfig = Cfg.Bind("Graphics", "Default", JsonConvert.SerializeObject(SettingsManager.Settings.Graphics), "Default Graphics Settings");
            CultBaseGraphicsConfig = Cfg.Bind("Graphics", "Base", JsonConvert.SerializeObject(SettingsManager.Settings.Graphics), "Cult Base Graphics Settings");
            DefaultGraphics = JsonConvert.DeserializeObject<SettingsData.GraphicsSettings>(DefaultGraphicsConfig.Value);
            CultBaseGraphics = JsonConvert.DeserializeObject<SettingsData.GraphicsSettings>(CultBaseGraphicsConfig.Value);
        }

        public static void SaveDefaultGraphics()
		{
            DefaultGraphicsConfig.BoxedValue = JsonConvert.SerializeObject(SettingsManager.Settings.Graphics);
            DefaultGraphics = CloneSettings(SettingsManager.Settings.Graphics);
		}

        public static void SaveBaseGraphics()
		{
            CultBaseGraphicsConfig.BoxedValue = JsonConvert.SerializeObject(SettingsManager.Settings.Graphics);
            CultBaseGraphics = CloneSettings(SettingsManager.Settings.Graphics);
        }

        public static void ApplySettings(FollowerLocation location = FollowerLocation.None)
		{
            if (location == FollowerLocation.Base)
            {
                SettingsManager.Settings.Graphics = CloneSettings(CultBaseGraphics);
            }
            else
            {
                SettingsManager.Settings.Graphics = CloneSettings(DefaultGraphics);
            }

            SettingsManager.Instance.ApplySettings();
        }
    }
}