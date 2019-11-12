using AIChara;
using BepInEx;
using BepInEx.Harmony;
using HarmonyLib;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("AI-Syoujyo")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string GUID = "kokaiinum.merchantreplacer";
        public const string PluginName = "Merchant Replacer";
        internal const string CardName = "Merchant";
        internal const CardType ExpectedCardType = CardType.AIGirl;

        internal void Main() => HarmonyWrapper.PatchAll(typeof(Hooks));

        internal static class Hooks
        {
            /// <summary>
            /// On loading the Merchant preset character load the specified replacement card if any.
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFromAssetBundle))]
            internal static bool LoadFromAssetBundle(ChaFileControl __instance, string assetName)
            {
                if (assetName != "ill_Default_Merchant") return true;
                if (!VerifyCard()) return true;

                Logger.LogDebug($"Replacing {CardName} with card: {CardPath.Value}");
                __instance.LoadCharaFile(CardPath.Value);
                return false;
            }

            /// <summary>
            /// Verify the card is still valid on game load screen
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(AIProject.TitleLoadScene), "Start")]
            internal static void TitleLoadSceneStart() => VerifyCard();
        }
    }
}
