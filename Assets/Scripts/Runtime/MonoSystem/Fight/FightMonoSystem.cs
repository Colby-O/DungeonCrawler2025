using PlazmaGames.Core.Debugging;
using PlazmaGames.Runtime.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Core;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;

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
        private TimingBar _playerAttackIndicator;

        private FightState _fightState;
        
        private Player _player;
        private Enemy _enemy;
        
        private void Start()
        {
            _grid = GameManager.GetMonoSystem<IGridMonoSystem>();
            _player = FindFirstObjectByType<Player>();
            _playerAttackIndicator = FindFirstObjectByType<TimingBar>();
        }

        public bool InFight() => _fightState == FightState.Fighting;

        public void StartFight(Enemy enemy)
        {
            Debug.Log("Fight start");
            Enemy.pause = true;
            Player.stopMovement = true;
            _enemy = enemy;
            _fightState = FightState.PlayerTurnToEnemy;
        }

        public void PlayerAttack()
        {
            if (_playerAttackIndicator.IsStopped()) return;
            
            if (_playerAttackIndicator.IsGreen())
            {
                _playerAttackIndicator.Stop();
                _player.DoAttackAnimation();
            }
        }

        public void PlayerAttackDone()
        {
            _playerAttackIndicator.Reset();
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
                case FightState.PlayerTurnToEnemyReal:
                {
                    if (_player.CurrentAction() != Action.None) break;
                    if (EntityFaceEntity(_player, _enemy))
                    {
                        _playerAttackIndicator.gameObject.SetActive(true);
                        _fightState = FightState.Fighting;
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
                case FightState.Fighting:
                {
                    break;
                }
            }
        }
    }
}
