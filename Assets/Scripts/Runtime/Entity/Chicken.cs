using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Animation;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace DC2025
{
    public class Chicken : Entity
    {
		[SerializeField] private List<Transform> _path;
		[SerializeField] private bool _loop;
        [SerializeField, ReadOnly] private bool _running = true;
        [SerializeField, ReadOnly] private PathDirector _pathDirector;
        [SerializeField, ReadOnly] private bool _deferDistraction = false;
        [SerializeField, ReadOnly] private bool _distracting = false;
        [SerializeField, ReadOnly] private bool _enemyKilled = false;
        [SerializeField, ReadOnly] private Enemy _enemy = null;

        private Vector3 start;
        private Quaternion startRot;
        private bool _isDead = false;

        protected override void Start()
        {
            base.Start();
            start = transform.position;
            startRot = transform.rotation;
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            SetPath();
            DCGameManager.OnRestart.AddListener(OnRestart);
        }

        private void SetPath()
        {
            _pathDirector = new PathDirector(this, _path.Select(p => _gridMs.WorldToGrid(p.position)).ToList(), _loop ? PathDirector.LoopMode.Circle : PathDirector.LoopMode.BackAndForth);
        }

        private void OnRestart()
        {
            if (!_isDead) return;

            GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);

            if (_enemy) _enemy.OnKilled.RemoveListener(OnEnemyKilled);
            if (_distracting) _gridMs.UnsetTileDistraction(GridPosition());

            transform.position = start;
            transform.rotation = startRot;
            gameObject.SetActive(true);

            GetComponent<ChickenItem>().OnRestart();

             _running = true;
            _deferDistraction = false;
            _distracting = false;
            _enemyKilled = false;
            _enemy = null;
            _isDead = false;
            SetPath();
            Sync();
        }

        private Vector2Int _savePos;

        private void FixedUpdate()
        {
            if (Enemy.pause) return;

            if (_deferDistraction)
            {
                _deferDistraction = false;
                Entity entity = _gridMs.GetClosestEntity(_savePos, e => e is Enemy && !(e as Enemy).IsDistracted());
                Debug.Log(entity);
                if (!entity) return;
                Enemy enemy = entity as Enemy;
                enemy.Distraction(_savePos);
                enemy.OnKilled.AddListener(OnEnemyKilled);
                _enemy = enemy;
                _gridMs.SetTileDistraction(_savePos);
                _distracting = true;
            }

            if (_distracting)
            {
                if (_enemyKilled || _gridMs.GetEntitesOnTile(_savePos).Any(e => e.entity is Enemy))
                {
                    if (_enemy && !_enemyKilled) _enemy.OnKilled.RemoveListener(OnEnemyKilled);
                    OnRestart();
                }
            }

            if (_running)
            {
                _pathDirector.Travel();
            }
        }

        private void OnEnemyKilled() => _enemyKilled = true;

        public void RemoveSelf()
        {
            GameManager.GetMonoSystem<IAnimationMonoSystem>().StopAllAnimations(this);
            _gridMs.RemoveEntity(this);
            _running = false;
        }
        
        public void CauseDistraction()
        {
            _deferDistraction = true;
            _savePos = DCGameManager.PlayerController.GridPosition();
            _isDead = true;
        }
    }
}
