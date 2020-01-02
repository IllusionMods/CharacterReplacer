using BepInEx;
using BepInEx.Harmony;
using KKAPI;

namespace IllusionMods
{
    [BepInProcess("Koikatu")]
    [BepInProcess("Koikatsu Party")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInIncompatibility("kokaiinum.janniereplacer")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class CharacterReplacer : BaseUnityPlugin
    {
        internal const string CardNameOther = "Janitor";
        internal const string AssetOther = "ill_Default_Male_Ex";
        internal const CardType ExpectedCardType = CardType.Koikatsu;

        internal void Main() => HarmonyWrapper.PatchAll(typeof(Hooks));
    }
}