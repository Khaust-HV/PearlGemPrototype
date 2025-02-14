using System.Collections;
using System.Collections.Generic;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class SegmentLayerSphere : MonoBehaviour, IControlTheSegmentLayer {
    private bool _isSegmentLayerActive;

    #region Sphere Of Balls Configs
        private float _neighborDistanceToBalls;
        private float _segmentDestoryTime;
    #endregion

    private List<IControlTheBall> _ballList = new();

    #region DI
        private SphereOfBallsConfigs _sphereOfBallsConfigs;
        private IControlTheBallsPool _iControlTheBallsPool;
    #endregion

    [Inject]
    private void Construct(SphereOfBallsConfigs sphereOfBallsConfigs, IControlTheBallsPool iControlTheBallsPool) {
        // Set DI
        _sphereOfBallsConfigs = sphereOfBallsConfigs;
        _iControlTheBallsPool = iControlTheBallsPool;

        // Set configurations
        _neighborDistanceToBalls = _sphereOfBallsConfigs.NeighborDistanceToBalls;
        _segmentDestoryTime = _sphereOfBallsConfigs.SegmentDestoryTime;
    }

    public bool IsSegmentLayerActive() {
        return _isSegmentLayerActive;
    }

    public void SetObjectParent(Transform objParent) {
        transform.SetParent(objParent);
    }

    public void SpawnSegmentBalls() {
        foreach (var ball in _ballList) {
            ball.BallSpawnEnable();
        }
    }

    public void SetSegmentEnable() {
        _isSegmentLayerActive = true;
    }

    public void SetBallToSegment(IControlTheBall ball, Vector3 position) {
        _ballList.Add(ball);

        ball.SetPosition(position, transform);
        ball.HitTheRightBall += StartSegmentDestruction;
    }

    public void StartSegmentDestruction(IControlTheBall startBall) {
        foreach (var ball in _ballList) {
            ball.HitTheRightBall -= StartSegmentDestruction;
        }

        StartCoroutine(DestroySegment(startBall));
    }

    private IEnumerator DestroySegment(IControlTheBall startBall) {
        Queue<IControlTheBall> queue = new Queue<IControlTheBall>();
        HashSet<IControlTheBall> visited = new HashSet<IControlTheBall>();

        queue.Enqueue(startBall);
        visited.Add(startBall);

        while (queue.Count > 0) {
            IControlTheBall currentBall = queue.Dequeue();

            currentBall.SetParent(_iControlTheBallsPool.GetBallPoolTransform());
            currentBall.SetBallLayer(6);
            currentBall.BallEffectEnable();
            currentBall.SetPhysicsActive(true);

            yield return new WaitForSeconds(_segmentDestoryTime);

            foreach (var neighbor in GetNeighborBalls(currentBall)) {
                if (!visited.Contains(neighbor)) {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private List<IControlTheBall> GetNeighborBalls(IControlTheBall ball) {
        List<IControlTheBall> neighbors = new List<IControlTheBall>();

        foreach (var otherBall in _ballList) {
            if (otherBall != ball && Vector3.Distance(ball.GetBallPosition(), otherBall.GetBallPosition()) <= _neighborDistanceToBalls) {
                neighbors.Add(otherBall);
            }
        }
        
        return neighbors;
    }

    public void DestroySegment() {
        foreach (var ball in _ballList) {
            ball.BallDestroyEnable();
        }

        _ballList.Clear();

        _isSegmentLayerActive = false;
    }
}

public interface IControlTheSegmentLayer  {
    public bool IsSegmentLayerActive();
    public void SetObjectParent(Transform objParent);
    public void SpawnSegmentBalls();
    public void SetSegmentEnable();
    public void SetBallToSegment(IControlTheBall ball, Vector3 position);
    public void DestroySegment();
}