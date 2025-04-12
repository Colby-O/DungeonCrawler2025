using UnityEngine;

namespace DC2025.Utils
{
    public static class Math
    {
        public static float NormalizeAngle360(float ang)
        {
            float v = Mathf.Repeat(ang, 360f);
            return v;
        }
        public static float NormalizeAngle(float ang)
        {
            float v = Mathf.Repeat(ang, 360f);
            return v < 180f ? v : v - 360f;
        }
        public static float Map(float v, float a, float b, float c, float d) => Mathf.Lerp(c, d, Mathf.InverseLerp(a, b, v));
    }
}
