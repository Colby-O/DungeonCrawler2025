using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
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

        public void SetRawMovement(Vector2 rawMovement)
        {
            _rawMovement = rawMovement;

            if (_rawMovement != Vector2.zero) ProcessMovement();
        }

        public void SetRawTurn(float rawTurn)
        {
            _rawTurn = rawTurn;

            if (rawTurn != 0) ProcessTurn();
        }

        private void HandleMovementAction(InputAction.CallbackContext e)
        {
            if (DCGameManager.IsPaused) return;
            SetRawMovement(e.ReadValue<Vector2>());
        }

        private void HandleMovementCancelAction(InputAction.CallbackContext e)
        {
            SetRawMovement(Vector2.zero);
        }

        private void HandleTurnAction(InputAction.CallbackContext e)
        {
            if (DCGameManager.IsPaused) return;
            SetRawTurn(e.ReadValue<float>());
           
        }

        private void HandleTurnCancelAction(InputAction.CallbackContext e)
        {
            SetRawTurn(0);
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
