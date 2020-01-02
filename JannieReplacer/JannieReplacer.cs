using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("Koikatu")]
    [BepInProcess("Koikatsu Party")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInIncompatibility("kokaiinum.janniereplacer")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        internal const string CardNameOther = "Janitor";
        internal const string AssetOther = "ill_Default_Male_Ex";
        internal const CardType ExpectedCardType = CardType.Koikatsu;

        internal void Main() => HarmonyWrapper.PatchAll(typeof(Hooks));
        internal static partial class Hooks
        {
            /// <summary>
            /// Verify the card is still valid on switching to Darkness H mode
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(FreeHScene), "SetMainCanvasObject")]
            public static void SetMainCanvasObjectPrefix(int _mode)
            {
                if (_mode == 4)
                    VerifyCard(ReplacementCardType.Other);
            }
        }
    }
}