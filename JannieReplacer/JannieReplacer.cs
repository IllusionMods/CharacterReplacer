using System.IO;
using System.ComponentModel;
using BepInEx;
using Harmony;
using UnityEngine;
using KKAPI;
using KKAPI.Utilities;
using Logger = BepInEx.Logger;
using BepInEx.Logging;
using System;

namespace JannieReplacer {
    [BepInProcess("Koikatu")]
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(GUID, Name, Version)]
    public class JannieReplacer : BaseUnityPlugin {

        public const string GUID = "kokaiinum.janniereplacer";
        public const string Name = "Janitor Replacer";
        public const string Version = "1.1";


        public const string FileExtension = ".png";
        public const string Filter = "Koikatu cards (*.png)|*.png|All files|*.*";


        [Category("Settings")]
        [DisplayName("Janitor character replacement")]
        [Description("Enable or disable swapping the janitor's character model\nwith a character card.")]
        public static ConfigWrapper<bool> Enabled { get; private set; }

        [Browsable(true)]
        [Category("Settings")]
        [DisplayName("Janitor replacement card")]        
        [CustomSettingDraw(nameof(CardGetButton))]        
        public string Card { get => null; private set { CardGetButton(); } }

        [Category("Settings")]
        [DisplayName("Replacement card location")]
        [Advanced(true)]
        public static ConfigWrapper<string> FilePath { get; private set; }


        private void CardGetButton() {
            if (GUILayout.Button("Browse for Janitor Replacement", GUILayout.ExpandWidth(true))) {
                GetCard();
            }
        }

        private void GetCard() {
            OpenFileDialog.Show(path => OnCardAccept(path), "Select replacement card", GetDir(), Filter, FileExtension, OpenFileDialog.OpenSaveFileDialgueFlags.OFN_FILEMUSTEXIST);
        }

        private void OnCardAccept(string[] path) {
            if (path.IsNullOrEmpty()) return;
            if (IsKoiCard(path[0])) {
                FilePath.Value = path[0];
            }
            else {
                Logger.Log(LogLevel.Message, "Error! Not a card?");
            }
        }

        private string GetDir() {
            if (FilePath.Value.IsNullOrEmpty()) {
                return Path.Combine(Paths.GameRootPath, "userdata\\chara");
            }
            else return Path.GetDirectoryName(FilePath.Value);
        }

        private static bool IsKoiCard(string path) {
            if (path.IsNullOrEmpty()) return false;
            using (var fS = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var bR = new BinaryReader(fS)) {
                    try {
                        PngFile.SkipPng(bR);
                        bR.ReadInt32();
                    }
                    catch (EndOfStreamException) {                        
                        return false;
                    }
                    try {
                        if (bR.ReadString() == "【KoiKatuChara】")
                            return true;
                    }
                    catch (EndOfStreamException) {
                        return false;
                    }
                }
                return false;
            }
        }        

        private void Awake() {
            Enabled = new ConfigWrapper<bool>("Enabled", GUID, true);
            FilePath = new ConfigWrapper<string>(nameof(FilePath), GUID, null);
            var harmony = HarmonyInstance.Create(GUID);
            harmony.PatchAll(typeof(JannieReplacer));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChaControl), "LoadPreset")]
        public static bool LoadPresetPrefix(int _exType, ChaControl __instance) {
            if (Enabled.Value) {
                if (!FilePath.Value.IsNullOrEmpty()) {
                    if (_exType == 1) {
                        if (!File.Exists(FilePath.Value)) {
                            Logger.Log(LogLevel.Message, $"The replacement card at \n{FilePath.Value}\nseems to be missing. Loading default instead.");
                            FilePath.Value = null;
                            return true;
                        }
                        if (!IsKoiCard(FilePath.Value)) {
                            Logger.Log(LogLevel.Message, $"The replacement card at \n{FilePath.Value}\nseems to be invalid. Loading default instead.");
                            FilePath.Value = null;
                            return true;
                        }
                        __instance.chaFile.LoadFileLimited(FilePath.Value);
                        return false;
                    }
                }                
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FreeHScene), "SetMainCanvasObject")]
        public static void SetMainCanvasObjectPrefix(int _mode) {
            if (_mode == 4) {
                if (Enabled.Value) {
                    if (!FilePath.Value.IsNullOrEmpty()) {
                        try {
                            if (!File.Exists(FilePath.Value)) {
                                Logger.Log(LogLevel.Message, $"The replacement card at \n{FilePath.Value}\nseems to be missing. The default janitor will be loaded unless you change it.");
                            }
                            if (!IsKoiCard(FilePath.Value)) {
                                Logger.Log(LogLevel.Message, $"The replacement card at \n{FilePath.Value}\nseems to be invalid. The default janitor will be loaded unless you change it.");
                            }
                        }
                        catch (Exception) { };
                    }
                }
            }
            
        }

    }
}
