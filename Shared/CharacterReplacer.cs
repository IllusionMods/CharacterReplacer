using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using KKAPI.Utilities;
using Shared;
using System;
using System.IO;
using UnityEngine;
#if AI
using AIChara;
#endif

namespace IllusionMods
{
    public partial class CharacterReplacer
    {
        public const string Version = "1.5";
        internal static new ManualLogSource Logger;

        public const string FileExtension = ".png";
        public const string Filter = "Character cards (*.png)|*.png|All files|*.*";

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<string> CardPath { get; private set; }

        internal void Awake()
        {
            Logger = base.Logger;

            Enabled = Config.AddSetting("Config", "Enabled", true, new ConfigDescription($"Whether to replace the {CardName} with the selected card.", null, new ConfigurationManagerAttributes { Order = 3 }));
            Config.AddSetting("Config", $"{CardName} Card Replacement", "", new ConfigDescription("Browse for a card.", null, new ConfigurationManagerAttributes { Order = 2, HideDefaultButton = true, CustomDrawer = new Action<ConfigEntryBase>(CardButtonDrawer) }));
            CardPath = Config.AddSetting("Config", "Card Path", "", new ConfigDescription("Path of the replacement card on disk.", null, new ConfigurationManagerAttributes { Order = 1 }));
        }

        private void CardButtonDrawer(ConfigEntryBase configEntry)
        {
            if (GUILayout.Button($"Browse for {CardName} Replacement", GUILayout.ExpandWidth(true)))
                GetCard();
        }

        private void OnCardAccept(string[] path)
        {
            if (path.IsNullOrEmpty()) return;

            var cardType = DetermineCardType(path[0]);
            switch (cardType)
            {
                case ExpectedCardType:
                    CardPath.Value = path[0];
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

        private void GetCard() => OpenFileDialog.Show(path => OnCardAccept(path), "Select replacement card", GetDir(), Filter, FileExtension, OpenFileDialog.OpenSaveFileDialgueFlags.OFN_FILEMUSTEXIST);
        private string GetDir() => CardPath.Value.IsNullOrEmpty() ? Path.Combine(Paths.GameRootPath, @"userdata\chara") : Path.GetDirectoryName(CardPath.Value);

        internal static bool VerifyCard()
        {
            if (!Enabled.Value) return false;
            if (CardPath.Value.IsNullOrEmpty()) return false;

            if (!File.Exists(CardPath.Value))
            {
                Logger.LogMessage($"[{PluginName}]: The replacement card at \n{CardPath.Value}\nseems to be missing. Loading default {CardName} instead.");
                CardPath.Value = "";
                return false;
            }
            if (DetermineCardType(CardPath.Value) != ExpectedCardType)
            {
                Logger.LogMessage($"[{PluginName}]: The replacement card at \n{CardPath.Value}\nseems to be invalid. Loading default {CardName} instead.");
                CardPath.Value = "";
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

        internal enum CardType { None, Unknown, AIGirl, EmotionCreators, HoneySelectFemale, HoneySelectMale, Koikatsu, PlayHomeFemale, PlayHomeMale, PremiumResortFemale, PremiumResortMale }
    }
}
