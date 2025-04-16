using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using System.Collections.Generic;
using PlazmaGames.Attribute;
using DC2025.Utils;
using static UnityEditor.Rendering.CameraUI;
using UnityEngine.UI;

namespace DC2025
{
    public class ForgeView : View
    {
        [Header("Forge Interface")]
        [SerializeField] private List<InventorySlot> _input;
        [SerializeField] private InventorySlot _output;
        [SerializeField] private EventButton _start;

        [Header("Mini-Game Interface")]
        [SerializeField] private GameObject _miniContainer;
        [SerializeField] private GameObject _ratingContainer;
        [SerializeField] private List<GameObject> _stars;
        [SerializeField] private Image _tempProg;
        [SerializeField] private RectTransform _timeProg;
        [SerializeField] private EventButton _fanFlame;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private bool _isStarted = false;
        [SerializeField, ReadOnly] private Forge _currentForge;
        [SerializeField, ReadOnly] private int _currentRating = 4;

        private GenericView _generic;

        public bool IsStarted() => _isStarted;
        public void SetForge(Forge forge) => _currentForge = forge;

        public void OnTempertureRangeChange(bool isInRange)
        {
            _tempProg.color = (isInRange) ? Color.green : Color.red;
        }

        public void UpdateProgress(float prog)
        {
            _timeProg.localScale = _timeProg.localScale.SetY(Mathf.Clamp01(prog));
        }

        public void RemoveStar()
        {
            if (_currentRating <= 3) return;
            _stars[_currentRating - 1].SetActive(false);
            _currentRating -= 1;
        }

        public void UpdateTemperture(float prog)
        {
            _tempProg.transform.localScale = _tempProg.transform.localScale.SetY(Mathf.Clamp01(prog));
        }

        public bool CanStartForge()
        {
            return (_input[0].Item is RawCraftingItem) && 
                (_input[1].Item is RawCraftingItem) && 
                (_input[0].Item as MaterialItem).GetMaterial() == (_input[1].Item as MaterialItem).GetMaterial();
        }

        public void StartForge()
        {
            _isStarted = true;
            _currentRating = 4;
            _start.IsDisabled = true;
            _input[0].ToogleDisableState(true);
            _input[1].ToogleDisableState(true);
            _generic.ToggleInventory(false, false);
            _miniContainer.SetActive(true);
            _ratingContainer.SetActive(true);

            OnTempertureRangeChange(true);

            ReturnOrDestoryOutputItem();
            _currentForge.StartForge();
        }

        public void StopForge()
        {
            _isStarted = false;

            CreateBucket();
            ClearInputSlots();
            UpdateForgeState();

            _input[0].ToogleDisableState(false);
            _input[1].ToogleDisableState(false);
            _generic.ToggleInventory(true, false);
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
        }

        private void ClearInputSlots()
        {
            _input[0].Item.Release();
            _input[1].Item.Release();
            _input[0].Clear();
            _input[1].Clear();
        }

        private void UpdateForgeState()
        {
            _start.IsDisabled = !CanStartForge();
        }

        private void CreateBucket()
        {
            BucketItem bucket = Instantiate(Resources.Load<BucketItem>("Prefabs/Items/BucketItem"));
            bucket.SetMaterial((_input[0].Item as RawCraftingItem).GetMaterial());
            bucket.SetRating(_currentRating);
            bucket.ForceInit();
            _output.UpdateSlot(bucket);
        }

        private void ReturnOrDestoryOutputItem()
        {
            if (_output.Item != null)
            {
                _output.ToogleDisableState(true);

                if (!GameManager.GetMonoSystem<IInventoryMonoSystem>().AddItemToInventory(_output.Item))
                {
                    _output.Item.Release();
                    _output.Clear();
                }
            }
        }

        private void FanFlame()
        {
            _currentForge.FanFlame();
        }

        private void CheckForOutputDisable()
        {
            _output.ToogleDisableState(_output.Item == null);
        }

        public override void Init()
        {
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            UpdateForgeState();
            CheckForOutputDisable();
            _input[0].OnChange.AddListener(UpdateForgeState);
            _input[1].OnChange.AddListener(UpdateForgeState);
            _output.OnChange.AddListener(CheckForOutputDisable);
            _start.onPointerDown.AddListener(StartForge);

            _input[0].ToogleDisableState(false);
            _input[1].ToogleDisableState(false);
            _output.ToogleDisableState(true);

            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);

            _fanFlame.onPointerDown.AddListener(FanFlame);
        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = true;
            _generic?.ToggleInventory(!_isStarted, false);
        }

        public override void Hide()
        {
            base.Hide();
            _generic.ToggleInventory(false, true);
        }
    }
}
