using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine.SceneManagement;
using ValheimTwitch.Config;
using ValheimTwitch.Events;
using ValheimTwitch.Gui;
using ValheimTwitch.Helpers;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.API.Helix;
using ValheimTwitch.Twitch.Auth;
using WebSocketSharp;

namespace ValheimTwitch
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "dev.skarab42.valheim.twitch";
        public const string NAME = "Valheim Twitch";
        public const string LABEL = "ValheimTwitch";
        public const string VERSION = "2.0.0";

        public const string GITHUB_RELEASE_URL = "https://api.github.com/repos/skarab42/ValheimTwitch/tags";
        public const string TWITCH_REDIRECT_HOST = "localhost";
        public const string TWITCH_SKARAB42_ID = "485824438"; //"18615783"; //
        public const int TWITCH_REDIRECT_PORT = 42224;

        public static readonly string[] TWITCH_SCOPES = {
            "user:read:email",
            "channel:read:redemptions",
            "channel:manage:redemptions"
        };

        public bool isInGame = false;
        public bool isRewardsEnabled = false;
        public bool isHuginIntroShown = false;
        public static bool isRewardUpdating = false;

        public IConfigProvider configProvider;
        public Rewards twitchRewards;
        public Rewards twitchCustomRewards;
        public Twitch.API.Client twitchClient;
        public Twitch.PubSub.Client twitchPubSubClient;

        private static Plugin instance;

        internal static InitialScreen initialScreen;

        public static Plugin Instance
        {
            get => instance;
            private set { instance = value; }
        }

        private Plugin() : base()
        {
            EmbeddedAsset.LoadAssembly("Assets.IpMatcher.dll");
            EmbeddedAsset.LoadAssembly("Assets.netstandard.dll");
            EmbeddedAsset.LoadAssembly("Assets.Newtonsoft.Json.dll");
            EmbeddedAsset.LoadAssembly("Assets.RegexMatcher.dll");
            EmbeddedAsset.LoadAssembly("Assets.System.Runtime.Serialization.dll");
            EmbeddedAsset.LoadAssembly("Assets.UrlMatcher.dll");
            EmbeddedAsset.LoadAssembly("Assets.ValheimTwitchGUI.dll");
            EmbeddedAsset.LoadAssembly("Assets.WatsonWebserver.dll");
            EmbeddedAsset.LoadAssembly("Assets.websocket-sharp.dll");

            if (!PluginConfig.HasKey("channel:manage:redemptions"))
            {
                PluginConfig.DeleteKey("twitchAuthToken");
                PluginConfig.SetString("channel:manage:redemptions", "ok");
            }

            //PluginConfig.DeleteKey("channel:manage:redemptions"); 
            //PluginConfig.DeleteKey("twitchAuthToken");
            //PluginConfig.DeleteKey("rewards");
        }

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

#if DEBUG
            SceneManager.LoadScene("start");
#endif

            InitConfig();
            TwitchConnect();

            SceneManager.activeSceneChanged += OnSceneChanged;

            Harmony harmony = new Harmony(GUID);
            harmony.PatchAll();

            initialScreen = new InitialScreen();
            initialScreen.Start();

            new Twitch.Test.RedeemServer().Start();
        }

        public void InitConfig()
        {
            configProvider = ConfigFactory.GetProvider();
        }

        private void OnToken(object sender, TokenArgs e)
        {
            PluginConfig.SetObject("twitchAuthToken", new {
                accessToken = e.Token.AccessToken,
                refreshToken = e.Token.RefreshToken
            });
            InitConfig();
            TwitchConnect();
            UpdateRwardsList();
        }

        public User GetUser()
        {
            return twitchClient?.user;
        }

        private void OnTokenError(object sender, TokenErrorArgs e)
        {
            Log.Warning($"OnTokenError: {e.Message}");
            // TODO notify user...
        }

        public void TwitchAuth()
        {
            Log.Debug("TwitchAuth....");

            twitchClient.Auth();
        }

        public void TwitchConnect()
        {
            try
            {
                var tokenProvider = new TokenProvider(
                    configProvider.Credentials.clientId,
                    TWITCH_REDIRECT_HOST,
                    TWITCH_REDIRECT_PORT,
                    TWITCH_SCOPES
                );

                tokenProvider.OnToken += OnToken;
                tokenProvider.OnError += OnTokenError;

                twitchClient = new Twitch.API.Client(configProvider, tokenProvider);
                var token = configProvider.Credentials.accessToken;

                if (token.IsNullOrEmpty())
                {
                    return;
                }

                twitchPubSubClient = new Twitch.PubSub.Client(twitchClient);

                User user = twitchClient.GetUser();
                var isFollowing = twitchClient.IsFollowing();
                var isAffiliate = twitchClient.IsAffiliate();
                var isPatner = twitchClient.IsPartner();

                Log.Info($"Twitch User: {user.Login}");
                Log.Info($"- isFollowing: {isFollowing}");
                Log.Info($"- isAffiliate: {isAffiliate}");
                Log.Info($"- isPatner: {isPatner}");

                if (twitchClient.HasChannelPoints()) {
                    //UpdateRwardsList(() => RewardsConfig.Load());
                    UpdateRwardsList();

                    twitchPubSubClient.OnRewardRedeemed += OnRewardRedeemed;
                    twitchPubSubClient.OnMaxReconnect += OnMaxReconnect;

                    twitchPubSubClient.Connect();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e}");
                // TODO notify user...
            }
        }

        public void UpdateRwardsList(Action callback = null)
        {
            new Thread(() =>
            {
                twitchCustomRewards = twitchClient?.GetCustomRewards();
                twitchRewards = twitchClient?.GetRewards();
                if (twitchRewards != null)
                {
                    configProvider.Rewards.Sync(twitchRewards.Data);
                }
                FejdStartupUpdatePatch.updateUI = true;

                if (callback != null)
                    ConsoleUpdatePatch.AddAction(callback);
            }).Start();
        }

        public void ToggleRewards(bool enable)
        {
            if (isRewardUpdating || twitchCustomRewards == null)
                return;

            bool needRefresh = false;
            isRewardsEnabled = enable;
            isRewardUpdating = true;

            if (isInGame)
            {
                var status = enable ? "Enabled" : "Disabled";
                HUDMessageAction.PlayerMessage($"Twitch Rewards {status}");
            }

            foreach (var reward in new List<Reward>(twitchCustomRewards.Data))
            {
                try
                {
                    //Log.Info($">>> ToggleReward: {reward.Id} -> {enable}");
                    twitchClient.ToggleReward(reward.Id, enable);
                }
                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (response.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw e;
                    }

                    needRefresh = true;
                }
            }

            isRewardUpdating = false;

            if (needRefresh)
            {
                UpdateRwardsList();
                FejdStartupUpdatePatch.updateUI = true;
            }
        }

        private void OnSceneChanged(Scene current, Scene next)
        {
            Log.Info("Current scene name " + current.name);
            Log.Info("Next scene name " + next.name);
            isInGame = next.name == "main";

            if (isInGame)
            {
                if (configProvider.EnableRewardsAutomatically)
                {
                    ToggleRewards(true);
                }
            }
            else
            {
                ToggleRewards(false);
            }
            if(isInGame)
            {
                //Prefab.ListPrefabs();
                //ChangeEnvironmentAction.ListEnvironments();
            }
        }

        private void OnMaxReconnect(object sender, Twitch.PubSub.MaxReconnectErrorArgs e)
        {
            Log.Info($"OnMaxReconnect: {e.Message}");
        }

        private void OnRewardRedeemed(object sender, Twitch.PubSub.RewardRedeemedArgs e)
        {
            var reward = e.Redemption.Reward;

            Log.Info($"OnRewardRedeemed: {reward.Title}");

            var action = configProvider.Rewards.GetSettings(reward.Id);

            if (action == null)
                return;

            Actions.RunAction(e.Redemption, action);
        }
    }
}