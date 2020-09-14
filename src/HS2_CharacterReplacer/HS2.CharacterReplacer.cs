using BepInEx;
using BepInEx.Harmony;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("HoneySelect2")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInIncompatibility("kokaiinum.janniereplacer")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        public const string PluginNameInternal = "HS2_CharacterReplacer";
        internal const string CardNameOther = "N/A";
        internal const string AssetOther = "N/A";
        internal const CardType ExpectedCardType = CardType.AIGirl;

        internal void Main() => HarmonyWrapper.PatchAll(typeof(Hooks));
    }
}
