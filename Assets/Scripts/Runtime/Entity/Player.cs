using PlazmaGames.Attribute;
using PlazmaGames.Animation;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace DC2025
{
    public class Player : Entity
    {
        public static bool stopMovement = false;

        private IFightMonoSystem _fightMs;
        
        [Header("Input System")]
        [SerializeField] private PlayerInput _input;
        [SerializeField, ReadOnly] private Vector2 _rawMovement;
        [SerializeField, ReadOnly] private float _rawTurn;
        [SerializeField] private float _attackAniMagnitude = 0.7f;
        [SerializeField] private float _attackAniSpeed = 0.2f;

        private void HandleMovementAction(InputAction.CallbackContext e)
        {
            if (DCGameManager.IsPaused || Player.stopMovement) return;
            _rawMovement = e.ReadValue<Vector2>();
            ProcessMovement();
        }

        private void HandleMovementCancelAction(InputAction.CallbackContext e)
        {
            _rawMovement = Vector2.zero;
        }

        private void HandleTurnAction(InputAction.CallbackContext e)
        {
            if (DCGameManager.IsPaused || Player.stopMovement) return;
            _rawTurn = e.ReadValue<float>();
            ProcessTurn();
        }

        private void HandleTurnCancelAction(InputAction.CallbackContext e)
        {
            _rawTurn = 0;
        }
        
        private void HandleInteract(InputAction.CallbackContext obj)
        {
            if (_fightMs.InFight()) _fightMs.PlayerAttack();
        }

        private void ProcessMovement()
        {
            if (_rawMovement.x > 0) RequestAction(Action.MoveRight);
            if (_rawMovement.x < 0) RequestAction(Action.MoveLeft);
            if (_rawMovement.y > 0) RequestAction(Action.MoveUp);
            if (_rawMovement.y < 0) RequestAction(Action.MoveDown);
        }

        private void ProcessTurn()
        {
            if (_rawTurn > 0) RequestAction(Action.TurnRight);
            if (_rawTurn < 0) RequestAction(Action.TurnLeft);
        }

        protected override void OnMoveComplete()
        {
            base.OnMoveComplete();
            if (_moveSpeed > 0)
            {
                ProcessMovement();
                ProcessTurn();
            }
        }

        private void Awake()
        {
            _fightMs = GameManager.GetMonoSystem<IFightMonoSystem>();
            if (_input == null) _input = GetComponent<PlayerInput>();

            _input.actions["Movement"].performed += HandleMovementAction;
            _input.actions["Movement"].canceled += HandleMovementCancelAction;

            _input.actions["Turn"].performed += HandleTurnAction;
            _input.actions["Turn"].canceled += HandleTurnCancelAction;
            
            _input.actions["Interact"].performed += HandleInteract;
        }

        private void FixedUpdate()
        {

        }

        public void DoAttackAnimation()
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + transform.forward * _attackAniMagnitude;
            GameManager.GetMonoSystem<IAnimationMonoSystem>().RequestAnimation(
                this,
                _attackAniSpeed,
                (float t) =>
                {
                    if (t < 0.5f) transform.position = Vector3.Lerp(startPos, endPos, t * 2.0f);
                    else transform.position = Vector3.Lerp(endPos, startPos, (t - 0.5f) * 2.0f);
                },
				() => _fightMs.PlayerAttackDone()
			);
        }
    }
}
