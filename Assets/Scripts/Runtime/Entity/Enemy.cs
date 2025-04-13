using UnityEngine;
using PlazmaGames.Animation;
using System.Collections.Generic;
using System.Linq;
using DC2025.Utils;
using PlazmaGames.Core;

namespace DC2025
{
	public class Enemy : Entity
    {
        public static bool pause = false;

        private IFightMonoSystem _fightMs;
        private IGridMonoSystem _gridMs;
		[SerializeField] List<Transform> _path;
		[SerializeField] bool _loop;
		[SerializeField] int _pathPosition;
		[SerializeField] int _pathDirection;
        private bool _attacking = false;
        private SwordSwing _sword;
        [SerializeField] private float _attackHintTime = 0.5f;

        public SwordSwing Sword() => _sword;
        public float AttackHintTime() => _attackHintTime;
        public bool Attacking() => _attacking;

        protected override void Start()
        {
            base.Start();
            _sword = GetComponentInChildren<SwordSwing>();
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            _fightMs = GameManager.GetMonoSystem<IFightMonoSystem>();
        }

		void FixedUpdate()
        {
            if (Enemy.pause) return;
            
            if (CurrentAction() == Action.None)
            {
                Transform next = TryNextPathPosition();
                Vector3 worldDir = (next.position - transform.position).normalized;
                float angle = Vector3.SignedAngle(Vector3.forward, worldDir, Vector3.up);
                Direction dir = Direction.North.GetFacingDirection(angle);
                if (dir != Facing())
                {
                    if (((int)Facing() - (int)dir + 4) % 4 > 2) RequestAction(Action.TurnRight);
                    else RequestAction(Action.TurnLeft);
                }
                else
                {
                    RequestAction(Action.MoveUp);
                }
            }

            CheckCanSeePlayer();
        }

        private void CheckCanSeePlayer()
        {
            Vector2Int forward = Facing().ToVector2Int();
            Vector2Int right = Facing().Right().ToVector2Int();
            Vector2Int pos = _gridMs.WorldToGrid(transform.position);
            if (!_gridMs.CanMoveTo(pos, Facing())) return;
            List<Vector2Int> vision = new List<Vector2Int>();
            vision.Add(pos);
            vision.Add(pos + 1 * forward + 0 * right);
            vision.Add(pos + 1 * forward + 1 * right);
            vision.Add(pos + 1 * forward - 1 * right);
            
            vision.ForEach(p => _gridMs.SetTileEnemySeen(p));

            if (vision.Any(p => _gridMs.GetEntitesOnTile(p).Any(e => e.entity.transform.GetComponent<Player>())))
            {
                GameManager.GetMonoSystem<IFightMonoSystem>().StartFight(this);
            }
        }

        private Transform TryNextPathPosition()
        {
            Vector2Int gridPos = _gridMs.WorldToGrid(transform.position);
            Vector2Int nextGridPos = _gridMs.WorldToGrid(_path[_pathPosition].position);
            if (gridPos != nextGridPos) return _path[_pathPosition];
            
			int nextPos = _pathPosition + _pathDirection;
			if (nextPos < 0 || nextPos >= _path.Count)
			{
                if (!_loop)
                {
                    _pathDirection *= -1;
                }
                else
                {
                    _pathPosition = -1;
                }
			}

			_pathPosition += _pathDirection;

			return _path[_pathPosition];
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
                0.2f,
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

    }
}
