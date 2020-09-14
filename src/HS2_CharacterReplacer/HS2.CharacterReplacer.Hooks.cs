using HarmonyLib;

namespace IllusionMods
{
    public partial class CharacterReplacer
    {
        internal static partial class Hooks
        {
            /// <summary>
            /// Verify the card is still valid on game load screen
            /// </summary>
            [HarmonyPrefix, HarmonyPatch(typeof(HS2.TitleScene), "Start")]
            internal static void TitleLoadSceneStart() => VerifyCard(ReplacementCardType.Other);
        }
    }
}
