using HarmonyLib;
#if AI || HS2
using AIChara;
#endif

namespace IllusionMods
{
    public partial class CharacterReplacer
    {
        internal static partial class Hooks
        {
            /// <summary>
            /// Verify the card is still valid and perform replacement
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFromAssetBundle))]
            internal static bool LoadFromAssetBundle(ChaFileControl __instance, string assetName)
            {
                if (assetName == AssetDefaultF)
                {
                    if (!VerifyCard(ReplacementCardType.DefaultFemale)) return true;
                    Logger.LogDebug($"Replacing {CardNameDefaultF} with card: {CardPathDefaultF.Value}");
                    __instance.LoadCharaFile(CardPathDefaultF.Value);
                    return false;
                }
                else if (assetName == AssetDefaultM)
                {
                    if (!VerifyCard(ReplacementCardType.DefaultMale)) return true;
                    Logger.LogDebug($"Replacing {CardNameDefaultM} with card: {CardPathDefaultM.Value}");
                    __instance.LoadCharaFile(CardPathDefaultM.Value);
                    return false;
                }
                else if (assetName == AssetOther && CardPathOther != null)
                {
                    if (!VerifyCard(ReplacementCardType.Other)) return true;
                    Logger.LogDebug($"Replacing {CardNameOther} with card: {CardPathOther.Value}");
                    __instance.LoadCharaFile(CardPathOther.Value);
                    return false;
                }

                return true;
            }
        }
    }
}