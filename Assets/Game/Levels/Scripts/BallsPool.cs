using System.Collections.Generic;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class BallsPool : IControlTheBallsPool { // Objects pool
    private Transform _ballsPool;

    private List<IControlTheBall> ballsList = new();

    #region DI
        private DiContainer _container;
        private BallThrowingConfigs _ballThrowingConfigs;
    #endregion

    [Inject]
    private void Construct(DiContainer container, BallThrowingConfigs ballThrowingConfigs) {
        // Set DI
        _container = container;
        _ballThrowingConfigs = ballThrowingConfigs;

        _ballsPool = new GameObject("BallPool").transform;
    }

    public Transform GetBallPoolTransform() {
        return _ballsPool;
    }

    public IControlTheBall GetDisableBall() {
        foreach (var ball in ballsList) {
            if (!ball.IsBallActive()) {
                return ball;
            }
        }

        CreateBalls(10);

        return GetDisableBall();
    }

    public void CreateBalls(int number) { // Factory method
        for (int i = 0; i < number; i++) {
            GameObject obj = _container.InstantiatePrefab(_ballThrowingConfigs.BallPrefab, Vector3.zero, Quaternion.identity, _ballsPool);

            obj.SetActive(false);

            Component[] components = obj.GetComponents<Component>();
            foreach (Component comp in components) {
                if (comp is IControlTheBall ball) {
                    ballsList.Add(ball);
                    break;
                }
            }
        }
    }
}

public interface IControlTheBallsPool {
    public Transform GetBallPoolTransform();
    public IControlTheBall GetDisableBall();
    public void CreateBalls(int number);
}