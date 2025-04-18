using System.Collections.Generic;
using Newtonsoft.Json.Schema;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DC2025
{
    public class AnvilView : View
    {
        [SerializeField] private GameObject _miniContainer;
        [SerializeField] private GameObject _ratingContainer;
        [SerializeField] private Transform _progressBar;
        [SerializeField] private InventorySlot _input;
        [SerializeField] private InventorySlot _output;
        [SerializeField] private List<GameObject> _stars;
        [SerializeField] private EventButton _hammerButton;
        [SerializeField] private EventButton _startButton;
        [SerializeField] private TimingBar _timingBar;
        
        private GenericView _generic;
        
        private bool _isStarted = false;
        private Anvil _anvil;
        private float _stopTime;
        private int _progress = 0;
        private bool _tookStar = false;
        private int _currentRating;
        private IChatWindowMonoSystem _chatMs;

        public bool IsStarted() => _isStarted;
        public void SetAnvil(Anvil anvil) => _anvil = anvil;
        
        public override void Init()
        {
            _chatMs = GameManager.GetMonoSystem<IChatWindowMonoSystem>();
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            _hammerButton.onPointerDown.AddListener(HitHammer);
            _startButton.onPointerDown.AddListener(StartAnvil);
            
            _input.OnChange.AddListener(UpdateState);
            _input.ToogleDisableState(false);
            _output.OnChange.AddListener(CheckForOutputDisable);
            _output.ToogleDisableState(true);
            
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
            
        }

        private void CheckForOutputDisable()
        {
            _output.ToogleDisableState(_output.Item == null);
        }

        private void StartAnvil()
        {
            _chatMs.Send($"You place the unfinished {GetMaterial()} {GetBlade()} blade onto to anvil and ready your hammer.");
            _progress = 0;
            _tookStar = false;
            _isStarted = true;
            _currentRating = (_input.Item as UnfBladeItem).GetRating();
            for (int i = 4 - 1; i >= 0; i--) _stars[i].SetActive(i <= _currentRating - 1);
            _startButton.IsDisabled = true;
            _input.ToogleDisableState(true);
            _generic.ToggleInventory(false, false);
            _miniContainer.SetActive(true);
            _ratingContainer.SetActive(true);
            SetProgressBar();

            ReturnOrDestroyOutputItem();
            //_anvil.StartAnvil();
        }

        private MaterialType GetMaterial()
        {
            return (_input.Item as UnfBladeItem).GetMaterial();
        }
        private BladeType GetBlade()
        {
            return (_input.Item as UnfBladeItem).bladeType;
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
        
        public void RemoveStar()
        {
            if (_tookStar) return;
            _tookStar = true;
            _stars[--_currentRating].SetActive(false);
            _chatMs.Send($"You swung too hard and cracked the blade.");
        }

        private void HitHammer()
        {
            if (_timingBar.IsStopped()) return;
            if (_timingBar.Stop())
            {
                _progress += 1;
                SetProgressBar();
            }
            else
            {
                RemoveStar();
            }

            _stopTime = Time.time;
        }

        private void SetProgressBar()
        {
            _progressBar.localScale = new Vector3(_progressBar.localScale.x, (float)_progress / DCGameManager.settings.anvilHammerCount, _progressBar.localScale.z);
        }

        private void Finish()
        {
            _chatMs.Send($"The {GetMaterial()} {GetBlade()} blade is now fully formed.");
            _isStarted = false;

            CreateBlade();
            ClearInputSlots();
            UpdateState();

            _input.ToogleDisableState(false);
            _generic.ToggleInventory(true, false);
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
        }

        private bool CanStart() => _input.Item is UnfBladeItem;

        private void UpdateState()
        {
            _startButton.IsDisabled = !CanStart();
        }

        private void ClearInputSlots()
        {
            _input.Item.Release();
            _input.Clear();
        }

        private void CreateBlade()
        {
            UnfBladeItem unfBlade = _input.Item as UnfBladeItem;
            BladeItem blade = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateBlade(unfBlade.bladeType, unfBlade.GetMaterial(), _currentRating);
            _output.UpdateSlot(blade);
        }

        private void FixedUpdate()
        {
            if (_timingBar.IsStopped() && Time.time - _stopTime >= DCGameManager.settings.anvilHammerShowHitTime)
            {
                _timingBar.Reset();
                if (_progress == DCGameManager.settings.anvilHammerCount)
                {
                    Finish();
                }
            }
        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = true;
            _generic?.ToggleInventory(!_isStarted, false);
            UpdateState();
        }

        public override void Hide()
        {
            base.Hide();
            _generic.ToggleInventory(false, true);
            if (_anvil != null && _anvil.IsEnabled) _anvil.Interact();
        }
    }
}
