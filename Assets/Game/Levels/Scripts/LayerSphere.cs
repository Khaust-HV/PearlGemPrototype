using System.Collections;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class LayerSphere : MonoBehaviour, IControlTheLayerSphere {
    private bool _isLayerSphereActive;

    private IControlTheSegmentLayer[] _currentSegments;

    #region DI
        private IControlTheBallsPool _iControlTheBallsPool;
        private SphereOfBallsConfigs _sphereOfBallsConfigs;
    #endregion

    [Inject]
    private void Construct(SphereOfBallsConfigs sphereOfBallsConfigs, IControlTheBallsPool iControlBallsPool) {
        // Set DI
        _iControlTheBallsPool = iControlBallsPool;
        _sphereOfBallsConfigs = sphereOfBallsConfigs;
    }

    public bool IsLayerSphereActive() {
        return _isLayerSphereActive;
    }

    public void LayerSphereEnable() {
        foreach (var segment in _currentSegments) {
            segment.SpawnSegmentBalls();
        }

        StartCoroutine(LayerSphereRotationStarted((LayerSphereRotateDirection)Random.Range(0, 2)));
    }

    private IEnumerator LayerSphereRotationStarted(LayerSphereRotateDirection direction) {
        Vector3 rotationAxis = direction switch {
            LayerSphereRotateDirection.Left => new Vector3(0f, 1f, 0f),
            LayerSphereRotateDirection.Right => new Vector3(0f, -1f, 0f),
            _ => throw new System.Exception("Incorrect direction for sphere rotation")
        };

        float rotationSpeed = Random.Range(_sphereOfBallsConfigs.MinLayerRotationSpeedSphere, _sphereOfBallsConfigs.MaxLayerRotationSpeedSphere);

        while (true) {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);

            yield return null;
        }
    }


    public void PlaceBallsOnLayerSphere(int ballsNumber, float sphereRadius, IControlTheSegmentLayer[] segments) {
        _isLayerSphereActive = true;

        _currentSegments = segments;

        int ballsPerSegment = ballsNumber / segments.Length;
        int extraBalls = ballsNumber % segments.Length;

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = 2 * Mathf.PI / goldenRatio;

        int ballIndex = 0;

        for (int segmentIndex = 0; segmentIndex < segments.Length; segmentIndex++) {
            int currentSegmentBallCount = ballsPerSegment + (segmentIndex < extraBalls ? 1 : 0);

            segments[segmentIndex].SetObjectParent(transform);

            BallColorType colorType = (BallColorType)Random.Range(0, 5);

            for (int i = 0; i < currentSegmentBallCount; i++) {
                float t = (float)(ballIndex + 0.5f) / ballsNumber;

                float verticalAngle = Mathf.Acos(1 - 2 * t);
                float horizontalAngle = angleIncrement * ballIndex;

                float x = sphereRadius * Mathf.Sin(verticalAngle) * Mathf.Cos(horizontalAngle);
                float y = sphereRadius * Mathf.Sin(verticalAngle) * Mathf.Sin(horizontalAngle);
                float z = sphereRadius * Mathf.Cos(verticalAngle);

                Vector3 ballPosition = transform.position + new Vector3(x, y, z);

                var ball = _iControlTheBallsPool.GetDisableBall();
                ball.SetBallColorType(colorType);
                ball.SetColliderActive(true);
                ball.SetBallLayer(3);

                segments[segmentIndex].SetBallToSegment(ball, ballPosition);

                ballIndex++;
            }
        }
    }

    public void DestroyLayerSphere() {
        StopAllCoroutines();

        foreach (var segment in _currentSegments) {
            segment.DestroySegment();
        }

        _isLayerSphereActive = false;
    }
}

public interface IControlTheLayerSphere  {
    public bool IsLayerSphereActive();
    public void LayerSphereEnable();
    public void PlaceBallsOnLayerSphere(int ballsNumber, float sphereRadius, IControlTheSegmentLayer[] segments);
    public void DestroyLayerSphere();
}

public enum LayerSphereRotateDirection {
    Left,
    Right
}