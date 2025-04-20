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

            Send(
                $"Welcome, traveller! Your quest is to steal four keys forged from " +
                $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[MaterialType.Bronze])}>{MaterialType.Bronze}</color>, " +
                $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[MaterialType.Iron])}>{MaterialType.Iron}</color>, " +
                $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[MaterialType.Steel])}>{MaterialType.Steel}</color> and, " +
                $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[MaterialType.Cobalt])}>{MaterialType.Cobalt}</color>" +
                " to unlock the sealed door within your Forge. I suggest you prepare yourself well before entering.", 
                Color.green);

            Send(
                $"The Forge is your safe haven. Here, you can use its facilities to craft your own weapons and potions from powerful minerals. " +
                $"To obtain these minerals, you'll need to steal them from the dungeon's inhabitants. Chests in the dungeon replenish after each visted to the Forge." +
                $" You can return to the Forge throughout your journey to safely store items in your personal chests and, of course, to forge weapons! ",
                Color.green);

            Send(
                $"Beware when venturing into the dungeon as it's inhabitants are not to be trifled with. You'll need to forge a weapon with their mineral before you'll stand a chance.",
                Color.green);

            Send(
                $"Also, try to catch any chickens you find. They might just come in handy!",
                Color.green);

            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().ScrollToTop();
        }
    }
}
