using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogoPhysicsAnimator : MonoBehaviour {
    [System.Serializable]
    public class LogoCube {
        public Transform cube;
        [HideInInspector] public Vector3 targetPosition;
        [HideInInspector] public Quaternion targetRotation;
        [HideInInspector] public Rigidbody rb;
    }

    public List<LogoCube> cubes;
    public float launchForce = 6f;
    public float chaosTime = 2f;
    public float settleDuration = 1.5f;
    public float overshootDistance = 0.5f;
    public float targetOffsetFactor = 0.2f;
    public Vector2 spawnArea = new Vector2(10f, 6f);

    private void Start() {
        foreach (var c in cubes) {
            c.targetPosition = c.cube.localPosition;
            c.targetRotation = c.cube.localRotation;
            c.rb = c.cube.GetComponent<Rigidbody>();

            Vector3 direction = (c.targetPosition - Vector3.zero).normalized;
            Vector3 spawnPos = c.targetPosition + direction * overshootDistance;
            spawnPos.x = Mathf.Sign(spawnPos.x) * Mathf.Max(Mathf.Abs(spawnPos.x), spawnArea.x);
            spawnPos.y = Mathf.Clamp(spawnPos.y, -spawnArea.y, spawnArea.y);

            c.cube.localPosition = spawnPos;
            c.cube.localRotation = Random.rotation;

            c.rb.isKinematic = false;
            c.rb.useGravity = false;

            Vector3 toTarget = (c.targetPosition - spawnPos).normalized;
            c.rb.AddForce(toTarget * launchForce, ForceMode.VelocityChange);
        }

        StartCoroutine(SettleAfterChaos());
    }

    private IEnumerator SettleAfterChaos() {
        yield return new WaitForSeconds(chaosTime);

        foreach (var c in cubes) {
            c.rb.linearVelocity = Vector3.zero;
            c.rb.angularVelocity = Vector3.zero;
            c.rb.isKinematic = true;
        }

        float t = 0f;
        while (t < settleDuration) {
            t += Time.deltaTime;
            float lerpT = t / settleDuration;
            foreach (var c in cubes) {
                c.cube.localPosition = Vector3.Lerp(c.cube.localPosition, c.targetPosition, lerpT);
                c.cube.localRotation = Quaternion.Slerp(c.cube.localRotation, c.targetRotation, lerpT);
            }
            yield return null;
        }
    }
}
