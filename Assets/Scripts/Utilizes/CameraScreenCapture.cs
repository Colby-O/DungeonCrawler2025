using System.Collections.Generic;
using System.Linq;
using PlazmaGames.Utilities;
using UnityEditor.Embree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DC2025
{
    using PlazmaGames.Core;
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;

    public class CameraCapture : MonoBehaviour
    {
        public string fileName;
        public KeyCode screenshotKey;
        public Transform _itemParent;

        private Camera Camera
        {
            get
            {
                if (!_camera)
                {
                    _camera = GetComponent<Camera>();
                }
                return _camera;
            }
        }
        private Camera _camera;

        private void LateUpdate()
        {
            if (Input.GetKeyDown(screenshotKey)) Capture();
        }

        public void Capture()
        {
            RenderTexture activeRenderTexture = RenderTexture.active;
            RenderTexture.active = Camera.targetTexture;

            Camera.Render();

            Texture2D image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height, TextureFormat.RGBA32_SIGNED, false, true);
            image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = activeRenderTexture;

            byte[] bytes = image.EncodeToPNG();
            Destroy(image);

            string fn = fileName;

            /*
            Transform items = transform.parent.Find("Items");
            fn = items.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject != items.gameObject)?.gameObject.name ?? "";
            if (fn == "") fn = fileName;
            */

            string path = Application.dataPath + "/Resources/Icons/" + fn + ".png";
            Debug.Log($"Saving icon to {path}.");
            File.WriteAllBytes(path, bytes);
        }

        private void Start()
        {
            return;
            List<HandleType> handles = new();
            handles.Add(HandleType.Balanced);
            handles.Add(HandleType.Dominant);
            handles.Add(HandleType.Lightweight);
            handles.Add(HandleType.Rugged);
            handles.Add(HandleType.Wise);
            List<MaterialType> mats = new();
            mats.Add(MaterialType.Bronze);
            mats.Add(MaterialType.Iron);
            mats.Add(MaterialType.Steel);
            mats.Add(MaterialType.Cobalt);

            Transform b = transform.parent.GetChild(0).GetChild(0);
            Transform items = transform.parent.Find("Items");

            foreach (HandleType h in handles)
            {
                foreach (MaterialType m in mats)
                {
                    WeaponItem w = GameManager.GetMonoSystem<ISwordBuilderMonoSystem>().CreateSword(BladeType.Axe, h, m, 4);
                    w.transform.position = b.position;
                    w.transform.rotation = b.rotation;
                    w.gameObject.SetGameLayerRecursive(LayerMask.NameToLayer("Screenshot"));
                    w.gameObject.name = $"Axe_{h}_{m}";
                    w.transform.parent = items;
                }
                
            }
        }
    }
}
