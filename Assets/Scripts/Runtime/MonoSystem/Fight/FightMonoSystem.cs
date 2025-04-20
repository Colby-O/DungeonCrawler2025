using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace DC2025
{
	public class FightMonoSystem : MonoBehaviour, IFightMonoSystem
    {
        private enum FightState
        {
            None,
            PlayerCancelMoves,
            PlayerTurnToEnemy,
            EnemyMoveToPlayer,
            EnemyTurnToPlayer,
            PlayerTurnToEnemyReal,
            Fighting,
        }
        
        private IGridMonoSystem _gridMs;
        private IChatWindowMonoSystem _chatMs;

        private FightState _fightState;

        private float _enemyAttackCountdown = 0;
        private float _enemyBlockCountdown = 0;
        
        private Player _player;
        private bool _playerAttackBlocked = false;
        
        private Enemy _enemy;
        private PathDirector _enemyPathDirector;
        private float _enemyHealth = 0;
        private bool _enemyAttackBlocked = false;

        private float _damageMultiplier = 1f;
        private float _strengthPotionTime;
        private float _maxStrengthPotionTime;

        private float _potionForesight;
        private float _foresightPotionTime;
        private float _maxForesightPotionTime;

        public bool InFight() => _fightState == FightState.Fighting;

        public float AttackHintTime() => _enemy.AttackHintTime() + _player.Sword().stats.foresight + _potionForesight;
        
        private void Start()
        {
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            _chatMs = GameManager.GetMonoSystem<IChatWindowMonoSystem>();
            _player = FindFirstObjectByType<Player>();
            DCGameManager.OnRestart.AddListener(EndFight);
        }


        private void FixedUpdate()
        {
            switch (_fightState)
            {
                case FightState.None: break;
                case FightState.PlayerCancelMoves:
                {
                    if (_player.CurrentAction() == Action.None)
                    {
                        _fightState = FightState.PlayerTurnToEnemy;
                    }
                    else
                    {
                        if (_player.CurrentAction().IsMove()) _player.CancelMove();
                    }
                    break;
                }
                case FightState.PlayerTurnToEnemy:
                {
                    if (_player.CurrentAction() != Action.None) break;
                    if (EntityFaceEntity(_player, _enemy))
                    {
                        List<Direction> dirs = DirectionExt.AllDirections().OrderBy(d =>
                            Vector2Int.Distance(_enemy.GridPosition(), (_player.GridPosition() + d.ToVector2Int()))).ToList();
                        foreach (Direction dir in dirs)
                        {
                            if (!_gridMs.CanMoveTo(_player.GridPosition() + dir.ToVector2Int(), dir.Opposite())) continue;
                            List<Vector2Int> path = _gridMs.PathFind(_enemy.GridPosition(), _player.GridPosition() + dir.ToVector2Int());
                            if (path.Count > 0)
                            {
                                _enemyPathDirector = new PathDirector(_enemy, path, PathDirector.LoopMode.Once);
                                break;
                            }
                        }
                        _fightState = FightState.EnemyMoveToPlayer;
                    }
                    break;
                }
                case FightState.EnemyMoveToPlayer:
                {
                    if (_enemyPathDirector.IsDone())
                    {
                        _fightState = FightState.EnemyTurnToPlayer;
                    }
                    else
                    {
                        _enemyPathDirector.Travel();
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
                    if (_enemyBlockCountdown > 0 && _enemyBlockCountdown - Time.fixedDeltaTime <= 0)
                    {
                        _enemy.DoUnblock();
                    }
                    _enemyBlockCountdown -= Time.fixedDeltaTime;
                    if (_enemyBlockCountdown <= 0)
                    {
                        if (_enemyAttackCountdown >= AttackHintTime() && _enemyAttackCountdown - Time.fixedDeltaTime < AttackHintTime())
                        {
                            _enemy.DoAttackHint();
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
            _enemyAttackBlocked = false;
            if (_player.Sword().HasSword())
            {
                if (UnityEngine.Random.Range(0, 100) < _enemy.BlockChance() * 100)
                {
                    _enemyBlockCountdown = UnityEngine.Random.Range(_enemy.AttackIntervalLow(), _enemy.AttackIntervalHigh());
                    _enemy.DoBlock();
                }
                _enemyAttackCountdown = UnityEngine.Random.Range(_enemy.AttackIntervalLow(), _enemy.AttackIntervalHigh());
            }
            else
            {
                _enemyAttackCountdown = DCGameManager.settings.enemyAttackPlayerNoSwordTime;
            }
        }

        private void EnemyAttack()
        {
            if (!_enemy.Attacking())
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.swordSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                _enemy.DoAttackAnimation();
            }
        }

        public void AddDamageBoost(float mul, float duration)
        {
            _damageMultiplier = mul;
            _maxStrengthPotionTime = duration;
            _strengthPotionTime = 0;
        }

        public void AddForesightBoost(float amount, float duration)
        {
            _potionForesight = amount;
            _maxForesightPotionTime = duration;
            _foresightPotionTime = 0;
        }


        public void EnemyAttackDone()
        {
            if (_enemyAttackBlocked)
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.blockSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                _chatMs.Send("You block the enemy's attack.");
                _player.Sword().Lower();
            }
            else
            {
                _chatMs.Send($"The enemy strikes you dealing {_enemy.AttackDamage()} damage.");
                _player.manager.Damage(_enemy.AttackDamage());
            }
            EnemyQueueNextMove();
        }
        
        public void StartFight(Enemy enemy)
        {
            _chatMs.Send("You encounter an enemy!");
            Enemy.pause = true;
            Player.stopMovement = true;
            _enemy = enemy;
            _enemyHealth = _enemy.Health();
            _enemy.SetHealBar(_enemyHealth / _enemy.Health());
            _enemy.EnterBattle();
            _fightState = FightState.PlayerCancelMoves;
        }

        public void PlayerAttack()
        {
            if (!_player.Sword().HasSword())
            {
                _chatMs.Send("Attacking without a sword is futile.");
                return;
            }
            if (_player.IsAttacking() || _enemyAttackBlocked || _player.manager.GetStamina() < _player.Sword().stats.ApplyStamina(DCGameManager.settings.playerAttackStamina)) return;
            if (_enemyBlockCountdown > 0)
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.blockSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                _player.manager.UseStamina(_player.Sword().stats.ApplyStamina(DCGameManager.settings.playerAttackFailStamina));
                _playerAttackBlocked = true;
            }
            else
            {
                _player.manager.UseStamina(_player.Sword().stats.ApplyStamina(DCGameManager.settings.playerAttackStamina));
                _playerAttackBlocked = false;
            }
            _player.Sword().TakeDurability();
            _player.DoAttackAnimation();
        }

        public void PlayerAttackDone()
        {
            if (_playerAttackBlocked)
            {
                _chatMs.Send("The enemy blocked your attack.");
            }
            else
            {
                _chatMs.Send($"You strike the enemy dealing {_player.Sword().stats.damage} damage.");
                _enemyHealth -= _player.Sword().stats.damage * _damageMultiplier;
                _enemy.SetHealBar(_enemyHealth / _enemy.Health());
                if (_enemyHealth <= 0)
                {
                    EnemyDie();
                }
            }
        }
        
        public bool PlayerBlock()
        {
            if (!_player.Sword().HasSword())
            {
                _chatMs.Send("Blocking without a sword is futile.");
                return false;
            }
            if (_enemyAttackBlocked) return false;
            if (_player.manager.GetStamina() < _player.Sword().stats.ApplyStamina(DCGameManager.settings.playerBlockStamina)) return false;
            if (_enemyAttackCountdown > 0 && _enemyAttackCountdown < AttackHintTime())
            {
                _enemyAttackBlocked = true;
                _player.Sword().Block();
                _player.manager.UseStamina(_player.Sword().stats.ApplyStamina(DCGameManager.settings.playerBlockStamina));
                return true;
            }
            else
            {
                _enemyAttackBlocked = false;
                _chatMs.Send("You try to block the enemy but stumble.");
                _player.manager.UseStamina(_player.Sword().stats.ApplyStamina(DCGameManager.settings.playerBlockFailStamina));
                _player.Stumble();
            }

            return false;
        }

        private void EnemyDie()
        {
            _enemy.OnKilled.Invoke();
            _chatMs.Send("The enemy died!");
            _gridMs.RemoveEntity(_enemy);
            _enemy.DisableHealthBar();
            _enemy.gameObject.SetActive(false);
            EndFight();
        }

        private void EndFight()
        {
            Enemy.pause = false;
            Player.stopMovement = false;
            _fightState = FightState.None;

        }

        private bool EntityFaceEntity(Entity facer, Entity facee)
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

        private void Awake()
        {
            _strengthPotionTime = 0;
            _maxStrengthPotionTime = 0;

            _foresightPotionTime = 0;
            _maxForesightPotionTime = 0;
        }

        private void Update()
        {
            if (_strengthPotionTime < _maxStrengthPotionTime)
            {
                _strengthPotionTime += Time.deltaTime;
                if (_strengthPotionTime >= _maxStrengthPotionTime)
                {
                    _damageMultiplier = 1;
                    _strengthPotionTime = 0;
                    _maxStrengthPotionTime = 0;
                }
            }

            if (_foresightPotionTime < _maxForesightPotionTime)
            {
                _foresightPotionTime += Time.deltaTime;
                if (_foresightPotionTime >= _maxForesightPotionTime)
                {
                    _potionForesight = 0;
                    _foresightPotionTime = 0;
                    _maxForesightPotionTime = 0;
                }
            }
        }
    }
}
