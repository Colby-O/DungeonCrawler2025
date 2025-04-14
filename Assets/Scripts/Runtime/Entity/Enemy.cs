using UnityEngine;
using PlazmaGames.Animation;
using System.Collections.Generic;
using System.Linq;
using DC2025.Utils;
using PlazmaGames.Core;
using UnityEditor.PackageManager.Requests;

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
        private SwordSwing _sword;
        [SerializeField] private float _attackHintTime = 0.5f;
        [SerializeField] private float _attackAniTime = 0.25f;
        [SerializeField] private float _blockChance = 0.1f;

        private Transform _healthBar;
        private Transform _healthBarBg;
        private float _healthBarFullSize;
        
        private bool _distracted = false;
        private Direction _saveDirection;
        private PathDirector _pathDirector;

        public SwordSwing Sword() => _sword;
        public float AttackHintTime() => _attackHintTime;
        public float BlockChance() => _blockChance;
        public bool Attacking() => _attacking;

        protected override void Start()
        {
            base.Start();
            _sword = GetComponentInChildren<SwordSwing>();
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            _fightMs = GameManager.GetMonoSystem<IFightMonoSystem>();
            _healthBar = transform.Find("HealthBar");
            _healthBarBg = transform.Find("HealthBarBg");
            _healthBarFullSize = _healthBar.localScale.y;
            DisableHealthBar();

            SetNormalPath();
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
            value /= 100;
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
                if (_gridMs.CanMoveTo(pos, Facing().Right())) vision.Add(pos + 1 * forward + 1 * right);
                if (_gridMs.CanMoveTo(pos, Facing().Left())) vision.Add(pos + 1 * forward - 1 * right);
            }
            
            vision.ForEach(p => _gridMs.SetTileEnemySeen(p));

            if (vision.Any(p => _gridMs.GetEntitesOnTile(p).Any(e => e.entity.transform.GetComponent<Player>())))
            {
                GameManager.GetMonoSystem<IFightMonoSystem>().StartFight(this);
            }
        }

        public void DoAttackAnimation()
        {
            if (_attacking) return;
            _sword.Swing();
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
