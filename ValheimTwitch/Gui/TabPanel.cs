using UnityEngine;

namespace ValheimTwitch.Gui
{
    public delegate void Draw(UnityEngine.Transform parent);
    
    class TabPanel
    {
        public string Title;
        public bool IsActive;
        private GameObject container;
        
        public TabPanel(string title, Draw draw, Transform parent)
        {
            this.Title = title;
            this.IsActive = false;
            this.container = new GameObject();
            this.container.transform.SetParent(parent);
            draw(this.container.transform);
        }
        
        public void SetActive(bool isAcitve)
        {
            this.IsActive = isAcitve;
            this.container.SetActive(isAcitve);
        }
    }
}
