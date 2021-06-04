using BepInEx;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("KoikatsuSunshineTrial")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string PluginNameInternal = "KKS_CharacterReplacer";
        internal const string CardNameOther = "N/A";
        internal const string AssetOther = "N/A";
        internal const CardType ExpectedCardType = CardType.KoikatsuSunshine;
    }
}