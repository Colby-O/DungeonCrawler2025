using System.Collections.Generic;
using DC2025.Utils;
using NUnit.Framework;
using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace DC2025
{
	public enum Action
	{
		MoveUp,
		MoveRight,
		MoveDown,
		MoveLeft,
		TurnLeft,
		TurnRight,
		None,
	}

	public enum Direction
	{
		North,
		East,
		South,
		West,
	}

	public static class ActionExtension
	{
		public static Direction GetDirection(this Action action, Direction facing)
		{
			return (Direction)(((int)facing + (int)action) % 4);
		}

		public static bool IsMove(this Action action)
		{
			return action is Action.MoveUp or Action.MoveDown or Action.MoveLeft or Action.MoveRight;
		}

		public static bool IsTurn(this Action action)
		{
			return action is Action.TurnLeft or Action.TurnRight;
		}
	}

	public static class DirectionExtension
    {
        public static Direction FromVector2Int(Vector2Int v)
        {
            if (v.x < 0) return Direction.West;
            if (v.x > 0) return Direction.East;
            if (v.y < 0) return Direction.South;
            else return Direction.North;
        }
        public static List<Direction> AllDirections() => new List<Direction>{Direction.North, Direction.East, Direction.South, Direction.West};
		public static Action GetMovement(this Direction dir, Direction facing)
		{
			return (Action)((((int)dir - (int)facing + 4)) % 4);
		}

		public static Vector2Int GetGridOffset(this Direction dir)
		{
			if (dir == Direction.North) return new Vector2Int(0, 1);
			else if (dir == Direction.South) return new Vector2Int(0, -1);
			else if (dir == Direction.West) return new Vector2Int(-1, 0);
			else if (dir == Direction.East) return new Vector2Int(1, 0);
			else return Vector2Int.zero;
		}

        public static Direction Right(this Direction dir)
        {
            return (Direction)(((int)dir + 1) % 4);
        }
        public static Direction Left(this Direction dir)
        {
            return (Direction)(((int)dir + 3) % 4);
        }

		public static Direction Opposite(this Direction dir)
		{
			return dir switch
			{
				Direction.North => Direction.South,
				Direction.South => Direction.North,
				Direction.East => Direction.West,
				Direction.West => Direction.East,
				_ => default
			};
		}

		public static Direction Turn(this Direction dir, Action action)
		{
			if (!action.IsTurn()) return dir;

			if (action == Action.TurnLeft) return (Direction)(((int)dir + 3) % 4);
			else return (Direction)(((int)dir + 1) % 4);
		}

		public static float GetFacing(this Direction dir)
		{
			return dir switch
			{
				Direction.North => 0,
				Direction.East => 90,
				Direction.South => 180,
				Direction.West => 270,
				_ => 0
			};
		}

		public static Direction GetFacingDirection(this Direction dir, float angle)
		{
			return (Direction)(((int)dir + Mathf.RoundToInt(Math.NormalizeAngle360(angle) / 90f)) % 4);
		}

		public static Vector3 ToVector3(this Direction dir)
		{
			return dir switch
			{
				Direction.North => new Vector3(0, 0, 1),
				Direction.South => new Vector3(0, 0, -1),
				Direction.East => new Vector3(1, 0, 0),
				Direction.West => new Vector3(-1, 0, 0),
				_ => Vector3.zero
			};
		}

        public static Vector2Int ToVector2Int(this Direction dir)
        {
			return dir switch
			{
				Direction.North => new Vector2Int(0, 1),
				Direction.South => new Vector2Int(0, -1),
				Direction.East => new Vector2Int(1, 0),
				Direction.West => new Vector2Int(-1, 0),
				_ => Vector2Int.zero
			};
        }
	}

	public abstract class Entity : MonoBehaviour
	{
		[Header("Movement Settings")]
		[SerializeField] protected float _moveSpeed;
		[SerializeField] protected float _turnSpeed;

		[Header("Postional Data")]
		[SerializeField, ReadOnly] private Direction _facing;
		[SerializeField, ReadOnly] private Vector2Int _gridPos;

		[Header("Input Actions")]
		[SerializeField, ReadOnly] private Action _currentActtion;
		[SerializeField, ReadOnly] private Action _queuedAction;

		[Header("Sync")]
		[SerializeField,] private float _syncInterval;
		[SerializeField, ReadOnly] private float _timeSinceLastSync;

        protected IGridMonoSystem _gridMs;

        [Header("Audio")]
        [SerializeField] private AudioSource _as;

        
        private bool _middleSynced = true;
        private Vector3 _moveFrom;
        private bool _canceling = false;

        public Vector2Int GridPosition() => _gridMs.WorldToGrid(transform.position);

        public Action CurrentAction() => _currentActtion;
        public Direction Facing() => _facing;

		private void MovementStep(float t, Vector3 startPos, Vector3 endPos)
		{
            if (t >= 0.51 && !_middleSynced)
            {
                Sync();
                _middleSynced = true;
            }
			transform.position = Vector3.Lerp(startPos, endPos, t);
		}

		private void RotationStep(float t, Quaternion startRot, Quaternion endRot)
		{
			transform.rotation = Quaternion.Lerp(startRot, endRot, t);
		}

		protected virtual void OnMoveComplete()
		{
			Sync();
			_currentActtion = Action.None;
			RequestAction(Action.None);
		}

		private void AnimateMove(Vector3 startPos, Vector3 endPos)
        {
            _moveFrom = startPos;
            _middleSynced = false;
			GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
				this,
				_moveSpeed,
				(float t) => MovementStep(t, startPos, endPos),
				OnMoveComplete
			);
		}

        public void CancelMove()
        {
            if (_canceling) return;
            if (!CurrentAction().IsMove()) return;
            _canceling = true;
            if (GameManager.GetMonoSystem<IAnimationMonoSystem>().HasAnimationRunning(this))
            {
                GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);
                _middleSynced = false;
                Vector3 startPos = transform.position;
                GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                    this,
                    _moveSpeed,
                    (float t) => MovementStep(t, startPos, _moveFrom),
                    () =>
                    {
                        _canceling = false;
                        OnMoveComplete();
                    });
            }
        }

		private void AnimateTurn(Quaternion startRot, Quaternion endRot)
		{
			GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
				this,
				_turnSpeed,
				(float t) => RotationStep(t, startRot, endRot),
				OnMoveComplete
			);
		}


		private void AnimateInvaildMove(Vector3 startPos, Vector3 endPos)
		{
			GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
				this,
				_moveSpeed / 2.0f,
				(float t) => MovementStep(t, startPos, endPos),
				() =>
				{
					GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
						this,
						_moveSpeed / 2.0f,
						(float t) => MovementStep(t, endPos, startPos),
						OnMoveComplete
					);
				}
			);
		}

		private void ProcessMove(Action action)
		{
			if (!action.IsMove()) return;

			Vector3 startPos = _gridMs.GridToWorld(_gridPos).SetY(transform.position.y);
			Vector2Int newGridPos = _gridPos + action.GetDirection(_facing).GetGridOffset();
			Vector3 endPos = _gridMs.GridToWorld(newGridPos).SetY(transform.position.y);

            if (_as != null) _as.PlayOneShot(DCGameManager.settings.entityStepSounds[Random.Range(0, DCGameManager.settings.entityStepSounds.Count)]);

            if (
				!_gridMs.CanMoveTo(_gridPos, action.GetDirection(_facing), this is Player) ||
				!_gridMs.CanMoveTo(newGridPos, action.GetDirection(_facing).Opposite())
			)
			{
				endPos = startPos + action.GetDirection(_facing).ToVector3() * _gridMs.GetTileSize().x / 4.0f;
				AnimateInvaildMove(startPos, endPos);
			}
			else
			{
				_gridPos = newGridPos;;
				AnimateMove(startPos, endPos);
			}
        }

		private void ProcessTurn(Action action)
		{
			if (!action.IsTurn()) return;

            if (_as != null) _as.PlayOneShot(DCGameManager.settings.entityStepSounds[Random.Range(0, DCGameManager.settings.entityStepSounds.Count)]);

            Quaternion startRot = transform.rotation;

			_facing = _facing.Turn(action);

			Quaternion endRot = Quaternion.Euler(transform.rotation.eulerAngles.SetY(_facing.GetFacing()));

			AnimateTurn(startRot, endRot);
		}

		public void RequestAction(Action action)
		{
			if (_currentActtion != Action.None)
			{
				if (_queuedAction == Action.None && _currentActtion != action) _queuedAction = action;
				//if (_queuedAction == Action.None || _currentActtion == _queuedAction) _queuedAction = action;
				return;
			}

			if (_queuedAction != Action.None)
			{
				_currentActtion = _queuedAction;
				_queuedAction = action;
			}
			else if (_currentActtion == Action.None)
			{
				_currentActtion = action;
			}
			
			if (_currentActtion.IsMove()) ProcessMove(_currentActtion);
			else if (_currentActtion.IsTurn()) ProcessTurn(_currentActtion);
		}

		public void Sync()
		{
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetY(_facing.GetFacing()));
			_gridPos = _gridMs.WorldToGrid(transform.position);
			_gridMs.Sync(this, _gridPos);
			_timeSinceLastSync = 0;
		}

        protected virtual void Awake()
        {
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
        }

		private void OnRestart()
		{
            if (this is not Player) GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);
            _queuedAction = Action.None;
            _currentActtion = Action.None;
            _timeSinceLastSync = 0;
            _facing = Direction.North.GetFacingDirection(transform.rotation.eulerAngles.y);
            Sync();
        }

		protected virtual void Start()
        {
			_queuedAction = Action.None;
			_currentActtion = Action.None;

			_timeSinceLastSync= 0;

            _facing = Direction.North.GetFacingDirection(transform.rotation.eulerAngles.y);

			Sync();

			DCGameManager.OnRestart.AddListener(OnRestart);
		}

		protected virtual void Update()
		{
			_timeSinceLastSync += Time.deltaTime;

			if (_timeSinceLastSync > _syncInterval) Sync();
		}
	}
}
