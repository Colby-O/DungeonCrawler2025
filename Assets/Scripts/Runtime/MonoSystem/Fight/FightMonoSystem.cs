using PlazmaGames.Core.Debugging;
using PlazmaGames.Runtime.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Core;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;
using Random = System.Random;

namespace DC2025
{
	public class FightMonoSystem : MonoBehaviour, IFightMonoSystem
    {
        private enum FightState
        {
            None,
            PlayerTurnToEnemy,
            EnemyMoveToPlayer,
            EnemyTurnToPlayer,
            PlayerTurnToEnemyReal,
            Fighting,
        }
        
        private IGridMonoSystem _grid;

        private FightState _fightState;

        private float _enemyAttackCountdown = 0;
        private float _enemyBlockCountdown = 0;
        
        private Player _player;
        private Enemy _enemy;
        private float _enemyHealth = 0;
        private bool _enemyAttackBlocked = false;

        public bool InFight() => _fightState == FightState.Fighting;
        
        private void Start()
        {
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
            _player = FindFirstObjectByType<Player>();
        }


        private void FixedUpdate()
        {
            switch (_fightState)
            {
                case FightState.None: break;
                case FightState.PlayerTurnToEnemy:
                {
                    if (_player.CurrentAction() != Action.None) break;
                    if (EntityFaceEntity(_player, _enemy))
                    {
                        _fightState = FightState.EnemyMoveToPlayer;
                    }
                    break;
                }
                case FightState.EnemyMoveToPlayer:
                {
                    if (_enemy.CurrentAction() != Action.None) break;
                    Vector2Int playerPos = _grid.WorldToGrid(_player.transform.position);
                    Vector2Int enemyPos = _grid.WorldToGrid(_enemy.transform.position);
                    Vector2Int dir = playerPos - enemyPos;
                    if ((dir.x == 0 || dir.y == 0) && (Mathf.Abs(dir.x) == 1 || Mathf.Abs(dir.y) == 1))
                    {
                        _fightState = FightState.EnemyTurnToPlayer;
                        break;
                    }

                    dir.x = dir.x switch { > 0 => 1, < 0 => -1, _ => 0 };
                    dir.y = dir.y switch { > 0 => 1, < 0 => -1, _ => 0 };
                    if (_enemy.Facing() == Direction.North || _enemy.Facing() == Direction.South)
                    {
                        if (dir.y == 0) dir.y = _enemy.Facing() == Direction.North ? -1 : 1;
                        if (enemyPos.y + dir.y != playerPos.y)
                        {
                            Direction dirdir = dir.y > 0 ? Direction.North : Direction.South;
                            _enemy.RequestAction(dirdir.GetMovement(_enemy.Facing()));
                        }
                        else if (enemyPos.x != playerPos.x)
                        {
                            Direction dirdir = dir.x > 0 ? Direction.East: Direction.West;
                            _enemy.RequestAction(dirdir.GetMovement(_enemy.Facing()));
                        }
                    }
                    else
                    {
                        if (dir.x == 0) dir.x = _enemy.Facing() == Direction.East ? -1 : 1;
                        if (enemyPos.x + dir.x != playerPos.x)
                        {
                            Direction dirdir = dir.x > 0 ? Direction.East: Direction.West;
                            _enemy.RequestAction(dirdir.GetMovement(_enemy.Facing()));
                        }
                        else if (enemyPos.y != playerPos.y)
                        {
                            Direction dirdir = dir.y > 0 ? Direction.North : Direction.South;
                            _enemy.RequestAction(dirdir.GetMovement(_enemy.Facing()));
                        }
                    }
                    break;
                }
                case FightState.EnemyTurnToPlayer:
                {
                    if (_player.CurrentAction() != Action.None) break;
                    if (EntityFaceEntity(_enemy, _player))
                    {
                        _fightState = FightState.PlayerTurnToEnemyReal;
                    }
                    break;
                }
                case FightState.PlayerTurnToEnemyReal:
                {
                    if (_player.CurrentAction() != Action.None) break;
                    if (EntityFaceEntity(_player, _enemy))
                    {
                        _fightState = FightState.Fighting;
                        EnemyQueueNextMove();
                    }
                    break;
                }
                case FightState.Fighting:
                {
                    _enemyBlockCountdown -= Time.fixedDeltaTime;
                    if (_enemyBlockCountdown <= 0)
                    {
                        if (_enemyAttackCountdown >= _enemy.AttackHintTime() && _enemyAttackCountdown - Time.fixedDeltaTime < _enemy.AttackHintTime())
                        {
                            _enemy.Sword().Raise();
                        }
                        _enemyAttackCountdown -= Time.fixedDeltaTime;
                        if (_enemyAttackCountdown <= 0)
                        {
                            EnemyAttack();
                        }
                    }
                    break;
                }
            }
        }

        private void EnemyQueueNextMove()
        {
            if (UnityEngine.Random.Range(0, 100) < 17)
            {
                _enemyBlockCountdown = UnityEngine.Random.Range(1.5f, 2.5f);
            }
            _enemyAttackBlocked = false;
            _enemyAttackCountdown = UnityEngine.Random.Range(1.5f, 3.0f);
        }

        private void EnemyAttack()
        {
            if (!_enemy.Attacking()) _enemy.DoAttackAnimation();
        }

        public void EnemyAttackDone()
        {
            if (_enemyAttackBlocked)
            {
                Debug.Log("Attack blocked!");
                _player.Sword().Lower();
                _player.manager.Damage(7);
            }
            else
            {
                Debug.Log("Failed to block!");
                _player.manager.Damage(25);
            }
            EnemyQueueNextMove();
        }
        
        public void StartFight(Enemy enemy)
        {
            Debug.Log("Fight start");
            Enemy.pause = true;
            Player.stopMovement = true;
            _enemy = enemy;
            _enemyHealth = 100;
            _fightState = FightState.PlayerTurnToEnemy;
        }

        public void PlayerAttack()
        {
            if (_player.IsAttacking() || _enemyAttackBlocked) return;
            _player.DoAttackAnimation();
        }

        public bool PlayerBlock()
        {
            Debug.Log("Try Blocked");
            if (!_enemyAttackBlocked && _enemyAttackCountdown > 0 && _enemyAttackCountdown < _enemy.AttackHintTime())
            {
                _enemyAttackBlocked = true;
                Debug.Log("Blocked Time Hit!");
                _player.Sword().Block();
                return true;
            }

            return false;
        }

        public void PlayerAttackDone()
        {
            if (_enemyBlockCountdown > 0)
            {
                Debug.Log("Enemy Blocked");
            }
            else
            {
                _enemyHealth -= 5;
                if (_enemyHealth <= 0)
                {
                    EnemyDie();
                }
            }
        }

        private void EnemyDie()
        {
            _enemy.gameObject.SetActive(false);
            Enemy.pause = false;
            Player.stopMovement = false;
            _fightState = FightState.None;
        }

        bool EntityFaceEntity(Entity facer, Entity facee)
        {
            if (facer.CurrentAction() != Action.None) return false;
            Vector3 worldDir = (facee.transform.position - facer.transform.position).normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, worldDir, Vector3.up);
            Direction dir = Direction.North.GetFacingDirection(angle);
            if (dir != facer.Facing())
            {
                if (((int)facer.Facing() - (int)dir + 4) % 4 > 2) facer.RequestAction(Action.TurnRight);
                else facer.RequestAction(Action.TurnLeft);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
