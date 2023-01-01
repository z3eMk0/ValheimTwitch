using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValheimTwitch.Events;
using ValheimTwitch.Twitch.PubSub.Messages;
using WatsonWebserver;

namespace ValheimTwitch.Twitch.Test
{
    class RedeemServer
    {
        private const string SERVER_HOST = "localhost";
        public const int SERVER_PORT = 42248;

        private Server server;

        public void Start()
        {
            server = new Server(SERVER_HOST, SERVER_PORT, false, Route);
            server.Start();
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
                if (context.Request.Method == HttpMethod.POST
                    && context.Request.Url.RawWithoutQuery.Equals("/eventsub/"))
                {
                    var eventJSON = context.Request.DataAsJsonObject<JToken>();
                    Log.Info($"redeem data {eventJSON}");
                    var redeemData = context.Request.DataAsJsonObject<RedeemData>();
                    var action = Plugin.Instance.configProvider.Rewards.GetSettings(redeemData.Event.Reward.Id);

                    if (action == null)
                        return;

                    var redemption = new Redemption();
                    redemption.User = new User();
                    redemption.User.DisplayName = "test zemko";
                    redemption.Id = redeemData.Event.Reward.Id;
                    redemption.Reward = new Reward();
                    redemption.Reward.Id = redeemData.Event.Reward.Id;
                    Log.Info($"starting action {JToken.FromObject(action)}");
                    Actions.RunAction(redemption, action);
                    await ServeHTML(context, 200, "OK");
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

        private class RedeemData
        {
            [JsonProperty("event")]
            public EventData Event { get; set; }
        }

        private class EventData
        {
            [JsonProperty("broadcaster_user_id")]
            public string BroadcasterId { get; set; }
            [JsonProperty("reward")]
            public RewardData Reward { get; set; }
        }

        private class RewardData
        {
            [JsonProperty("cost")]
            public int Cost { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
        }
    }
}

