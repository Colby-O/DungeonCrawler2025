using PlazmaGames.Attribute;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.XR;
using Input = UnityEngine.Input;

namespace DC2025
{
    public class Player : Entity
    {
        public static bool stopMovement = false;

        private IFightMonoSystem _fightMs;
        private IGridMonoSystem _gridMs;
        private IInventoryMonoSystem _invMs;

        private Sword _sword;

        public PlayerManager manager;
        
        [Header("Input System")]
        [SerializeField] private PlayerInput _input;
        [SerializeField, ReadOnly] private Vector2 _rawMovement;
        [SerializeField, ReadOnly] private float _rawTurn;
        [SerializeField] private float _attackAniMagnitude = 0.7f;
        [SerializeField] private float _stumbleAniSpeed = 0.2f;
        [SerializeField] private float _stumbleAniMagnitude = 0.5f;

        public Sword Sword() => _sword;
        private bool _attacking = false;

        public bool IsAttacking() => _attacking;

        public void SetRawMovement(Vector2 rawMovement)
        {
            if (DCGameManager.IsPaused || Player.stopMovement) return;
            _rawMovement = rawMovement;

            if (_rawMovement != Vector2.zero) ProcessMovement();
        }

        public void SetRawTurn(float rawTurn)
        {
            if (DCGameManager.IsPaused || Player.stopMovement) return;

            _rawTurn = rawTurn;

            if (rawTurn != 0) ProcessTurn();
        }

        private void HandleMovementAction(InputAction.CallbackContext e)
        {
            SetRawMovement(e.ReadValue<Vector2>());
        }

        private void HandleMovementCancelAction(InputAction.CallbackContext e)
        {
            SetRawMovement(Vector2.zero);
        }

        private void HandleTurnAction(InputAction.CallbackContext e)
        {
            SetRawTurn(e.ReadValue<float>());
        }

        private void HandleTurnCancelAction(InputAction.CallbackContext e)
        {
            SetRawTurn(0);
        }
        
        private void HandleInteract(InputAction.CallbackContext obj)
        {
            if (_fightMs.InFight()) _fightMs.PlayerAttack();
        }
        
        private void HandleBlock(InputAction.CallbackContext obj)
        {
            if (_fightMs.InFight())
            {
                _fightMs.PlayerBlock();
            }
        }
        
        private void ProcessMovement()
        {
            if (_rawMovement.x > 0)
            {
                RequestAction(Action.MoveRight);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveRight, true);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveLeft, false);
            }
            else if (_rawMovement.x < 0)
            {
                RequestAction(Action.MoveLeft);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveRight, false);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveLeft, true);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveRight, false);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveLeft, false);
            }

            if (_rawMovement.y > 0)
            {
                RequestAction(Action.MoveUp);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveUp, true);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveDown, false);
            }
            else if (_rawMovement.y < 0)
            {
                RequestAction(Action.MoveDown);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveUp, false);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveDown, true);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveUp, false);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.MoveDown, false);
            }
        }

        private void ProcessTurn()
        {
            if (_rawTurn > 0)
            {
                RequestAction(Action.TurnRight);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.TurnRight, true);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.TurnLeft, false);
            }
            else if (_rawTurn < 0)
            {
                RequestAction(Action.TurnLeft);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.TurnRight, false);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.TurnLeft, true);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.TurnRight, false);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceHighlighted(Action.TurnLeft, false);
            }
        }

        protected override void OnMoveComplete()
        {
            base.OnMoveComplete();
            if (_moveSpeed > 0 && !DCGameManager.IsPaused && !Player.stopMovement)
            {
                ProcessMovement();
                ProcessTurn();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.manager = GetComponent<PlayerManager>();
            _sword = GetComponentInChildren<Sword>();
            _fightMs = GameManager.GetMonoSystem<IFightMonoSystem>();
            _gridMs = GameManager.GetMonoSystem<IGridMonoSystem>();
            _invMs = GameManager.GetMonoSystem<IInventoryMonoSystem>();
            if (_input == null) _input = GetComponent<PlayerInput>();

            _input.actions["Movement"].performed += HandleMovementAction;
            _input.actions["Movement"].canceled += HandleMovementCancelAction;

            _input.actions["Turn"].performed += HandleTurnAction;
            _input.actions["Turn"].canceled += HandleTurnCancelAction;
            
            _input.actions["Interact"].performed += HandleInteract;
            _input.actions["Block"].performed += HandleBlock;
            
        }

        protected override void Start()
        {
            base.Start();
            Debug.Log(_invMs);
            Debug.Log(_invMs.GetHandSlot(SlotType.Left));
            Debug.Log(_invMs.GetHandSlot(SlotType.Left).OnChange);
            _invMs.GetHandSlot(SlotType.Left).OnChange.AddListener(HandleSwordSlotChange);
        }

        private void HandleSwordSlotChange()
        {
            PickupableItem item = _invMs.GetHandSlot(SlotType.Left).Item;
            if (!item || item is not WeaponItem)
            {
                _sword.SetModel(null);
            }
            else
            {
                WeaponItem weapon = item as WeaponItem;
                _sword.SetModel(weapon.transform.Find("Model"));
            }
        }

        protected override void Update()
        {
            base.Update();
            do
            {
                if (!Input.GetKeyDown(KeyCode.T)) break;
                Entity entity = _gridMs.GetClosestEntity(GridPosition(), e => e is Enemy);
                if (!entity) break;
                Enemy enemy = entity as Enemy;
                Debug.Log("DISTRACT");
                enemy.Distraction(GridPosition());
            } while (false);
        }

        public void DoAttackAnimation()
        {
            if (!_sword.HasSword()) return;
            if (_attacking) return;
            _attacking = true;
            _sword.Swing();
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + transform.forward * _attackAniMagnitude;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _sword.stats.speed,
                (float t) =>
                {
                    if (t < 0.5f) transform.position = Vector3.Lerp(startPos, endPos, t * 2.0f);
                    else transform.position = Vector3.Lerp(endPos, startPos, (t - 0.5f) * 2.0f);
                },
				() =>
                {
                    _attacking = false;
                    _fightMs.PlayerAttackDone();
                });
        }

        public void Stumble()
        {
            if (_attacking) return;
            _attacking = true;
            _sword.Stumble();
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + transform.right * _stumbleAniMagnitude;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _stumbleAniSpeed,
                (float t) =>
                {
                    if (t < 0.5f) transform.position = Vector3.Lerp(startPos, endPos, t * 2.0f);
                    else transform.position = Vector3.Lerp(endPos, startPos, (t - 0.5f) * 2.0f);
                },
				() =>
                {
                    _attacking = false;
                });
        }
    }
}
