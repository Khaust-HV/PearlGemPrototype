using UnityEngine;
using Zenject;

public sealed class Bootstrap : MonoBehaviour { // Entry point
    #region DI
        private ISwitchGameplayInput _iSwitchGameplayInput;
        private IThrowingBalls _iThrowingBalls;
        private IControlTheBallsPool _iControlBallPool;
        private IControlThePlayer _iControlThePlayer;
    #endregion

    [Inject]
    private void Construct (
        IThrowingBalls iThrowingBalls, 
        ISwitchGameplayInput iSwitchGameplayInput, 
        IControlTheBallsPool iControlTheBallPool,
        IControlThePlayer iControlThePlayer
        ) {
        // Set DI
        _iThrowingBalls = iThrowingBalls;
        _iSwitchGameplayInput = iSwitchGameplayInput;
        _iControlBallPool = iControlTheBallPool;
        _iControlThePlayer = iControlThePlayer;
    }

    private void Awake() {
        // Generate level
        _iControlBallPool.CreateBalls(50);

        Physics.IgnoreLayerCollision(3, 6, true);

        _iControlThePlayer.StartTheGame();

        // Set control
        _iSwitchGameplayInput.SetGameplayInputActionMapActive(true);
        _iSwitchGameplayInput.SetAllGameplayActive(true);
    }
}