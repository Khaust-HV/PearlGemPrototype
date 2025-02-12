using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class PlayerManager : IControlThePlayer {
    #region DI
        private IThrowingBalls _iThrowingBalls;
        private IControlTheBallsPool _iControlTheBallPool;
        private ICreateSphereOfBalls _iCreateSphereOfBalls;
    #endregion

    [Inject]
    private void Construct (
        IThrowingBalls iThrowingBalls, 
        IControlTheBallsPool iControlBallPool,
        ICreateSphereOfBalls iCreateSphereOfBalls
        ) {
        // Set DI
        _iThrowingBalls = iThrowingBalls;
        _iThrowingBalls.NeedNewBall += SetNewBall;
        _iControlTheBallPool = iControlBallPool;
        _iCreateSphereOfBalls = iCreateSphereOfBalls;
    }

    private void SetNewBall() {
        var ball = _iControlTheBallPool.GetDisableBall();
        ball.SetBallColorType((BallColorType)Random.Range(0, 5));

        _iThrowingBalls.SetNewBall(ball);
    }

    public void StartTheGame() {
        _iCreateSphereOfBalls.CreateSphere();
        _iCreateSphereOfBalls.SpawnLayersSphere();

        _iThrowingBalls.SetThrowingPosition();
        _iThrowingBalls.SetAbilityToThrowBallsActive(true);
    }
}

public interface IControlThePlayer {
    public void StartTheGame();
}