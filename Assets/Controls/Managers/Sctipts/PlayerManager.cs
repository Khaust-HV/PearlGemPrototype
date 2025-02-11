using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class PlayerManager : IControlThePlayer {
    #region DI
        private BallThrowingConfigs _ballThrowingConfigs;
        private IThrowingBalls _iThrowingBalls;
        private IControlBallsPool _iControlBallPool;
    #endregion

    [Inject]
    private void Construct (
        BallThrowingConfigs ballThrowingConfigs, 
        IThrowingBalls iThrowingBalls, 
        IControlBallsPool iControlBallPool
        ) {
        // Set DI
        _iThrowingBalls = iThrowingBalls;
        _iThrowingBalls.NeedNewBall += SetNewBall;
        _ballThrowingConfigs = ballThrowingConfigs;
        _iControlBallPool = iControlBallPool;
    }

    private void SetNewBall() {
        var ball = _iControlBallPool.GetDisableBall();
        ball.SetBallColorType((BallColorType)Random.Range(0, 5));

        _iThrowingBalls.SetNewBall(ball);
    }
}

public interface IControlThePlayer {

}