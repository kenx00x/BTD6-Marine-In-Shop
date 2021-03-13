﻿using Assets.Main.Scenes;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Simulation.Input;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.Upgrade;
using Harmony;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System.IO;
[assembly: MelonInfo(typeof(BTD6_Marine_In_Shop.Class1), "Marine In Shop", "1.1.0", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BTD6_Marine_In_Shop
{
    public class Class1 :MelonMod
    {
        public static string dir = $"{Directory.GetCurrentDirectory()}\\Mods\\MarineInShop";
        public static string config = $"{dir}\\config.txt";
        public static int MarineCost = 2500;
        public override void OnApplicationStart()
        {
            MelonLogger.Log("Marine In Shop mod loaded");
            Directory.CreateDirectory($"{dir}");
            if (File.Exists(config))
            {
                MelonLogger.Log("Reading config file");
                using (StreamReader sr = File.OpenText(config))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        MarineCost = int.Parse(s.Substring(s.IndexOf(char.Parse("=")) + 1));
                    }
                }
                MelonLogger.Log("Done reading");
            }
            else
            {
                MelonLogger.Log("Creating config file");
                using (StreamWriter sw = File.CreateText(config))
                {
                    sw.WriteLine("MarineCost=2500");
                }
                MelonLogger.Log("Done Creating");
            }
        }
        [HarmonyPatch(typeof(ProfileModel), "Validate")]
        public class ProfileModel_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ProfileModel __instance)
            {
                HashSet<string> unlockedTowers = __instance.unlockedTowers;
                if (unlockedTowers.Contains("Marine"))
                {
                    MelonLogger.Log("Marine already unlocked");
                }
                else
                {
                    MelonLogger.Log("unlocking Marine");
                    unlockedTowers.Add("Marine");
                }
            }
        }
        [HarmonyPatch(typeof(TitleScreen), "UpdateVersion")]
        public class TitleScreen_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                TowerModel towerModel = Game.instance.model.GetTowerWithName("Marine");
                if (towerModel.icon == null)
                {
                    towerModel.icon = towerModel.portrait;
                }
                towerModel.cost = MarineCost;
                towerModel.towerSet = "Military";
            }
        }
        [HarmonyPatch(typeof(TowerInventory), "Init")]
        public class TowerInventory_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref List<TowerDetailsModel> allTowersInTheGame)
            {
                ShopTowerDetailsModel towerDetails = new ShopTowerDetailsModel("Marine", 1, 0, 0, 0, -1, 0, null);
                allTowersInTheGame.Add(towerDetails);
                return true;
            }
        }
        [HarmonyPatch(typeof(UpgradeScreen), "UpdateUi")]
        public class UpgradeScreen_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref string towerId)
            {
                if (towerId.Contains("Marine"))
                {
                    towerId = "DartMonkey";
                }
                return true;
            }
        }
    }
}