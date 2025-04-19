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

            string path = Application.dataPath + "/Resources/Icons/" + fileName + ".png";
            Debug.Log($"Saving icon to {path}.");
            File.WriteAllBytes(path, bytes);
        }
    }
}
