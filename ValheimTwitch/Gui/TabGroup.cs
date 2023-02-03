using System.Collections.Generic;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.Gui
{
    class TabGroup
    {
        public readonly List<TabPanel> Panels = new List<TabPanel>();
        public readonly Transform parent;
        public readonly float width;
        public readonly float height;
        public readonly float anchorX;
        public readonly float anchorY;
        private readonly GameObject panel;
        private readonly GameObject buttonContainer;

        public TabGroup(Transform parent, float width, float height, float anchorX, float anchorY)
        {
            this.parent = parent;
            this.width = width;
            this.height = height;
            this.anchorX = anchorX;
            this.anchorY = anchorY;
            this.panel = GUIManager.Instance.CreateWoodpanel(
                parent: parent,
                anchorMin: new Vector2(0f, 0.66f),
                anchorMax: new Vector2(0.33f, 1f),
                position: new Vector2(0f, 0f),
                width: width,
                height: height,
                draggable: true);
            this.buttonContainer = new GameObject();
            this.buttonContainer.transform.SetParent(this.panel.transform);
            this.Redraw();
        }

        public void Redraw()
        {
            this.buttonContainer.transform.DetachChildren();
            float tabButtonWidth = (float) this.width / this.Panels.Count;
            float tabButtonHeight = 30f;
            for (int i = 0; i < this.Panels.Count; i++)
            {
                TabPanel panel = this.Panels[i];
                var button = GUIManager.Instance.CreateButton(
                    panel.Title, this.panel.transform,
                    new Vector2(0f, 1f), new Vector2(0f, 1f),
                    new Vector2(tabButtonWidth / 2, -tabButtonHeight / 2),
                    tabButtonWidth, tabButtonHeight);
                Log.Info($"button position {button.transform.localPosition}");
            }
        }

        private void SetButtonColor(GameObject button, bool isActive)
        {
            Text txt = button.GetComponentInChildren<Text>(includeInactive: true);
            if (txt)
            {
                txt.color = isActive ? GUIManager.Instance.ValheimOrange : Color.gray;
            }
        }
    }
}
