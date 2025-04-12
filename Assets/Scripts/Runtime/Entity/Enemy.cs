using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DC2025.Utils;
using PlazmaGames.Core;

namespace DC2025
{
	public class Enemy : Entity
    {
        public static bool pause = false;
        
        private IGridMonoSystem _grid;
		[SerializeField] List<Transform> _path;
		[SerializeField] bool _loop;
		[SerializeField] int _pathPosition;
		[SerializeField] int _pathDirection;

        protected override void Start()
        {
            base.Start();
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
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
            Vector2Int pos = _grid.WorldToGrid(transform.position);
            if (!_grid.CanMoveTo(pos, Facing())) return;
            List<Vector2Int> vision = new List<Vector2Int>();
            vision.Add(pos);
            vision.Add(pos + 1 * forward + 0 * right);
            vision.Add(pos + 1 * forward + 1 * right);
            vision.Add(pos + 1 * forward - 1 * right);
            
            vision.ForEach(p => _grid.SetTileEnemySeen(p));

            if (vision.Any(p => _grid.GetEntitesOnTile(p).Any(e => e.entity.transform.GetComponent<Player>())))
            {
                GameManager.GetMonoSystem<IFightMonoSystem>().StartFight(this);
            }
        }

        private Transform TryNextPathPosition()
        {
            Vector2Int gridPos = _grid.WorldToGrid(transform.position);
            Vector2Int nextGridPos = _grid.WorldToGrid(_path[_pathPosition].position);
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
	}
}
