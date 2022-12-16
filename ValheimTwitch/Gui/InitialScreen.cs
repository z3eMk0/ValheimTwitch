using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.Gui
{
    class InitialScreen
    {
        private GameObject affiliateTextObject;
        private GameObject userTextOjbect;
        private GameObject rewardsSettingsButtonObject;
        private GameObject loginButtonObject;
        private GameObject logoutButtonObject;
        private SettingsServer server;

        public void Start()
        {
            GUIManager.OnCustomGUIAvailable += Draw;
        }

        public void Draw()
        {
            if (GUIManager.Instance == null)
            {
                Log.Error("GUIManager instance is null");
                return;
            }

            if (!GUIManager.CustomGUIFront)
            {
                Log.Error("GUIManager CustomGUI is null");
                return;
            }

            this.CreateMainPanel();
        }

        private void CreateMainPanel()
        {
            var panelObject = GUIManager.Instance.CreateWoodpanel(
                parent: GUIManager.CustomGUIFront.transform,
                anchorMin: new Vector2(0f, 0.66f),
                anchorMax: new Vector2(0.33f, 1f),
                position: new Vector2(0f, 0f),
                width: 300f,
                height: 100f,
                draggable: true);
            
            this.affiliateTextObject = GUIManager.Instance.CreateText(
                text: "You need to be a Twitch affiliate or partner to use rewards",
                parent: panelObject.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, -60f),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 15,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 200f,
                height: 60f,
                addContentSizeFitter: false);
            this.affiliateTextObject.SetActive(false);
            
            this.rewardsSettingsButtonObject = GUIManager.Instance.CreateButton(
                text: "View rewards",
                parent: panelObject.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, -40f),
                width: 200f,
                height: 30f);
            this.rewardsSettingsButtonObject.SetActive(false);
            Button rewardsButton = this.rewardsSettingsButtonObject.GetComponent<Button>();
            rewardsButton.onClick.AddListener(this.ShowRewardsSettings);
            
            GUIManager.Instance.CreateText(
                text: "Welcome",
                parent: panelObject.transform,
                anchorMin: new Vector2(0f, 1f),
                anchorMax: new Vector2(0f, 1f),
                position: new Vector2(40f, -20f),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 15,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 70f,
                height: 30f,
                addContentSizeFitter: false);
            
            this.userTextOjbect = GUIManager.Instance.CreateText(
                text: "",
                parent: panelObject.transform,
                anchorMin: new Vector2(0f, 1f),
                anchorMax: new Vector2(0f, 1f),
                position: new Vector2(155f, -20f),
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: 15,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: 150f,
                height: 30f,
                addContentSizeFitter: false);
            this.userTextOjbect.SetActive(false);

            this.loginButtonObject = GUIManager.Instance.CreateButton(
                text: "Login to Twitch",
                parent: panelObject.transform,
                anchorMin: new Vector2(0.5f, 1f),
                anchorMax: new Vector2(0.5f, 1f),
                position: new Vector2(0f, -90f),
                width: 200f,
                height: 30f);
            this.loginButtonObject.SetActive(false);
            Button loginButton = this.loginButtonObject.GetComponent<Button>();
            loginButton.onClick.AddListener(this.LoginToTwitch);

            this.logoutButtonObject = GUIManager.Instance.CreateButton(
               text: "Logout from Twitch",
               parent: panelObject.transform,
               anchorMin: new Vector2(0.5f, 1f),
               anchorMax: new Vector2(0.5f, 1f),
               position: new Vector2(0f, -90f),
               width: 200f,
               height: 30f);
            this.logoutButtonObject.SetActive(false);
            Button logoutButton = this.logoutButtonObject.GetComponent<Button>();
            logoutButton.onClick.AddListener(this.LogoutFromTwitch);

            this.RefreshMainPanel();
        }

        public void RefreshMainPanel()
        {
            var user = Plugin.Instance.GetUser();
            if (user != null)
            {
                var text = this.userTextOjbect.GetComponent<Text>();
                text.text = user.DisplayName;
                this.userTextOjbect.SetActive(true);
                this.affiliateTextObject.SetActive(!Plugin.Instance.twitchClient.HasChannelPoints());
                this.rewardsSettingsButtonObject.SetActive(true);
                this.loginButtonObject.SetActive(false);
                this.logoutButtonObject.SetActive(true);
            }
            else
            {
                this.userTextOjbect.SetActive(false);
                this.affiliateTextObject.SetActive(false);
                this.rewardsSettingsButtonObject.SetActive(false);
                this.loginButtonObject.SetActive(true);
                this.logoutButtonObject.SetActive(false);
            }
        }


        private void LoginToTwitch()
        {
            if (Plugin.Instance.GetUser() == null)
            {
                Plugin.Instance.TwitchAuth();
                this.RefreshMainPanel();
            }
        }

        private void LogoutFromTwitch()
        {
            if (Plugin.Instance.GetUser() != null)
            {
                PluginConfig.DeleteKey("twitchAuthToken");
                Plugin.Instance.twitchClient.user = null;
                this.RefreshMainPanel();
            }
        }

        private void ShowRewardsSettings()
        {
            if (this.server == null)
            {
                this.server = new SettingsServer();
            }
            this.server.OpenSettings();
        }
    }
}
