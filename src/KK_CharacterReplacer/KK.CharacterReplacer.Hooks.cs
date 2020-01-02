using HarmonyLib;

namespace IllusionMods
{
    public partial class CharacterReplacer
    {
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