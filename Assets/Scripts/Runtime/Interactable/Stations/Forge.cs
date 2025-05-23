using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class Forge : Station
    {
        [Header("Particle System")]
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private int _normalSpawnRate = 100;
        [SerializeField] private float _normalLife = 0.4f;
        [SerializeField] private int _fanSpawnRate = 200;
        [SerializeField] private float _fanLife = 0.6f;

        [Header("Settings")]
        [SerializeField, Min(0)] private float _graceTime = 5;
        [SerializeField, Range(0, 1)] private float _startTemperture = 0.8f;
        [SerializeField, Min(0)] private float _accelerationWhileDown = 0.01f;
        [SerializeField, Min(0)] private float _accelerationWhileUp = 0.02f;
        [SerializeField, Min(0)] private float _fanForce = 0.03f;
        [SerializeField, Range(0, 1)] private float _outOfRangeAllowed;
        [SerializeField] private Vector2 _allowedRange = new Vector2(0.60f, 0.77f);

        [Header("Debugging")]
        [SerializeField, ReadOnly] private ForgeView _view;
        [SerializeField, ReadOnly] private float _timerOn = 0;
        private float _fullTime = 0;
        [SerializeField, ReadOnly] private float _maxOutOfRangeTime = 0;
        [SerializeField, ReadOnly] private float _outOfRangeTime = 0;
        [SerializeField, ReadOnly] private bool _isStarted = false;
        [SerializeField, ReadOnly] private float _temperture = 0.0f;
        [SerializeField, ReadOnly] private float _vel;

        private ParticleSystem.MainModule _psMain;
        private ParticleSystem.EmissionModule _psEmission;

        private IChatWindowMonoSystem _chatMs;

        public override void Interact()
        {
            if (_view.IsStarted()) return;

            IsEnabled = !IsEnabled;
            StartTransition();

            if (IsEnabled)
            {
                if (!GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<GameView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<ForgeView>();
                _view.SetForge(this);
            }
            else
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<ForgeView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetForge(null);
                OnClose();
            }
        }

        public override void ForceClose()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<ForgeView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetForge(null);
                OnClose();
            }
        }

        public void StartForge()
        {
            _isStarted = true;
            _timerOn = 0;
            _fullTime = 0;
            _temperture = _startTemperture;
            _maxOutOfRangeTime = DCGameManager.settings.forgeCookTime * _outOfRangeAllowed;
            _outOfRangeTime = 0;
            _view.UpdateProgress(1f);
            _view.UpdateTemperture(_temperture);
            _vel = 0;
        }

        public void ForgetSetp()
        {
            _fullTime += Time.deltaTime;
            if (_graceTime > _fullTime) return;
            
            _timerOn += Time.deltaTime;


            _vel -= ((_vel > 0) ? _accelerationWhileUp : _accelerationWhileDown) * Time.deltaTime;
            _temperture += _vel * Time.deltaTime;

            if (_vel > 0)
            {
                _psMain.startLifetime = _fanLife;
                _psEmission.rateOverTime = _fanSpawnRate;
            }
            else
            {
                _psMain.startLifetime = _normalLife;
                _psEmission.rateOverTime = _normalSpawnRate;
            }

            bool isInRange = _temperture > _allowedRange.y || _temperture < _allowedRange.x;
            _view.OnTempertureRangeChange(!isInRange);

            if (isInRange)
            {
                _outOfRangeTime += Time.deltaTime;

                if (_outOfRangeTime > _maxOutOfRangeTime)
                {
                    _view.RemoveStar(_temperture < _allowedRange.x);
                }
            }

            _view.UpdateProgress(1f - _timerOn / DCGameManager.settings.forgeCookTime);
            _view.UpdateTemperture(_temperture);

            if (_timerOn > DCGameManager.settings.forgeCookTime)
            {
                StopForge();
            }
        }

        public void FanFlame()
        {
            if (_graceTime > _fullTime) return;
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.fanFireSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
            _vel = _fanForce;
        }

        public void StopForge()
        {
            _isStarted = false;
            _view.StopForge();
            _psMain.startLifetime = _normalLife;
            _psEmission.rateOverTime = _normalSpawnRate;
        }

        private void Start()
        {
            _chatMs = GameManager.GetMonoSystem<IChatWindowMonoSystem>();
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<ForgeView>();
            _psMain = _particleSystem.main;
            _psEmission = _particleSystem.emission;
            _psMain.startLifetime = _normalLife;
            _psEmission.rateOverTime = _normalSpawnRate;

        }

        private void Update()
        {
            if (_view.IsStarted())
            {
                ForgetSetp();
            }
        }
    }
}
