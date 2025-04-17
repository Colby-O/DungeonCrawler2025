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

        public bool IsStarted() => _isStarted;
        public void SetAnvil(Anvil anvil) => _anvil = anvil;
        
        public override void Init()
        {
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            _hammerButton.onPointerDown.AddListener(HitHammer);
            _startButton.onPointerDown.AddListener(StartAnvil);
            
            _input.ToogleDisableState(false);
            _output.ToogleDisableState(true);
            
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
        }

        private void StartAnvil()
        {
            _progress = 0;
            _tookStar = false;
            _isStarted = true;
            _currentRating = (_input.Item as BladeItem).GetRating();
            for (int i = 4 - 1; i >= 0; i--) _stars[i].SetActive(i <= _currentRating - 1);
            _startButton.IsDisabled = true;
            _input.ToogleDisableState(true);
            _generic.ToggleInventory(false, false);
            _miniContainer.SetActive(true);
            _ratingContainer.SetActive(true);

            ReturnOrDestroyOutputItem();
            //_anvil.StartAnvil();
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
        }

        private void HitHammer()
        {
            if (_timingBar.Stop())
            {
                _progress += 1;
                _progressBar.localScale = new Vector3(_progressBar.localScale.x, (float)_progress / DCGameManager.settings.anvilHammerCount, _progressBar.localScale.z);
            }
            else
            {
                RemoveStar();
            }

            _stopTime = Time.time;
        }

        private void Finish()
        {
            _isStarted = false;

            //CreateBlade();
            //ClearInputSlots();
            //UpdateState();

            _input.ToogleDisableState(false);
            _generic.ToggleInventory(true, false);
            _miniContainer.SetActive(false);
            _ratingContainer.SetActive(false);
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
        }

        public override void Hide()
        {
            base.Hide();
            _generic.ToggleInventory(false, true);
        }
    }
}
