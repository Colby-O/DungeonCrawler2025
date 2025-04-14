using PlazmaGames.Core.Debugging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlazmaGames.Core.Utils;

namespace DC2025
{
    public class CursorFollow : MonoBehaviour
    {
        [SerializeField] private RectTransform _rt;
        [SerializeField] private Canvas _canvas;

        private List<Vector2> _pivots = new List<Vector2>()
        {
            new Vector2(-0.02f, 1.06f),
            new Vector2(1, 1.04f),
            new Vector2(-0.02f, 1.06f),
            new Vector2(0.99f, 0.02f),
            new Vector2(0.01f, 0.02f),
            new Vector2(1, 1.04f),
            new Vector2(-0.02f, 1.06f),
            new Vector2(-0.02f, 1.06f),
            new Vector2(0.01f, 0.02f)
    };

        private void FollowMouse()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                _canvas.worldCamera,
                out Vector2 mousePos
            );

            _rt.anchoredPosition = mousePos;
        }

        public void PositionRelativeToEdges()
        {
            bool foundVaildPosition = false;
            foreach (Vector2 p in _pivots)
            {
                _rt.pivot = p;
                if (_rt.IsFullyVisibleFrom(Camera.main))
                {
                    foundVaildPosition = true;
                    break;
                }
            }

            if (!foundVaildPosition) PlazmaDebug.LogWarning("Failed to find a vaild position for popup.", "CursorFollow", 1);
        }

        private void Awake()
        {
            if (_rt == null) _rt = GetComponent<RectTransform>();
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                FollowMouse();
                if (!_rt.IsFullyVisibleFrom()) PositionRelativeToEdges();
            }
        }
    }
}
