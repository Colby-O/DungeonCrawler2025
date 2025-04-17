using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class CraftingTableView : View
    {
        [SerializeField] private EventButton _craftButton;
        [SerializeField] private InventorySlot _input;
        [SerializeField] private InventorySlot _output;
        
        [SerializeField] private EventButton _nextHandle;
        [SerializeField] private EventButton _prevHandle;

        private int _selectedHandle = 0;
        
        private GenericView _generic;
        private IChatWindowMonoSystem _chatMs;

        public override void Init()
        {
            _chatMs = GameManager.GetMonoSystem<IChatWindowMonoSystem>();
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            _craftButton.onPointerDown.AddListener(Craft);
            _input.OnChange.AddListener(UpdateState);
            _output.OnChange.AddListener(CheckForOutputDisable);
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
            _chatMs.Send($"You insert the {GetMaterial()} {GetBlade()} blade into the {GetHandle()} handle forming a complete weapon.");
            BladeItem blade = _input.Item as BladeItem;
            WeaponItem weapon = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateSword(blade.bladeType, HandleType.Balanced, blade.GetMaterial(), blade.GetRating());
            _output.UpdateSlot(weapon);
            _input.Item.Release();
            _input.Clear();
            UpdateState();
        }

        private HandleType GetHandle() => HandleType.Balanced;
        
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
        }
    }
}
