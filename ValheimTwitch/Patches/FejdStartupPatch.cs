﻿using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using ValheimTwitch.Config;
using ValheimTwitch.Gui;
using ValheimTwitch.Helpers;
using ValheimTwitch.Twitch.API;
using ValheimTwitch.Twitch.API.Helix;

namespace ValheimTwitch.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "Update")]
    public static class FejdStartupUpdatePatch
    {
        public static bool updateUI = false;

        public static void Postfix()
        {
            // TODO use action queue
            if (updateUI)
            {
                FejdStartupStartPatch.UpdateRewardGrid();
                FejdStartupStartPatch.UpdateMainButonText();
            }

            updateUI = false;
        }
    }

    public class Shortcut
    {
        public string Name { set; get; }
        public string Label { set; get; }
        public KeyCode Code { set; get; } = KeyCode.None;
    }

    [HarmonyPatch(typeof(FejdStartup), "Start")]
    public static class FejdStartupStartPatch
    {
        public static GameObject gui;
        public static GameObject mainGui;
        //public static ValheimTwitchGUIScript guiScript;

        public static List<Shortcut> shortcuts = new List<Shortcut> { 
            new Shortcut { Name = "Whistle", Label = "Whistle" },
            new Shortcut { Name = "ToggleAllRewards", Label = "Toggle all rewards" },
            new Shortcut { Name = "ToggleSpawnRewards", Label = "Toggle spawn rewards" }
        };
        
        public static void LoadShortcuts()
        {
            foreach (var shortcut in shortcuts)
            {
                shortcut.Code = (KeyCode)PluginConfig.GetInt($"shortcut-{shortcut.Name}");

                Log.Info($"Load shortcut {shortcut.Name} -> {shortcut.Code}");

                //guiScript.settingsPanel.AddKeyInput(shortcut.Label, shortcut.Code, (object sender, KeyCodeArgs args) => {
                //    PluginConfig.SetInt($"shortcut-{shortcut.Name}", (int)args.Code);
                //    shortcut.Code = args.Code;
                //});
            }
            shortcuts.Find(s => s.Name == "ToggleSpawnRewards").Code = Plugin.Instance.configProvider.ToggleSpawnRewardsKey;
        }

        public static void Postfix(FejdStartup __instance)
        {
            mainGui = __instance.m_mainMenu;
            //gui.transform.SetParent(mainGui.transform);

            //guiScript = gui.GetComponent<ValheimTwitchGUIScript>();

            //guiScript.mainButton.OnClick(() => OnMainButtonClick());
            //guiScript.refreshRewardButton.OnClick(() => {
            //    Plugin.Instance.UpdateRwardsList();
            //    UpdateRewardGrid();
            //});

            //guiScript.rewardSettings.OnSettingsChanged += OnRewardSettingschanged;
            //guiScript.addRewardForm.OnSave += OnNewRewardSave;

            UpdateMainButonText();
            UpdateRewardGrid();
            ShowUpdatePanel();
            LoadShortcuts();
        }

        public static void ShowUpdatePanel()
        {
            //if (Release.HasNewVersion())
            //    guiScript.updatePanel.SetActive(true);
        }

        private static void OnNewRewardSave(object sender, NewRewardArgs reward)
        {
            try
            {
                var newReward = Plugin.Instance.twitchClient.CreateCustomReward(reward);

                Plugin.Instance.UpdateRwardsList(() =>
                {
                    //guiScript.addRewardForm.Hide();
                    UpdateRewardGrid(newReward.Id);
                    //guiScript.rewardSettings.SetActive(true);
                });
            }
            catch(CustomRewardException e)
            {
                var message = e.Message;

                if (message == "CREATE_CUSTOM_REWARD_DUPLICATE_REWARD")
                {
                    message = "A reward with the same name already exists on your account.";
                }

                //guiScript.addRewardForm.error.Show(message);
            }
        }

        private static void OnMainButtonClick()
        {
            if (Plugin.Instance.GetUser() == null)
            {
                Plugin.Instance.TwitchAuth();
            }
            else
            {
                //guiScript.notAffiliatPanel.SetActive(!Plugin.Instance.twitchClient.HasChannelPoints());
                //guiScript.mainPanel.ToggleActive();
            }
        }

        public static void UpdateMainButonText()
        {
            Plugin.initialScreen.RefreshMainPanel();
            //var user = Plugin.Instance.GetUser();

            //guiScript?.mainButton.SetText(user == null ? "Connexion" : user.DisplayName);
        }

        public static void UpdateRewardGrid(string newRewardId = null)
        {
            //guiScript?.rewardGrid.Clear();

            if (Plugin.Instance?.twitchRewards == null)
            {
                return;
            }

            foreach (Reward reward in Plugin.Instance.twitchRewards.Data)
            {
                try
                {
                    var customReward = Plugin.Instance.twitchCustomRewards.Data.Exists(x => x.Id == reward.Id);

                    //Log.Info($"Reward: {reward.Title} - custom: {customReward}");

                    var title = reward.Title;
                    var data = JToken.FromObject(new object());//RewardsConfig.Get(reward.Id);
                    var color = Colors.FromHex(reward.BackgroundColor);
                    var texture = TextureLoader.LoadFromURL((reward.Image ?? reward.DefaultImage).Url4x);
                    var rewardGridItem = new RewardGridItem(reward.Id, title, color, texture, customReward, data);

                    bool isNew = newRewardId == reward.Id;

                    //guiScript.rewardGrid.Add(rewardGridItem, isNew);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    //Log.Warning($"Reward image unavailable: {reward.Title}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(FejdStartup), "OnStartGame")]
    public static class FejdStartupOnStartGamePatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            Plugin.initialScreen.RefreshMainPanel();
        }
    }

    [HarmonyPatch(typeof(FejdStartup), "ShowStartGame")]
    public static class FejdStartupShowStartGamePatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            Plugin.initialScreen.RefreshMainPanel();
        }
    }

    [HarmonyPatch(typeof(FejdStartup), "OnCredits")]
    public static class FejdStartupOnCreditsPatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            Plugin.initialScreen.RefreshMainPanel();
        }
    }

    [HarmonyPatch(typeof(FejdStartup), "OnSelelectCharacterBack")]
    public static class FejdStartupOnSelelectCharacterBackPatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            Plugin.initialScreen.RefreshMainPanel();
        }
    }

    [HarmonyPatch(typeof(FejdStartup), "OnCreditsBack")]
    public static class FejdStartupOnCreditsBackPatch
    {
        public static void Postfix(FejdStartup __instance)
        {
            Plugin.initialScreen.RefreshMainPanel();
        }
    }
}

