using BepInEx;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("AI-Syoujyo")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string PluginNameInternal = "AI_CharacterReplacer";
        internal const string CardNameOther = "Merchant";
        internal const string AssetOther = "ill_Default_Merchant";
        internal const CardType ExpectedCardType = CardType.AIGirl;
    }
}
