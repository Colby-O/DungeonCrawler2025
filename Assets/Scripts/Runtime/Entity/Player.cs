using PlazmaGames.Attribute;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace DC2025
{
    public class Player : Entity
    {
        [Header("Input System")]
        [SerializeField] private PlayerInput _input;
        [SerializeField, ReadOnly] private Vector2 _rawMovement;
        [SerializeField, ReadOnly] private float _rawTurn;

        private void HandleMovementAction(InputAction.CallbackContext e)
        {
            _rawMovement = e.ReadValue<Vector2>();
            ProcessMovement();
        }

        private void HandleMovementCancelAction(InputAction.CallbackContext e)
        {
            _rawMovement = Vector2.zero;
        }

        private void HandleTurnAction(InputAction.CallbackContext e)
        {
            _rawTurn = e.ReadValue<float>();
            ProcessTurn();
        }

        private void HandleTurnCancelAction(InputAction.CallbackContext e)
        {
            _rawTurn = 0;
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
            if (_input == null) _input = GetComponent<PlayerInput>();

            _input.actions["Movement"].performed += HandleMovementAction;
            _input.actions["Movement"].canceled += HandleMovementCancelAction;

            _input.actions["Turn"].performed += HandleTurnAction;
            _input.actions["Turn"].canceled += HandleTurnCancelAction;
        }

        private void FixedUpdate()
        {

        }
    }
}
