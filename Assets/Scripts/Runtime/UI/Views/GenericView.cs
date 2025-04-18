using DC2025.Utils;
using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DC2025
{
    public class GenericView : View
    {
        [Header("Chat Window")]
        [SerializeField] private float _scrollRate = 0.05f;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private EventButton _scrollUp;
        [SerializeField] private EventButton _scrollDown;

        [Header("Health")]
        [SerializeField] private RectTransform _healthBar;
        [SerializeField, ReadOnly] private Vector2 _healthRectRange;

        [Header("Stamina")]
        [SerializeField] private RectTransform _staminaBar;
        [SerializeField, ReadOnly] private Vector2 _staminaRectRange;


        [Header("Inventory")]
        [SerializeField] private GameObject _inv;
        [SerializeField] private GameObject _invHolder;
        [SerializeField] private GameObject _headerMain;
        [SerializeField] private GameObject _headerSecondary;
        [SerializeField] private EventButton _openInv;
        [SerializeField] private EventButton _backFromInv;
        [SerializeField] private EventButton _backFromStation;

        [Header("Stats")]
        [SerializeField] private GameObject _stats;
        [SerializeField] private EventButton _pause;

        private PlayerManager _playerManager;

        [SerializeField, ReadOnly] private int _scrollDir = 0;

        IEnumerator ChangeScrollRect(float val)
        {
            yield return new WaitForEndOfFrame();
            _container.verticalNormalizedPosition = val;
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

        public void ScrollToBottom()
        {
            StartCoroutine(ChangeScrollRect(0));
        }

        public void ScrollToTop()
        {
            StartCoroutine(ChangeScrollRect(1));
        }

        public void ToggleInventory(bool status, bool fromGame = false)
        {
            if (fromGame)
            {
                _invHolder.SetActive(true);
                _inv.SetActive(status);
                _headerMain.SetActive(status);
                _headerSecondary.SetActive(false);
                _stats.SetActive(!status);
            }
            else
            {
                _invHolder.SetActive(status);
                _inv.SetActive(true);
                _stats.SetActive(false);
                _headerMain.SetActive(false);
                _headerSecondary.SetActive(true);
            }
        }

        private void OpenInventory()
        {
            IInventoryMonoSystem inventory = GameManager.GetMonoSystem<IInventoryMonoSystem>();

            if (inventory.GetMouseSlot().HasItem())
            {
                if (inventory.AddItemToInventory(inventory.GetMouseSlot().Item))
                {
                    GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.uiClickSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                    inventory.GetMouseSlot().Clear();
                }
            }
            else
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.zipperSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                ToggleInventory(true, true);
            }
        }

        private void CloseStation()
        {
            if (!GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<GameView>() && !GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<MainMenuView>())
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
            }
        }

        private void Pause()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<PauseMenuView>();
        }

        public override void Init()
        {
            _scrollDown.onPointerDown.AddListener(() => _scrollDir = -1);
            _scrollDown.onPointerUp.AddListener(() => _scrollDir = 0);
            _scrollUp.onPointerDown.AddListener(() => _scrollDir = 1);
            _scrollUp.onPointerUp.AddListener(() => _scrollDir = 0);

            _healthRectRange.y = 0;
            _healthRectRange.y = _healthBar.sizeDelta.y;

            _staminaRectRange.y = 0;
            _staminaRectRange.y = _staminaBar.sizeDelta.y;

            ToggleInventory(false);
            _openInv.onPointerDown.AddListener(OpenInventory);
            _openInv.ToggleSound(false);
            _backFromInv.onPointerDown.AddListener(() => ToggleInventory(false, true));
            _backFromStation.onPointerDown.AddListener(CloseStation);

            _pause.onPointerDown.AddListener(Pause);

        }

        public override void Hide() { }

        private void Scroll(int dir)
        {
            if (dir == 0) return;
            float height = _container.content.sizeDelta.y;
            float shift = _scrollRate * dir * Time.deltaTime;
            float val = Mathf.Clamp01(_container.verticalNormalizedPosition + shift / height);
            StartCoroutine(ChangeScrollRect(val));
        }

        private void Update() 
        {
            Scroll(_scrollDir);

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseStation();
            }
        }
    }
}
