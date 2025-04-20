using UnityEngine;
using PlazmaGames.Animation;
using System.Collections.Generic;
using System.Linq;
using DC2025.Utils;
using PlazmaGames.Core;
using UnityEngine.Events;

namespace DC2025
{
	public class Enemy : Entity
    {
        public static bool pause = false;

        private IFightMonoSystem _fightMs;
        private IGridMonoSystem _gridMs;
		[SerializeField] private List<Transform> _path;
		[SerializeField] private bool _loop;
        private bool _attacking = false;
        [SerializeField] private float _attackHintTime = 0.5f;
        [SerializeField] private float _attackAniTime = 0.25f;
        [SerializeField] private float _blockChance = 0.1f;
        [SerializeField] private float _attackIntervalLow = 1.5f;
        [SerializeField] private float _attackIntervalHigh = 3.0f;
        [SerializeField] private MaterialType _materialType = MaterialType.Bronze;
        [SerializeField] private float _health = 100;

        private Animator _animator;
        
        private Transform _healthBar;
        private Transform _healthBarBg;
        private float _healthBarFullSize;

        private Vector3 start;
        private Quaternion startRot;

        private bool _distracted = false;
        private Direction _saveDirection;
        private PathDirector _pathDirector;
        [SerializeField] private int _attackDamage = 20;
        public UnityEvent OnKilled = new UnityEvent();
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

        public float Health() => _health;
        public float AttackIntervalLow() => _attackIntervalLow;
        public float AttackIntervalHigh() => _attackIntervalHigh;

        public float AttackHintTime() => _attackHintTime;
        public float BlockChance() => _blockChance;
        public bool Attacking() => _attacking;

        public int AttackDamage() => _attackDamage;

        protected override void Start()
        {
            base.Start();
            _animator = transform.GetComponentInChildren<Animator>();
            start = transform.position;
            startRot = transform.rotation;
            OnKilled.AddListener(HandleKilled);
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            _fightMs = GameManager.GetMonoSystem<IFightMonoSystem>();
            _healthBar = transform.Find("HealthBar");
            _healthBarBg = transform.Find("HealthBarBg");
            _healthBarFullSize = _healthBar.localScale.y;
            DisableHealthBar();

            SetNormalPath();

            DCGameManager.OnRestart.AddListener(OnRestart);
        }

        private void OnRestart()
        {
            _animator.SetTrigger("Reset");
            gameObject.SetActive(true);
            transform.position = start;
            transform.rotation = startRot;
            _healthBar.gameObject.SetActive(false);
            _healthBarBg.gameObject.SetActive(false);
            SetNormalPath();
            Sync();
        }

        private void HandleKilled()
        {
            Transform drop = Instantiate(Resources.Load<GameObject>($"Prefabs/Items/Specfic/Rock{_materialType}")).transform;
            drop.position = new Vector3(transform.position.x, drop.position.y, transform.position.z);
        }

        private void SetNormalPath()
        {
            _distracted = false;
            _pathDirector = new PathDirector(this, _path.Select(p => _gridMs.WorldToGrid(p.position)).ToList(), _loop ? PathDirector.LoopMode.Circle : PathDirector.LoopMode.BackAndForth);
        }

		void FixedUpdate()
        {
            if (Enemy.pause) return;
            
            _pathDirector.Travel();

            if (_distracted && _pathDirector.IsDone())
            {
                if (_pathDirector.PathPosition() <= 0)
                {
                    if (Facing() != _saveDirection)
                    {
                        if (CurrentAction() == Action.None) RequestAction(Action.TurnLeft);
                    }
                    else
                    {
                        SetNormalPath();
                    }
                }
                else if (_pathDirector.FinishTime() > DCGameManager.settings.enemyDistractedWaitTime)
                {
                    _pathDirector.Reverse();
                }
            }

            CheckCanSeePlayer();
        }

        public void DisableHealthBar()
        {
            _healthBar.gameObject.SetActive(false);
            _healthBarBg.gameObject.SetActive(false);
        }
        
        public void SetHealBar(float value)
        {
            _healthBar.gameObject.SetActive(true);
            _healthBarBg.gameObject.SetActive(true);
            if (value < 0) value = 0;
            float worldSize = _healthBarFullSize * value;
            _healthBar.localScale = new Vector3(_healthBar.localScale.x, worldSize, _healthBar.localScale.z);
            _healthBar.localPosition = new Vector3((_healthBarFullSize - worldSize) / 2, _healthBar.localPosition.y, _healthBar.localPosition.z);
        }

        private void CheckCanSeePlayer()
        {
            Vector2Int forward = Facing().ToVector2Int();
            Vector2Int right = Facing().Right().ToVector2Int();
            Vector2Int pos = _gridMs.WorldToGrid(transform.position);
            List<Vector2Int> vision = new List<Vector2Int>();
            vision.Add(pos);
            if (_gridMs.CanMoveTo(pos, Facing()))
            {
                vision.Add(pos + 1 * forward + 0 * right);
                if (_gridMs.CanMoveTo(pos, Facing().Right()) && _gridMs.CanMoveTo(pos + Facing().ToVector2Int(), Facing().Right())) vision.Add(pos + 1 * forward + 1 * right);
                if (_gridMs.CanMoveTo(pos, Facing().Left()) && _gridMs.CanMoveTo(pos + Facing().ToVector2Int(), Facing().Left())) vision.Add(pos + 1 * forward - 1 * right);
            }
            
            vision.ForEach(p => _gridMs.SetTileEnemySeen(p));

            if (vision.Any(p => _gridMs.GetEntitesOnTile(p).Any(e => e.entity.transform.GetComponent<Player>())))
            {
                GameManager.GetMonoSystem<IFightMonoSystem>().StartFight(this);
            }
        }

        public void EnterBattle()
        {
            _animator.SetTrigger("Battle");
        }
        
        public void DoAttackHint()
        {
            _animator.SetTrigger("Hint");
        }

        public void DoBlock()
        {
            _animator.SetTrigger("Block");
        }
        
        public void DoUnblock()
        {
            _animator.SetTrigger("Unblock");
        }

        public void DoAttackAnimation()
        {
            if (_attacking) return;
            _animator.SetTrigger("Swing");
            _attacking = true;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + transform.forward * 0.7f;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _attackAniTime,
                (float t) =>
                {
                    if (t < 0.5f) transform.position = Vector3.Lerp(startPos, endPos, t * 2.0f);
                    else transform.position = Vector3.Lerp(endPos, startPos, (t - 0.5f) * 2.0f);
                },
				() =>
                {
                    _attacking = false;
                    _fightMs.EnemyAttackDone();
                });
        }

        public void Distraction(Vector2Int at)
        {
            List<Vector2Int> path = _gridMs.PathFind(GridPosition(), at);
            CancelMove();
            if (!_distracted)
            {
                _pathDirector = new PathDirector(this, path, PathDirector.LoopMode.Once);
                _saveDirection = Facing();
                _distracted = true;
            }
            else
            {
                _pathDirector.JoinPath(path);
            }
        }

    }
}
