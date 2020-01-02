using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("AI-Syoujyo")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInIncompatibility("kokaiinum.janniereplacer")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        internal const string CardNameOther = "Merchant";
        internal const string AssetOther = "ill_Default_Merchant";
        internal const CardType ExpectedCardType = CardType.AIGirl;

        internal void Main() => HarmonyWrapper.PatchAll(typeof(Hooks));

        internal static partial class Hooks
        {
            /// <summary>
            /// Verify the card is still valid on game load screen
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(AIProject.TitleLoadScene), "Start")]
            internal static void TitleLoadSceneStart() => VerifyCard(ReplacementCardType.Other);
        }
    }
}
