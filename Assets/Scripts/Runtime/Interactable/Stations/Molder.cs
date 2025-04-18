using System.Collections.Generic;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

namespace DC2025
{
    public class Molder : Station
    {
        private MolderView _view;

        [SerializeField] private float _temp = 1;
        [SerializeField, ReadOnly] private float _low;
        [SerializeField, ReadOnly] private float _high;
        private Transform _currentMold = null;
        private Transform _moldSlot;

        private Dictionary<BladeType, GameObject> _molds = new();

        private float _moldAniTargetY = 0;

        private bool _lowering = false;
        private bool _done = false;
        private float _doneTime;
        private bool _inRange = false;

        public void Lower()
        {
            _lowering = true;   
            _moldAniTargetY = -0.2f;
        }

        public void Raise()
        {
            if (_done) return;
            _done = true;
            _doneTime = Time.time;
            _moldAniTargetY = 0;
        }
        
        private void Start()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<MolderView>();
            _view.SubscribeMoldChange(MoldChange);
            _view.SubscribeMaterialChange(MaterialChange);
            _moldSlot = transform.Find("MoldSlot");
            _molds.Add(BladeType.Axe, Resources.Load<GameObject>("Prefabs/Molds/Axe"));
            _molds.Add(BladeType.BattleAxe, Resources.Load<GameObject>("Prefabs/Molds/BattleAxe"));
            _molds.Add(BladeType.LongSword, Resources.Load<GameObject>("Prefabs/Molds/LongSword"));
            _molds.Add(BladeType.ShortSword, Resources.Load<GameObject>("Prefabs/Molds/ShortSword"));
            _molds.Add(BladeType.Dagger, Resources.Load<GameObject>("Prefabs/Molds/Dagger"));
        }

        private bool HasMaterial()
        {
            if (!_currentMold) return false;
            return _currentMold.Find("Liquid").gameObject.activeSelf;
        }

        private void MaterialChange()
        {
            if (!_currentMold) return;
            
            BucketItem bucket = _view.GetBucket();
            if (!bucket && HasMaterial())
            {
                Transform liquid = _currentMold.Find("Liquid");
                liquid.gameObject.SetActive(false);
            }
            else if (bucket && !HasMaterial())
            {
                Transform liquid = _currentMold.Find("Liquid");
                liquid.gameObject.SetActive(true);
                liquid.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", DCGameManager.settings.materialColors[_view.GetMaterial()]);
            }
        }

        private void MoldChange()
        {
            MoldItem mold = _view.GetMold();
            if (!mold && _currentMold)
            {
                Destroy(_currentMold.gameObject);
                _currentMold = null;
            }
            else if (mold && !_currentMold)
            {
                _currentMold = Instantiate(_molds[mold.bladeType], _moldSlot).transform;
                MaterialChange();
            }
        }

        private void Update()
        {
            if (!_view.IsStarted()) return;

            if (_currentMold)
            {
                Vector3 newPos = _currentMold.transform.localPosition;
                newPos.y = Mathf.Lerp(newPos.y, _moldAniTargetY, Time.deltaTime * DCGameManager.settings.moldMoveSpeed);
                _currentMold.transform.localPosition = newPos;
            }

            if (_done && Time.time - _doneTime >= DCGameManager.settings.molderTimeToWin)
            {
                if (!InRange()) _view.RemoveStar(_temp <= _low);
                _view.StopMolder();
            }
            else if (_lowering && !_done)
            {
                _temp -= DCGameManager.settings.molderTempDropRate * Time.deltaTime;
                if (_temp < 0) _temp = 0;
                if (_temp < _low) _view.RemoveStar(true);
                _view.SetTemperaturePosition(_temp);
                if (_inRange != InRange())
                {
                    _inRange = InRange();
                    _view.SetTemperatureColor(_inRange);
                }
            }
        }

        private bool InRange() => _temp >= _low && _temp <= _high;
        
        public override void Interact()
        {
            if (_view.IsStarted()) return;
            IsEnabled = !IsEnabled;
            StartTransition();
            if (IsEnabled)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<MolderView>();
                _view.SetMolder(this);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetMolder(null);
                OnClose();
            }
        }

        public override void ForceClose()
        {
             if (IsEnabled)
            {
                IsEnabled = false;
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetMolder(null);
                OnClose();
            }
        }

        public void StartMolder(float low, float high)
        {
            _low = low;
            _high = high;
            _temp = 1;
            _done = false;
            _lowering = false;
            _inRange = false;
        }
    }
}
