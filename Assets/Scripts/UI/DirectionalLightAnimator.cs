using UnityEngine;
using DG.Tweening;

namespace ph.UI {
    public class DirectionalLightAnimator : MonoBehaviour {
        [Header("Light Rotation Settings")]
        public Vector2 rotationAmplitude = new Vector2(10f, 5f); // max zmiany rotacji w stopniach
        public float cycleDuration = 5f; // czas jednego pełnego cyklu (tam i z powrotem)

        private Vector3 initialRotation;

        private void Start() {
            initialRotation = transform.eulerAngles;

            AnimateLight();
        }
        private void AnimateLight() {
            DOTween.To(() => 0f, x => {
                float xAngle = initialRotation.x + Mathf.Sin(x * Mathf.PI * 2f) * rotationAmplitude.x;
                float yAngle = initialRotation.y + Mathf.Cos(x * Mathf.PI * 2f) * rotationAmplitude.y;
                if (transform != null) transform.rotation = Quaternion.Euler(xAngle, yAngle, initialRotation.z);
                else return;
            }, 1f, cycleDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetTarget(this)
            .SetAutoKill(true); ;
        }
    }
}
