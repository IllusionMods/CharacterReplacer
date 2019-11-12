using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using KKAPI;
using System.Linq;

namespace IllusionMods
{
    [BepInProcess("Koikatu")]
    [BepInProcess("Koikatsu Party")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string GUID = "kokaiinum.janniereplacer";
        public const string PluginName = "Janitor Replacer";
        internal const string CardName = "Janitor";
        internal const CardType ExpectedCardType = CardType.Koikatsu;

        internal void Main()
        {
            // There's no reason to install this plugin on a game without Darkness, but just in case someone does so anyway,
            // this check prevents a MissingMethodExcetpion being thrown.
            if (typeof(ChaControl).GetProperties(AccessTools.all).Any(p => p.Name == "exType"))
                HarmonyWrapper.PatchAll(typeof(Hooks));
        }
        internal static class Hooks
        {
            /// <summary>
            /// On loading the Janitor preset character (exType == 1) load the specified replacement card if any.
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(ChaControl), "LoadPreset")]
            public static bool LoadPresetPrefix(int _exType, ChaControl __instance)
            {
                if (_exType != 1) return true;
                if (!VerifyCard()) return true;

                Logger.LogDebug($"Replacing {CardName} with card: {CardPath.Value}");
                __instance.chaFile.LoadCharaFile(CardPath.Value);
                return false;
            }

            /// <summary>
            /// Verify the card is still valid on switching to Darkness H mode
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), "SetMainCanvasObject")]
            public static void SetMainCanvasObjectPrefix(int _mode)
            {
                if (_mode == 4)
                    VerifyCard();
            }
        }
    }
}