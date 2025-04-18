using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace DC2025
{
    public class Chicken : Entity
    {
		[SerializeField] private List<Transform> _path;
		[SerializeField] private bool _loop;
        private bool _running = true;
        private PathDirector _pathDirector;
        private bool _deferDistraction = false;
        private bool _distracting = false;
        private bool _enemyKilled = false;
        private Enemy _enemy = null;

        protected override void Start()
        {
            base.Start();
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            _pathDirector = new PathDirector(this, _path.Select(p => _gridMs.WorldToGrid(p.position)).ToList(), _loop ? PathDirector.LoopMode.Circle : PathDirector.LoopMode.BackAndForth);
        }

        private void FixedUpdate()
        {
            if (Enemy.pause) return;

            if (_deferDistraction)
            {
                Debug.Log(GridPosition());
                _deferDistraction = false;
                Entity entity = _gridMs.GetClosestEntity(GridPosition(), e => e is Enemy);
                Debug.Log(entity);
                if (!entity) return;
                Enemy enemy = entity as Enemy;
                enemy.Distraction(GridPosition());
                enemy.OnKilled.AddListener(OnEnemyKilled);
                _enemy = enemy;
                _gridMs.SetTileDistraction(GridPosition());
                _distracting = true;
            }

            if (_distracting)
            {
                if (_enemyKilled || _gridMs.GetEntitesOnTile(GridPosition()).Any(e => e.entity is Enemy))
                {
                    if (_enemy && !_enemyKilled) _enemy.OnKilled.RemoveListener(OnEnemyKilled);
                    _gridMs.UnsetTileDistraction(GridPosition());
                    _distracting = false;
                    _gridMs.RemoveEntity(this);
                    Destroy(gameObject);
                }
            }
            
            if (_running) _pathDirector.Travel();
        }

        private void OnEnemyKilled() => _enemyKilled = true;

        public void RemoveSelf()
        {
            _gridMs.RemoveEntity(this);
            _running = false;
        }
        
        public void CauseDistraction()
        {
            _deferDistraction = true;
        }
    }
}
