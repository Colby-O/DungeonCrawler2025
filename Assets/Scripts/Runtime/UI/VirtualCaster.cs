using DC2025.Utils;
using PlazmaGames.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DC2025
{
    public class VirtualCaster : GraphicRaycaster
    {
        [SerializeField] private Camera _screenCamera;

        public void CheckForHit(Vector3 pos)
        {
            Ray ray = _screenCamera.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform != null)
                {
                    if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
                    {
                        if (
                            Mathf.FloorToInt(
                                Vector3.Distance(hit.transform.position.SetY(0), DCGameManager.Player.transform.position.SetY(0)) / GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().x
                            ) <= DCGameManager.Player.GetClickDistance()
                        )
                        {
                            DCGameManager.isHovering = true;
                            if (Mouse.current.leftButton.wasPressedThisFrame) interactable.OnPressedDown();
                            else if (Mouse.current.leftButton.wasReleasedThisFrame) interactable.OnPressedUp();
                            else interactable.OnHover();
                        }
                    }
                }
            }
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            Ray ray = eventCamera.ScreenPointToRay(eventData.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform == transform)
                {
                    Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
                    virtualPos.x *= _screenCamera.targetTexture.width;
                    virtualPos.y *= _screenCamera.targetTexture.height;

                    CheckForHit(virtualPos);
                }
            }
        }
    }
}
