using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DC2025
{
    public class ChatWindowMonoSystem : MonoBehaviour, IChatWindowMonoSystem
    {
        [Header("Settings")]
        [SerializeField] private Color _deafultColor = Color.white;

        private GameObject _container;

        private TMP_Text _chatPrefab;

        [SerializeField] private List<TMP_Text> _log;

        public void Send(string msg, Color? color = null)
        {
            TMP_Text text = Instantiate(_chatPrefab, _container.transform);
            text.text = msg;
            text.color = color ?? _deafultColor;
            _log.Add(text);
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().ScrollToBottom();
        }

        public void Clear()
        {
            foreach (TMP_Text chat in _log) 
            {
                if (chat != null) Destroy(chat.gameObject);
            }

            _log.Clear();
        }

        private void Start()
        {
            _log = new List<TMP_Text>();
            _container = GameObject.FindWithTag("ChatWindowContainer");
            _chatPrefab = Resources.Load<TMP_Text>("Prefabs/ChatElement");

            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
            Send("Welcome!", Color.blue);
        }
    }
}
