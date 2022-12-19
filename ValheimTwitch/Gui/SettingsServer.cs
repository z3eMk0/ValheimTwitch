using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ValheimTwitch.Helpers;
using ValheimTwitch.Twitch.API;
using ValheimTwitch.Twitch.API.Helix;
using WatsonWebserver;

namespace ValheimTwitch.Gui
{
    class SettingsServer
    {
        private const string SERVER_HOST = "localhost";
        public const int SERVER_PORT = 42244;

        private Server server;

        public void OpenSettings()
        {
            server = new Server(SERVER_HOST, SERVER_PORT, false, Route);
            server.Start();
            Application.OpenURL($"http://{SERVER_HOST}:{SERVER_PORT}/settings");
        }

        public void Stop()
        {
            server.Stop();
            server.Dispose();
            server = null;
        }

        private async Task Route(HttpContext context)
        {
            try
            {
                Log.Info($"server request {context.Request.Method} {context.Request.Url.RawWithoutQuery}");
                if (context.Request.Method == HttpMethod.GET
                    && context.Request.Url.RawWithoutQuery.Equals("/settings"))
                {
                    string content = EmbeddedAsset.LoadString("Assets.settings.html");
                    await ServeHTML(context, 200, content);
                }
                else if (context.Request.Method == HttpMethod.GET
                    && context.Request.Url.RawWithoutQuery.Equals("/favicon.ico"))
                {
                    await ServeHTML(context, 200, "");
                }
                else if (context.Request.Method == HttpMethod.GET
                    && context.Request.Url.RawWithoutQuery.Equals("/rewards"))
                {
                    var rewards = Plugin.Instance.twitchClient.GetRewards();
                    var customRewards = Plugin.Instance.twitchClient.GetCustomRewards();
                    var rewardsSettings = rewards.Data.ConvertAll(reward =>
                    {
                        var settings = RewardsConfig.GetSettings(reward.Id);
                        bool isManagable = customRewards.Data.Exists(r => r.Id == reward.Id);
                        return new RewardSettings(reward, settings, isManagable);
                    });
                    await ServeJSON(context, 200, rewardsSettings);
                }
                else if (context.Request.Method == HttpMethod.POST
                    && context.Request.Url.RawWithoutQuery.Equals("/rewards"))
                {
                    var rewardSettings = context.Request.DataAsJsonObject<RewardSettings>();
                    if (rewardSettings.data.Title.Length == 0)
                    {
                        await ServeHTML(context, 400, "need title");
                    }
                    else
                    {
                        var rewardArgs = new NewRewardArgs
                        {
                            Title = rewardSettings.data.Title,
                            Cost = rewardSettings.data.Cost.ToString(),
                            IsUserInputRequired = rewardSettings.data.IsUserInputRequired.ToString(),
                            Prompt = rewardSettings.data.Prompt,
                            ShouldRedemptionsSkipRequestQueue = rewardSettings.data.ShouldRedemptionsSkipRequestQueue.ToString()
                        };
                        var reward = Plugin.Instance.twitchClient.CreateCustomReward(rewardArgs);
                        var data = this.GetData(context.Request);
                        Log.Info($"Reward settings {JsonConvert.SerializeObject(data)}");
                        var settingsChangedArgs = new SettingsChangedArgs
                        {
                            RewardId = reward.Id,
                            Data = data
                        };
                        this.SaveRewardSettings(settingsChangedArgs);
                        await ServeHTML(context, 201, "Created");
                    }
                }
                else if (context.Request.Method == HttpMethod.PUT
                    && context.Request.Url.RawWithoutQuery.StartsWith("/rewards/"))
                {
                    var id = context.Request.Url.RawWithoutQuery.Substring("/rewards/".Length);
                    var rewardSettings = context.Request.DataAsJsonObject<RewardSettings>();
                    if (rewardSettings.data.Title.Length == 0)
                    {
                        await ServeHTML(context, 400, "need title");
                    }
                    else
                    {
                        var rewardArgs = new NewRewardArgs
                        {
                            Title = rewardSettings.data.Title,
                            Cost = rewardSettings.data.Cost.ToString(),
                            IsUserInputRequired = rewardSettings.data.IsUserInputRequired.ToString(),
                            Prompt = rewardSettings.data.Prompt,
                            ShouldRedemptionsSkipRequestQueue = rewardSettings.data.ShouldRedemptionsSkipRequestQueue.ToString(),
                            IsEnabled = rewardSettings.data.IsEnabled,
                        };
                        var reward = Plugin.Instance.twitchClient.UpdateCustomReward(id, rewardArgs);
                        var data = this.GetData(context.Request);
                        Log.Info($"Reward settings {JsonConvert.SerializeObject(data)}");
                        var settingsChangedArgs = new SettingsChangedArgs
                        {
                            RewardId = id,
                            Data = data
                        };
                        this.SaveRewardSettings(settingsChangedArgs);
                        await ServeHTML(context, 200, "Updated");
                    }
                }
                else if (context.Request.Method == HttpMethod.DELETE
                    && context.Request.Url.RawWithoutQuery.StartsWith("/rewards/"))
                {
                    var id = context.Request.Url.RawWithoutQuery.Substring("/rewards/".Length);
                    var reward = Plugin.Instance.twitchClient.DeleteCustomReward(id);
                    RewardsConfig.Delete(id);
                    await ServeHTML(context, 200, "Deleted");
                }
                else
                {
                    await ServeHTML(context, 404, "not found");
                }
            }
            catch (Exception ex)
            {
                Log.Error("HTTP ERROR -> " + ex.ToString());
                await ServeHTML(context, 500, ex.ToString());
            }
        }

        private static Task<bool> ServeHTML(HttpContext context, int code, string content)
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = "text/html; charset=utf-8";
            return context.Response.Send(content);
        }

        private static Task<bool> ServeJSON(HttpContext context, int code, object data)
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json; charset=utf-8";
            var content = JsonConvert.SerializeObject(data);
            return context.Response.Send(content);
        }

        private SettingsMessageData GetData(HttpRequest request)
        {
            var body = request.DataAsJsonObject<JToken>();
            Log.Info("data settings action " + body.ToString());
            var dataJSON = body["settings"];
            var data = Model.FromToken(dataJSON);
            return data;
        }
        private void SaveRewardSettings(SettingsChangedArgs e)
        {
            RewardsConfig.Set(e.RewardId, e.Data);
        }

        private class RewardSettings
        {
            public RewardSettings()
            {
            }
            public RewardSettings(Reward data, SettingsMessageData settings, bool isManagable)
            {
                this.data = data;
                this.settings = settings;
                this.isManagable = isManagable;
            }

            public Reward data;
            public SettingsMessageData settings;
            public bool isManagable;
        }
    }
}

