using DC2025.Utils;
using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;

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
        West
    }

    public static class ActionExtension
    {
        public static Direction GetDirection(this Action action, Direction facing)
        {
            return (Direction)(((int)facing + (int)action) % 4);
        }

        public static bool IsMove(this Action action)
        {
            return action == Action.MoveUp || action == Action.MoveDown || action == Action.MoveLeft || action == Action.MoveRight;
        }

        public static bool IsTurn(this Action action)
        {
            return action == Action.TurnLeft || action == Action.TurnRight;
        }
    }

    public static class DirectionExtension
    {
        public static Action GetMovement(this Direction dir, Direction facing)
        {
            return (Action)((((int)dir - (int)facing)) % 4);
        }

        public static Vector2Int GetGridOffset(this Direction dir)
        {
            if (dir == Direction.North) return new Vector2Int(0, 1);
            else if (dir == Direction.South) return new Vector2Int(0, -1);
            else if (dir == Direction.West) return new Vector2Int(-1, 0);
            else if (dir == Direction.East) return new Vector2Int(1, 0);
            else return Vector2Int.zero;
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
            return (Direction)(((int)dir + Mathf.RoundToInt((Math.NormalizeAngle(angle) + 180f) / 90f)) % 4);
        }
    }

    public abstract class Entity : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] protected float _moveSpeed;

        [Header("Postional Data")]
        [SerializeField, ReadOnly] private Direction _facing;
        [SerializeField, ReadOnly] private Vector2Int _gridPos;

        [Header("Input Actions")]
        [SerializeField, ReadOnly] private Action _currentActtion;
        [SerializeField, ReadOnly] private Action _queuedAction;

        private void MovementStep(float t, Vector3 startPos, Vector3 endPos)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
        }

        private void RotationStep(float t, Quaternion startRot, Quaternion endRot)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
        }

        protected virtual void OnMoveComplete()
        {
            _currentActtion = Action.None;
            RequestAction(Action.None);
        }

        private void ProcessMove(Action action)
        {
            if (!action.IsMove()) return;

            Vector3 startPos = GameManager.GetMonoSystem<IGridMonoSystem>().GridToWorld(_gridPos).SetY(transform.position.y);

            Vector2Int newGridPos = _gridPos + action.GetDirection(_facing).GetGridOffset();

            if (
                !GameManager.GetMonoSystem<IGridMonoSystem>().CanMoveTo(_gridPos, action.GetDirection(_facing).Opposite()) ||
                !GameManager.GetMonoSystem<IGridMonoSystem>().CanMoveTo(newGridPos, action.GetDirection(_facing))
            )
            {
                Debug.Log("Can't Move Here");
                //OnMoveComplete();
                _currentActtion = Action.None;
                return;
            }

            _gridPos = newGridPos;
            Vector3 endPos = GameManager.GetMonoSystem<IGridMonoSystem>().GridToWorld(_gridPos).SetY(transform.position.y);

            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _moveSpeed,
                (float t) => MovementStep(t, startPos, endPos),
                OnMoveComplete
            );
        }

        private void ProcessTurn(Action action)
        {
            if (!action.IsTurn()) return;

            Quaternion startRot = transform.rotation;

            _facing = _facing.Turn(action);

            Quaternion endRot = Quaternion.Euler(transform.rotation.eulerAngles.SetY(_facing.GetFacing()));

            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _moveSpeed,
                (float t) => RotationStep(t, startRot, endRot),
                OnMoveComplete
            );
        }

        public void RequestAction(Action action)
        {
            if (_currentActtion != Action.None)
            {
                if (_queuedAction == Action.None && _currentActtion != action) _queuedAction = action;
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

        protected virtual void Start()
        {
            _queuedAction = Action.None;
            _currentActtion = Action.None;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.SetY(_facing.GetFacing()));
            _gridPos = GameManager.GetMonoSystem<IGridMonoSystem>().WorldToGrid(transform.position);
        }
    }
}
