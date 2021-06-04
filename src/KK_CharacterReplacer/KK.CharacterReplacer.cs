using BepInEx;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("Koikatu")]
    [BepInProcess("KoikatuVR")]
    [BepInProcess("Koikatsu Party")]
    [BepInProcess("Koikatsu Party VR")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInIncompatibility("kokaiinum.janniereplacer")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string PluginNameInternal = "KK_CharacterReplacer";
        internal const string CardNameOther = "Janitor";
        internal const string AssetOther = "ill_Default_Male_Ex";
        internal const CardType ExpectedCardType = CardType.Koikatsu;
    }
}