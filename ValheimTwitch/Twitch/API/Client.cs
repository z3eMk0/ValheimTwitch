using Newtonsoft.Json;
using System.IO;
using System.Net;
using UnityEngine;
using ValheimTwitch.Config;
using ValheimTwitch.Twitch.API.Helix;
using ValheimTwitch.Twitch.Auth;

namespace ValheimTwitch.Twitch.API
{
    // TODO ip: move it to a better place
    public class NewRewardArgs : System.EventArgs
    {
        public NewRewardArgs()
        {
            this.IsEnabled = false;
        }
        public string Title { get; set; }
        public string Cost { get; set; }
        public string Prompt { get; set; }
        public string IsUserInputRequired { get; set; }
        public string ShouldRedemptionsSkipRequestQueue { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class Client
    {
        public User user;
        public TokenProvider tokenProvider;
        public readonly Credentials credentials;

        private readonly string helixURL;

        public Client(string clientId, string accessToken, string refreshToken, TokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
            credentials = new Credentials(clientId, accessToken, refreshToken);
        }

        public Client(IConfigProvider configProvider, TokenProvider tokenProvider)
        {
            helixURL = configProvider.HelixUrl;
            credentials = configProvider.Credentials;
            this.tokenProvider = tokenProvider;
        }

        public string Get(string url, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");

                    return client.DownloadString(url);
                }
                catch (WebException e)
                {
                    Log.Error($"get {url} error {e}");
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return Get(url, false);
                }
            }
        }

        public string Post(string url, string query, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                Log.Info($"POST {credentials.clientId} {credentials.accessToken}");
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                    return client.UploadString(url, "POST", query);
                }
                catch (WebException e)
                {
                    Log.Error($"POST error: {e.ToString()}");
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return Post(url, query, false);
                }
            }
        }

        public string PostData(string url, string data, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                Log.Info($"POST data {credentials.clientId} {credentials.accessToken}");
                try
                {
                    client.Headers.Add("client-id", credentials.clientId);
                    client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {credentials.accessToken}");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    return client.UploadString(url, "POST", data);
                }
                catch (WebException e)
                {
                    Log.Error($"POST data error: {e.ToString()}");
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return PostData(url, data, false);
                }
            }
        }

        public string Patch(string url, string query, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    return client.UploadString(url, "PATCH", query);
                }
                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return Patch(url, query, false);
                }
            }
        }

        public string Delete(string url, bool refresh = true)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Headers.Add($"Client-Id: {credentials.clientId}");
                    client.Headers.Add($"Authorization: Bearer {credentials.accessToken}");

                    return client.UploadString(url, "DELETE", string.Empty);
                }
                catch (WebException e)
                {
                    HttpWebResponse response = (HttpWebResponse)e.Response;

                    if (refresh == false || response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        throw e;
                    }

                    if (tokenProvider.RefreshToken(this) == null)
                    {
                        throw e;
                    }

                    return Delete(url, false);
                }
            }
        }

        public User GetUser(bool force = false)
        {
            if (user != null && force == false)
            {
                return user;
            }

            string json = Get($"{helixURL}/users");

            var users = JsonConvert.DeserializeObject<Users>(json);

            user = users.Data[0];

            return user;
        }

        public void Auth()
        {
            tokenProvider.GetToken();
        }

        public string GetUserAcessToken()
        {
            return credentials.accessToken;
        }

        public Rewards GetRewards(bool custom = false)
        {
            if (!HasChannelPoints())
                return null;
            
            string json = Get($"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}&only_manageable_rewards={custom}");

            return JsonConvert.DeserializeObject<Rewards>(json);
        }

        public Rewards GetCustomRewards()
        {
            return GetRewards(true);
        }

        public bool IsAffiliate()
        {
            return user?.BroadcasterType == "affiliate";
        }

        public bool IsPartner()
        {
            return user?.BroadcasterType == "partner";
        }

        public bool HasChannelPoints()
        {
            return IsAffiliate() || IsPartner();
        }

        public bool IsFollowing()
        {
            string json = Get($"{helixURL}/users/follows?to_id={Plugin.TWITCH_SKARAB42_ID}&from_id={user.Id}");
            var follower = JsonConvert.DeserializeObject<FollowsResponse>(json);

            return follower?.Total == 1;
        }

        public string ToggleReward(string id, bool enabled)
        {
            try
            {
                var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}&id={id}";

                return Patch(url, JsonConvert.SerializeObject(new { is_enabled = enabled }));
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var json = reader.ReadToEnd();
                    var message = JsonConvert.DeserializeObject<CustomRewardResponse>(json);

                    throw new CustomRewardException(message);
                }

                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw e;
                }

                return null;
            }
        }

        public string UpdateCustomReward(string id, NewRewardArgs reward)
        {
            try
            {
                var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}&id={id}";
                var data = new
                {
                    cost = int.Parse(reward.Cost),
                    title = reward.Title,
                    prompt = reward.Prompt,
                    is_user_input_required = reward.IsUserInputRequired == bool.TrueString,
                    should_redemptions_skip_request_queue = reward.ShouldRedemptionsSkipRequestQueue == bool.TrueString,
                    is_enabled = reward.IsEnabled,
                };
                var dataString = JsonConvert.SerializeObject(data);
                Log.Info($"UpdateReward: {url} {dataString}");

                return Patch(url, dataString);
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var json = reader.ReadToEnd();
                    var message = JsonConvert.DeserializeObject<CustomRewardResponse>(json);

                    throw new CustomRewardException(message);
                }

                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw e;
                }

                return null;
            }
        }

        public string DeleteCustomReward(string id)
        {
            try
            {
                var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}&id={id}";
                Log.Info($"DeleteReward: {url}");

                return Delete(url);
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var json = reader.ReadToEnd();
                    var message = JsonConvert.DeserializeObject<CustomRewardResponse>(json);

                    throw new CustomRewardException(message);
                }

                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw e;
                }

                return null;
            }
        }

        public Reward CreateCustomReward(NewRewardArgs reward)
        {
            var color = Random.ColorHSV(0f, 1f, 0.6f, 0.7f, 0.4f, 0.5f);
            var backgroundColor = "#" + ColorUtility.ToHtmlStringRGB(color);
            var url = $"{helixURL}/channel_points/custom_rewards?broadcaster_id={user.Id}";
            var data = new
            {
                is_enabled = false,
                cost = int.Parse(reward.Cost),
                title = reward.Title,
                prompt = reward.Prompt,
                background_color = backgroundColor,
                is_user_input_required = reward.IsUserInputRequired == bool.TrueString,
                should_redemptions_skip_request_queue = reward.ShouldRedemptionsSkipRequestQueue == bool.TrueString,
            };
            var dataString = JsonConvert.SerializeObject(data);
            Log.Info($"CreateCustomReward: {url} {dataString}");
            try
            {
                var json = PostData(url, dataString);

                return JsonConvert.DeserializeObject<Rewards>(json).Data[0];
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;
                
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var json = reader.ReadToEnd(); 
                    var message = JsonConvert.DeserializeObject<CustomRewardResponse>(json);

                    throw new CustomRewardException(message);
                }

                if (response.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw e;
                }

                return null;
            }
        }
    }
}
