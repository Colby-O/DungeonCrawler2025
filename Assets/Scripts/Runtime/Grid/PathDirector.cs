using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Core.Utils;
using UnityEngine;
using Time = UnityEngine.Time;

namespace DC2025
{
    public class PathDirector
    {
        public enum LoopMode
        {
            Once,
            Circle,
            BackAndForth,
        }

        private Entity _entity;
        private List<Vector2Int> _path;
        private int _pathPosition = 0;
        private int _pathDirection = 1;
        private float _finishTime = 0;
        private LoopMode _loopMode;

        public int PathPosition() => _pathPosition;
        public float FinishTime() => Time.time - _finishTime;
        
        public PathDirector(Entity entity, List<Vector2Int> path, LoopMode loopMode = LoopMode.BackAndForth)
        {
            _entity = entity;
            _path = path;
            _loopMode = loopMode;
        }

        public bool IsDone() => _pathPosition >= _path.Count || _pathPosition < 0;

        public void Reverse()
        {
            _pathDirection *= -1;
            _pathPosition += _pathDirection;
        }
        
        public void JoinPath(List<Vector2Int> path)
        {
            if (_pathPosition < 0) _pathPosition = 0;
            if (_pathPosition >= _path.Count) _pathPosition = _path.Count - 1;
            _path.RemoveRange(_pathPosition, _path.Count - _pathPosition);
            path.ForEach(p => _path.Add(p));
            _pathDirection = 1;
        }

        public void Travel()
        {
            if (_entity.CurrentAction() != Action.None) return;

            Vector2Int cur = _entity.GridPosition();
            Vector2Int next = NextPosition();
            if (next == cur) return;
            Direction dir = DirectionExtension.FromVector2Int(next - cur);
            if (dir != _entity.Facing())
            {
                if (((int)_entity.Facing() - (int)dir + 4) % 4 > 2) _entity.RequestAction(Action.TurnRight);
                else _entity.RequestAction(Action.TurnLeft);
            }
            else
            {
                _entity.RequestAction(Action.MoveUp);
            }
        }

        private Vector2Int NextPosition()
        {
            Vector2Int gridPos = _entity.GridPosition();
            if (_pathPosition >= _path.Count || _pathPosition < 0) return gridPos;
            Vector2Int nextGridPos = _path[_pathPosition];
            if (gridPos != nextGridPos) return _path[_pathPosition];
            
			int nextPos = _pathPosition + _pathDirection;
			if (nextPos < 0 || nextPos >= _path.Count)
			{
                if (_loopMode == LoopMode.Circle)
                {
                    _pathPosition = -1;
                }
                else if (_loopMode == LoopMode.BackAndForth)
                {
                    _pathDirection *= -1;
                }
			}

			_pathPosition += _pathDirection;

            if (_pathPosition >= _path.Count || _pathPosition < 0)
            {
                _finishTime = Time.time;
                return gridPos;
            }

			return _path[_pathPosition];
        }

    }
}
