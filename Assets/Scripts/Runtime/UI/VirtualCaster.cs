using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
                    Debug.Log(hit.collider.transform.name);
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
                    eventData.position = virtualPos;

                    CheckForHit(virtualPos);
                }
            }
        }
    }
}
