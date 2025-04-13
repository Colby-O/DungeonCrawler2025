using NUnit.Framework.Internal.Filters;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class GameView : View
    {
        [Header("Movement Buttons")]
        [SerializeField] private List<EventButton> _moveButtons;

        private Player _player;

        public void ForceHighlighted(Action action, bool isHelighted)
        {
            _moveButtons[(int)action].ForceHighlightedt(isHelighted);
        }

        private void ApplyAction(Action action, bool state)
        {
            if (_player == null) _player = DCGameManager.Player.GetComponent<Player>();

            switch (action)
            {
                case Action.MoveUp:
                    _player.SetRawMovement((state) ? new Vector2(0, 1) : Vector2.zero);
                    break;
                case Action.MoveDown:
                    _player.SetRawMovement((state) ? new Vector2(0, -1) : Vector2.zero);
                    break;
                case Action.MoveLeft:
                    _player.SetRawMovement((state) ? new Vector2(-1, 0) : Vector2.zero);
                    break;
                case Action.MoveRight:
                    _player.SetRawMovement((state) ? new Vector2(1, 0) : Vector2.zero);
                    break;
                case Action.TurnLeft:
                    _player.SetRawTurn((state) ? -1 : 0);
                    break;
                case Action.TurnRight:
                    _player.SetRawTurn((state) ? 1 : 0);
                    break;
                default:
                    break;
            }
        }

        public override void Init()
        {
            if (_moveButtons != null) 
            { 
                for (int i = 0; i < _moveButtons.Count; i++)
                {
                    Action action = (Action)i;
                    _moveButtons[i].onPointerDown.AddListener(() => ApplyAction(action, true));
                    _moveButtons[i].onPointerUp.AddListener(() => ApplyAction(action, false));
                }
            }
        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = false;
        }
    }
}
