using BepInEx;
using BepInEx.Harmony;
using KKAPI;

namespace IllusionMods
{
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInIncompatibility("kokaiinum.janniereplacer")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string PluginNameInternal = "EC_CharacterReplacer";
        internal const string CardNameOther = "N/A";
        internal const string AssetOther = "N/A";
        internal const CardType ExpectedCardType = CardType.EmotionCreators;

        internal void Main() => HarmonyWrapper.PatchAll(typeof(Hooks));
    }
}