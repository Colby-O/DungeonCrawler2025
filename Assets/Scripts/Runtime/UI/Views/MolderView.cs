using System.Collections.Generic;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class MolderView : View
    {
        [SerializeField] private List<InventorySlot> _input;
        [SerializeField] private InventorySlot _output;
        [SerializeField] private EventButton _start;
        [SerializeField] private List<GameObject> _stars;
        
        [SerializeField] private GameObject _miniContainer;
        [SerializeField] private GameObject _ratingContainer;
        [SerializeField] private EventButton _waterButton;
        [SerializeField] private Image _tempProg;
        [SerializeField] private Transform _tickTop;
        [SerializeField] private Transform _tickBottom;
        [SerializeField] private Transform _tickLow;
        [SerializeField] private Transform _tickHigh;

        private float _tickLowNrml;
        private float _tickHighNrml;
        
        private GenericView _generic;
        private Molder _molder;
        private bool _isStarted = false;
        private int _currentRating = 4;
        private bool _tookStar = false;
        
        private IChatWindowMonoSystem _chatMs;

        public bool IsStarted() => _isStarted;
        
        public void SetMolder(Molder m) => _molder = m;
        
        public MoldItem GetMold() => _input[1].Item as MoldItem;
        public BucketItem GetBucket() => _input[0].Item as BucketItem;

        public void RemoveStar(bool tooLow)
        {
            if (_tookStar) return;
            _tookStar = true;
            _stars[--_currentRating].SetActive(false);
            string cause = tooLow ? "cooled too much" : "is still too hot";
            _chatMs.Send($"The molten <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()}</color> {cause}.");
        }

        public MaterialType GetMaterial()
        {
            return (_input[0].Item as BucketItem).GetMaterial();
        }

        public void SubscribeMoldChange(UnityEngine.Events.UnityAction func)
        {
            _input[1].OnChange.AddListener(func);
        }
        public void SubscribeMaterialChange(UnityEngine.Events.UnityAction func)
        {
            _input[0].OnChange.AddListener(func);
        }

        public override void Init()
        {
            _chatMs = GameManager.GetMonoSystem<IChatWindowMonoSystem>();
            float height = _tickTop.localPosition.y - _tickBottom.localPosition.y;
            _tickLowNrml = 1 - (_tickLow.localPosition.y - _tickBottom.localPosition.y) / height;
            _tickHighNrml = 1 - (_tickHigh.localPosition.y - _tickBottom.localPosition.y) / height;
            
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            UpdateState();
            CheckForOutputDisable();
            _input[0].OnChange.AddListener(UpdateState);
            _input[1].OnChange.AddListener(UpdateState);
            _output.OnChange.AddListener(CheckForOutputDisable);
            _start.onPointerDown.AddListener(StartMolder);

            _input[0].ToogleDisableState(false);
            _input[1].ToogleDisableState(false);
            _output.ToogleDisableState(true);
            
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
            
            _waterButton.onPointerDown.AddListener(LowerMold);
            _waterButton.onPointerUp.AddListener(RaiseMold);
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

            if (_molder != null && _molder.IsEnabled) _molder.Interact(); 
        }
        
        private void StartMolder()
        {
            _chatMs.Send($"You prepare to dip the {GetMoldType()} mold with the molten <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()}</color> into water.");
            _tookStar = false;
            _isStarted = true;
            _currentRating = (_input[0].Item as BucketItem).GetRating();
            for (int i = 4 - 1; i >= 0; i--) _stars[i].SetActive(i <= _currentRating - 1);
            _start.IsDisabled = true;
            _input[0].ToogleDisableState(true);
            _input[1].ToogleDisableState(true);
            _generic.ToggleInventory(false, false);
            _miniContainer.SetActive(true);
            _ratingContainer.SetActive(true);

            SetTemperatureColor(false);
            SetTemperaturePosition(1);

            ReturnOrDestroyOutputItem();
            _molder.StartMolder(_tickLowNrml, _tickHighNrml);
        }

        private BladeType GetMoldType()
        {
            return (_input[1].Item as MoldItem).bladeType;
        }

        public void StopMolder()
        {
            _chatMs.Send($"You peel the unfinished <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()} {GetMoldType()}</color> blade from the mold.");
            _isStarted = false;

            CreateBlade();
            ClearInputSlots();
            UpdateState();

            _input[0].ToogleDisableState(false);
            _input[1].ToogleDisableState(false);
            _generic.ToggleInventory(true, false);
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
        }
        
        private void CreateBlade()
        {
            BucketItem bucket = _input[0].Item as BucketItem;
            MoldItem mold = _input[1].Item as MoldItem;
            UnfBladeItem blade = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateUnfBlade(mold.bladeType, bucket.GetMaterial(), _currentRating);
            _output.UpdateSlot(blade);
        }
        
        private void ClearInputSlots()
        {
            _input[0].Item.Release();
            _input[0].Clear();
        }
        
        private void ReturnOrDestroyOutputItem()
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

        private void LowerMold()
        {
            _molder.Lower();
        }

        private void RaiseMold()
        {
            _molder.Raise();
        }

        private void CheckForOutputDisable()
        {
            _output.ToogleDisableState(_output.Item == null);
        }
        
        private bool CanStart() => _input[0].Item is BucketItem && _input[1].Item is MoldItem;

        private void UpdateState()
        {
            _start.IsDisabled = !CanStart();
        }

        public void SetTemperatureColor(bool good)
        {
            _tempProg.color = good ? Color.green : Color.red;
        }

        public void SetTemperaturePosition(float temp)
        {
            _tempProg.transform.localScale = new Vector3(_tempProg.transform.localScale.x, temp, _tempProg.transform.localScale.z);
        }
    }
}
