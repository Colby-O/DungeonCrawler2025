using DC2025.Utils;
using NUnit.Framework.Internal.Filters;
using PlazmaGames.Attribute;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class GameView : View
    {
        [Header("Movement Buttons")]
        [SerializeField] private List<EventButton> _moveButtons;

        [Header("Health")]
        [SerializeField] private RectTransform _healthBar;
        [SerializeField, ReadOnly] private Vector2 _healthRectRange;

        [Header("Stamina")]
        [SerializeField] private RectTransform _staminaBar;
        [SerializeField, ReadOnly] private Vector2 _staminaRectRange;


        [Header("Inventory")]
        [SerializeField] private GameObject _inv;
        [SerializeField] private EventButton _openInv;
        [SerializeField] private EventButton _backFromInv;

        [Header("Stats")]
        [SerializeField] private GameObject _stats;

        private Player _player;
        private PlayerManager _playerManager;

        public void ForceHighlighted(Action action, bool isHelighted)
        {
            _moveButtons[(int)action].ForceHighlightedt(isHelighted);
        }

        public void UpdateHealth()
        {
            if (_playerManager == null) _playerManager = DCGameManager.Player.GetComponent<PlayerManager>();

            _healthBar.sizeDelta = new Vector2(_healthBar.sizeDelta.x, Math.Map(_playerManager.GetHealth(), 0, _playerManager.GetMaxHealth(), _healthRectRange.x, _healthRectRange.y));
        }

        public void UpdateStamina()
        {
            if (_playerManager == null) _playerManager = DCGameManager.Player.GetComponent<PlayerManager>();

            _staminaBar.sizeDelta = new Vector2(_staminaBar.sizeDelta.x, Math.Map(_playerManager.GetStamina(), 0, _playerManager.GetMaxStamina(), _staminaRectRange.x, _staminaRectRange.y));
        }

        private void ApplyAction(Action action, bool state)
        {
            if (_player == null) _player = DCGameManager.Player.GetComponent<Player>();

            switch (action)
            {
                case Action.MoveUp:
                    _player.SetRawMovement((state) ? new Vector2(0, 1) : Vector2.zero);
                    break;
                case Action.MoveDown:
                    _player.SetRawMovement((state) ? new Vector2(0, -1) : Vector2.zero);
                    break;
                case Action.MoveLeft:
                    _player.SetRawMovement((state) ? new Vector2(-1, 0) : Vector2.zero);
                    break;
                case Action.MoveRight:
                    _player.SetRawMovement((state) ? new Vector2(1, 0) : Vector2.zero);
                    break;
                case Action.TurnLeft:
                    _player.SetRawTurn((state) ? -1 : 0);
                    break;
                case Action.TurnRight:
                    _player.SetRawTurn((state) ? 1 : 0);
                    break;
                default:
                    break;
            }
        }

        private void ToggleInventory(bool status)
        {
            _inv.SetActive(status);
            _stats.SetActive(!status);
        }

        public override void Init()
        {
            _healthRectRange.y = 0;
            _healthRectRange.y = _healthBar.sizeDelta.y;

            _staminaRectRange.y = 0;
            _staminaRectRange.y = _staminaBar.sizeDelta.y;

            if (_moveButtons != null) 
            { 
                for (int i = 0; i < _moveButtons.Count; i++)
                {
                    Action action = (Action)i;
                    _moveButtons[i].onPointerDown.AddListener(() => ApplyAction(action, true));
                    _moveButtons[i].onPointerUp.AddListener(() => ApplyAction(action, false));
                }
            }

            ToggleInventory(false);
            _openInv.onPointerDown.AddListener(() => ToggleInventory(true));
            _backFromInv.onPointerDown.AddListener(() => ToggleInventory(false));
        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = false;
        }
    }
}
