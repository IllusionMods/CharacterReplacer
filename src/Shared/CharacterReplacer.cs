using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using KKAPI.Utilities;
using System;
using System.IO;
using UnityEngine;
#if AI || HS2
using AIChara;
#endif

namespace IllusionMods
{
    public partial class CharacterReplacer
    {
        public const string GUID = "IllusionMods.CharacterReplacer";
        public const string PluginName = "Character Replacer";
        public const string Version = "1.6.2";
        internal static new ManualLogSource Logger;

        public const string FileExtension = ".png";
        public const string Filter = "Character cards (*.png)|*.png|All files|*.*";
        internal const string CardNameDefaultF = "Default Female";
        internal const string CardNameDefaultM = "Default Male";
        internal const string AssetDefaultF = "ill_Default_Female";
        internal const string AssetDefaultM = "ill_Default_Male";

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<string> CardPathDefaultF { get; private set; }
        public static ConfigEntry<string> CardPathDefaultM { get; private set; }
        public static ConfigEntry<string> CardPathOther { get; private set; }

        internal void Awake()
        {
            Logger = base.Logger;

            Enabled = Config.Bind("Config", "Enabled", true, new ConfigDescription($"Whether to replace selected cards.", null, new ConfigurationManagerAttributes { Order = 10 }));
            Config.Bind("Config", $"{CardNameDefaultF} Card Replacement", "", new ConfigDescription("Browse for a card.", null, new ConfigurationManagerAttributes { Order = 9, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(CardButtonDrawer) }));
            CardPathDefaultF = Config.Bind("Config", $"{CardNameDefaultF} Card Path", "", new ConfigDescription("Path of the replacement card on disk.", null, new ConfigurationManagerAttributes { Order = 8 }));
            Config.Bind("Config", $"{CardNameDefaultM} Card Replacement", "", new ConfigDescription("Browse for a card.", null, new ConfigurationManagerAttributes { Order = 7, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(CardButtonDrawer) }));
            CardPathDefaultM = Config.Bind("Config", $"{CardNameDefaultM} Card Path", "", new ConfigDescription("Path of the replacement card on disk.", null, new ConfigurationManagerAttributes { Order = 6 }));

            AddOtherConfig();
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }

        /// <summary>
        /// Only show the janitor/merchant stuff in games where it exists and in versions of KK where it exists
        /// </summary>
        private void AddOtherConfig()
        {
#if EC || HS2 || KKS
            //EC, HS2 has no Merchant or Janitor
#else
#if KK
            //KK Party and KK without Darkness don't have Janitor
            if (typeof(ChaControl).GetProperty("exType", AccessTools.all) == null) return;
#endif
            Config.Bind("Config", $"{CardNameOther} Card Replacement", "", new ConfigDescription("Browse for a card.", null, new ConfigurationManagerAttributes { Order = 5, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(CardButtonDrawer) }));
            CardPathOther = Config.Bind("Config", $"{CardNameOther} Card Path", "", new ConfigDescription("Path of the replacement card on disk.", null, new ConfigurationManagerAttributes { Order = 4 }));
#endif
        }

        private void CardButtonDrawer(ConfigEntryBase configEntry)
        {
            string text;
            if (configEntry.Definition.Key.StartsWith(CardNameDefaultF))
                text = CardNameDefaultF;
            else if (configEntry.Definition.Key.StartsWith(CardNameDefaultM))
                text = CardNameDefaultM;
            else
                text = CardNameOther;

            if (GUILayout.Button($"Browse for {text} Replacement", GUILayout.ExpandWidth(true)))
                GetCard(configEntry.Definition.Key);
        }

        private void OnCardAccept(string key, string[] path)
        {
            if (path.IsNullOrEmpty()) return;

            var cardType = DetermineCardType(path[0]);
            switch (cardType)
            {
                case ExpectedCardType:
                    if (key.StartsWith(CardNameDefaultF))
                        CardPathDefaultF.Value = path[0];
                    else if (key.StartsWith(CardNameDefaultM))
                        CardPathDefaultM.Value = path[0];
                    else if (key.StartsWith(CardNameOther) && CardPathOther != null)
                        CardPathOther.Value = path[0];
                    break;
                case CardType.None:
                    Logger.LogMessage("Error! Not a card.");
                    break;
                case CardType.Unknown:
                    Logger.LogMessage("Error! Unknown card type.");
                    break;
                default:
                    Logger.LogMessage($"Error! This is a {cardType.ToString()} card.");
                    break;
            }
        }

        private void GetCard(string key) => OpenFileDialog.Show(path => OnCardAccept(key, path), "Select replacement card", GetDir(), Filter, FileExtension, OpenFileDialog.OpenSaveFileDialgueFlags.OFN_FILEMUSTEXIST);
        private string GetDir() => Path.Combine(Paths.GameRootPath, @"userdata\chara");

        internal static bool VerifyCard(ReplacementCardType replacementCardType)
        {
            if (!Enabled.Value) return false;

            ConfigEntry<string> configEntry;
            string text = "";
            if (replacementCardType == ReplacementCardType.DefaultFemale)
                configEntry = CardPathDefaultF;
            else if (replacementCardType == ReplacementCardType.DefaultMale)
                configEntry = CardPathDefaultM;
            else if (replacementCardType == ReplacementCardType.Other && CardPathOther != null)
            {
                configEntry = CardPathOther;
                text = $" {CardNameOther}";
            }
            else return false;

            if (configEntry.Value.IsNullOrEmpty()) return false;

            if (!File.Exists(configEntry.Value))
            {
                Logger.LogMessage($"[{PluginName}]: The replacement card at \n{configEntry.Value}\nseems to be missing. Loading default{text} instead.");
                configEntry.Value = "";
                return false;
            }
            if (DetermineCardType(configEntry.Value) != ExpectedCardType)
            {
                Logger.LogMessage($"[{PluginName}]: The replacement card at \n{configEntry.Value}\nseems to be invalid. Loading default{text} instead.");
                configEntry.Value = "";
                return false;
            }

            return true;
        }

        internal static CardType DetermineCardType(string path)
        {
            if (path.IsNullOrEmpty()) return CardType.None;

            using (var fS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var bR = new BinaryReader(fS))
                {
                    try
                    {
                        PngFile.SkipPng(bR);

                        string tag = bR.ReadString();
                        tag = tag.Remove(0, tag.IndexOf("【") + 1);
                        tag = tag.Remove(tag.IndexOf("】"));

                        switch (tag)
                        {
                            case "AIS_Chara":
                                return CardType.AIGirl;
                            case "EroMakeChara":
                                return CardType.EmotionCreators;
                            case "HoneySelectCharaFemale":
                                return CardType.HoneySelectFemale;
                            case "HoneySelectCharaMale":
                                return CardType.HoneySelectMale;
                            case "KoiKatuChara":
                            case "KoiKatuCharaS":
                            case "KoiKatuCharaSP":
                                return CardType.Koikatsu;
                            case "KoiKatuCharaSun":
                                return CardType.KoikatsuSunshine;
                            case "PlayHome_Female":
                                return CardType.PlayHomeFemale;
                            case "PlayHome_Male":
                                return CardType.PlayHomeMale;
                            case "PremiumResortCharaFemale":
                                return CardType.PremiumResortFemale;
                            case "PremiumResortCharaMale":
                                return CardType.PremiumResortMale;
                            default:
                                return CardType.Unknown;
                        }
                    }
                    catch { }
                }

                return CardType.None;
            }
        }

        internal enum CardType { None, Unknown, AIGirl, EmotionCreators, HoneySelectFemale, HoneySelectMale, Koikatsu, PlayHomeFemale, PlayHomeMale, PremiumResortFemale, PremiumResortMale, KoikatsuSunshine }
        internal enum ReplacementCardType { DefaultFemale, DefaultMale, Other }
    }
}
