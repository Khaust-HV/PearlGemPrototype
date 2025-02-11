using UnityEngine;
using Zenject;

public sealed class Bootstrap : MonoBehaviour { // Entry point
    #region DI
        private ISwitchGameplayInput _iSwitchGameplayInput;
        private IThrowingBalls _iThrowingBalls;
        private IControlBallsPool _iControlBallPool;
    #endregion

    [Inject]
    private void Construct (
        IThrowingBalls iThrowingBalls, 
        ISwitchGameplayInput iSwitchGameplayInput, 
        IControlBallsPool iControlBallPool
        ) {
        // Set DI
        _iThrowingBalls = iThrowingBalls;
        _iSwitchGameplayInput = iSwitchGameplayInput;
        _iControlBallPool = iControlBallPool;
    }

    private void Awake() {
        // Generate level
        _iControlBallPool.CreateBalls(50);

        _iThrowingBalls.SetThrowingPosition();
        _iThrowingBalls.SetAbilityToThrowBallsActive(true);

        // Set control
        _iSwitchGameplayInput.SetGameplayInputActionMapActive(true);
        _iSwitchGameplayInput.SetAllGameplayActive(true);
    }
}