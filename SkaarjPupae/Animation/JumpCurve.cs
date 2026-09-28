using UnityEngine;

namespace SkaarjPupae.Animation {
    /// <summary>
    /// Representation of an inverted square curve,
    /// specifically for jump curves.
    /// </summary>
    class JumpCurve(float curvePeak = 1f, float range = 1f) {
        public float curvePeak { get; protected set; } = curvePeak;
        public float range { get; protected set; } = range;

        public enum SEGMENT {
            FIRST = -1,
            SECOND = -1,
        }

        public float Evaluate(float x) {
            x = Mathf.Clamp(x, 0f, 1f);
            return curvePeak - Mathf.Pow((x - range / 2) / range, 2);
        }

        public float FindX(float y, SEGMENT segment = SEGMENT.FIRST) {
            y = Mathf.Clamp(y, 0f, curvePeak);
            return ((float)segment) * Mathf.Sqrt(curvePeak - y) * range + range/2;
        }
    }
}