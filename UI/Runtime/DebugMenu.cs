using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DebugMenu.UI.Runtime
{
    public class DebugMenu : MonoBehaviour
    {
        #region Exposed

        public static List<Button> m_menuDebugButton = new List<Button>();
        public int m_depth;
        [SerializeField]
        private Text _headerTitle;
        [SerializeField]
        private RectTransform _backgroundMenu;
        [SerializeField]
        private RectTransform _prefabButton;
        [SerializeField]
        private RectTransform _parentMenuButton;
        [SerializeField]
        private RectTransform _mask;
        [SerializeField]
        private float _textSpacing;

        public List<string> Paths { get; set; }
        public string Title
        {
            get => _headerTitle.text;

            set
            {
                _headerTitle.text = value;
            }
        }

        public string ParentPath
        {
            get => _parent;
            set => _parent = value;
        }

        #endregion Exposed


        #region Unity API

        private void Update()
        {
            ResponsiveMenu();
        }

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        #endregion Unity API


        #region Main

        public void StartGenerate()
        {
            if (_isGenerated) return;
            _isGenerated = true;
            GenerateButton(Paths.ToArray());
        }

        public void ReturnToParent()
        {
            if (ParentPath.Length == 0) return;
            DebugMenuRoot.m_instance.TryDisplayPanel(ParentPath);
        }

        #endregion Main


        #region Utils

        private void ResponsiveMenu()
        {
            var addToList = _parentMenuButton.GetComponentsInChildren<Button>();

            foreach (var element in addToList)
            {
                if (!m_menuDebugButton.Contains(element))
                {
                    m_menuDebugButton.Add(element);
                }
            }

            var sizeMenu = _prefabButton.rect.height * m_menuDebugButton.Count;
            if (m_menuDebugButton.Count < 20)
            {
                _backgroundMenu.sizeDelta = new Vector2(_backgroundMenu.rect.width, _headerTitle.rectTransform.rect.height + sizeMenu + _textSpacing);
                _mask.sizeDelta = new Vector2(_backgroundMenu.rect.width, sizeMenu);
            }
            _parentMenuButton.sizeDelta = new Vector2(_parentMenuButton.rect.width, sizeMenu);
        }

        private void GenerateButton(string[] paths)
        {
            Dictionary<string, string> firstmenu = new Dictionary<string, string>();
            List<string> otherPanel = new List<string>();

            for (int i = 0; i < paths.Length; i++)
            {
                string[] commands = paths[i].Split('/');
                if (!firstmenu.ContainsKey(commands[m_depth + 1]))
                {
                    firstmenu.Add(commands[m_depth + 1], BuildNextPath(commands));
                }

                if (commands.Length - m_depth > 2)
                {
                    otherPanel.Add(paths[i]);
                }
            }
            foreach (var item in firstmenu)
            {
                _prefabButton.GetComponent<Button>().GetComponentInChildren<Text>().text = item.Key;
                _prefabButton.GetComponent<DebugButton>().m_path = item.Value;
                var buttonObject = GameObject.Instantiate(_prefabButton, _parentMenuButton);
                var button = buttonObject.GetComponent<DebugButton>();
                button.Owner = this;
            }

            DebugMenuRoot.m_instance.GeneratePanel(otherPanel, m_depth + 1);
        }

        private string BuildNextPath(string[] paths)
        {
            var result = "";

            for (int i = 0; i <= m_depth + 1; i++)
            {
                if (i != 0)
                {
                    result += "/";
                }
                result += paths[i];
            }

            return result;
        }

        #endregion Utils


        #region Private

        private string _parent;
        private bool _isGenerated;

        #endregion Private
    }
}