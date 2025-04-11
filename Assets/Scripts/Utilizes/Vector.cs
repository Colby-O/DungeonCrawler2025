using UnityEngine;

namespace DC2025.Utils
{
    public static class Vector
    {
        public static Vector3 SetX(this Vector3 vec, float val) => new Vector3(val, vec.y, vec.z);
        public static Vector3 SetY(this Vector3 vec, float val) => new Vector3(vec.x, val, vec.z);
        public static Vector3 SetZ(this Vector3 vec, float val) => new Vector3(vec.x, vec.y, val);
        public static Vector3 CastToXZPlane(this Vector2 vec, float y = 0) => new Vector3(vec.x, y, vec.y);
    }
}
