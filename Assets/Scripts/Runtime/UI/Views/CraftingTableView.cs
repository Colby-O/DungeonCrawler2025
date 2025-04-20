using System.Collections.Generic;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class CraftingTableView : View
    {
        [SerializeField] private EventButton _craftButton;
        [SerializeField] private InventorySlot _input;
        [SerializeField] private InventorySlot _output;
        
        [SerializeField] private EventButton _nextHandle;
        [SerializeField] private EventButton _prevHandle;
        [SerializeField] private Image _handleIcon;
        [SerializeField] private HandleIcon _handleIconHover;

        private Dictionary<HandleType, Sprite> _handleIcons = new();

        private int _selectedHandle = 0;
        
        private GenericView _generic;
        private IChatWindowMonoSystem _chatMs;

        private CraftingTable _currentTable;

        public void SetTable(CraftingTable table) => _currentTable = table;

        public override void Init()
        {
            _handleIcons.Add(HandleType.Balanced, Resources.Load<Sprite>("Icons/Handles/Balanced"));
            _handleIcons.Add(HandleType.Dominant, Resources.Load<Sprite>("Icons/Handles/Dominant"));
            _handleIcons.Add(HandleType.Lightweight, Resources.Load<Sprite>("Icons/Handles/Lightweight"));
            _handleIcons.Add(HandleType.Rugged, Resources.Load<Sprite>("Icons/Handles/Rugged"));
            _handleIcons.Add(HandleType.Wise, Resources.Load<Sprite>("Icons/Handles/Wise"));
            
            UpdateHandleSlot();
            
            _chatMs = GameManager.GetMonoSystem<IChatWindowMonoSystem>();
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            _craftButton.onPointerDown.AddListener(Craft);
            _input.OnChange.AddListener(UpdateState);
            _output.OnChange.AddListener(CheckForOutputDisable);
            CheckForOutputDisable();
            _nextHandle.onPointerDown.AddListener(NextHandle);
            _prevHandle.onPointerDown.AddListener(PrevHandle);
        }

        private void PrevHandle()
        {
            _selectedHandle = (_selectedHandle + HandleTypeExt.HandleCount() - 1) % HandleTypeExt.HandleCount();
            UpdateHandleSlot();
        }

        private void NextHandle()
        {
            _selectedHandle = (_selectedHandle + 1) % HandleTypeExt.HandleCount();
            UpdateHandleSlot();
        }
        
        private void UpdateHandleSlot()
        {
            _handleIconHover._handle = (HandleType)_selectedHandle;
            _handleIcon.sprite = _handleIcons[(HandleType)_selectedHandle];
        }

        private void CheckForOutputDisable()
        {
            _output.ToogleDisableState(_output.Item == null);
        }

        private void UpdateState()
        {
            _craftButton.IsDisabled = !CanCraft();
        }

        private bool CanCraft() => _input.Item is BladeItem;

        private void Craft()
        {
            if (!CanCraft()) return;
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.craftSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
            _chatMs.Send($"You insert the <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()} {GetBlade()}</color> blade into the {GetHandle()} handle forming a complete weapon.");
            BladeItem blade = _input.Item as BladeItem;
            Debug.Log($"blade mat: {blade.GetMaterial()}");
            WeaponItem weapon = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateSword(blade.bladeType, (HandleType)_selectedHandle, blade.GetMaterial(), blade.GetRating());
            _output.UpdateSlot(weapon);
            _input.Item.Release();
            _input.Clear();
            UpdateState();
            Debug.Log($"wep: {weapon.GetMaterial()}");
        }

        private HandleType GetHandle() => (HandleType)_selectedHandle;
        
        private MaterialType GetMaterial() => (_input.Item as BladeItem).GetMaterial();
        private BladeType GetBlade() => (_input.Item as BladeItem).bladeType;

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = true;
            _generic?.ToggleInventory(true, false);
            UpdateState();
        }
        
        public override void Hide()
        {
            base.Hide();
            _generic.ToggleInventory(false, true);

            if (_currentTable != null && _currentTable.IsEnabled) _currentTable.Interact();
        }
    }
}
