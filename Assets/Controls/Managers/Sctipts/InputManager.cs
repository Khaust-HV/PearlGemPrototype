using UnityEngine;
using Zenject;

namespace Managers {
    public sealed class InputManager : ISwitchGameplayInput {
        private TouchscreenInputActions _touchscreenInputActions;

        #region DI
            private IThrowingBalls _iThrowingBalls;
        #endregion

        [Inject]
        private void Construct(IThrowingBalls iThrowingBalls) {
            // Set DI
            _iThrowingBalls = iThrowingBalls;

            _touchscreenInputActions = new TouchscreenInputActions(new InputMap());
        }

        public void SetGameplayInputActionMapActive(bool isActive) {
            _touchscreenInputActions.SetGameplayInputActive(isActive);
        }

        public void SetAllGameplayActive(bool isActive) {
            if (isActive) {
                _touchscreenInputActions.FirstTouchContact += SetFirstTouchActive;
                _touchscreenInputActions.SingleSwipeOnScreenDelta += SingleSwipeDelta;
            } else {
                _touchscreenInputActions.FirstTouchContact -= SetFirstTouchActive;
                _touchscreenInputActions.SingleSwipeOnScreenDelta -= SingleSwipeDelta;
            }
        }

        private void SetFirstTouchActive(bool isActive) {
            if (isActive) {
                _iThrowingBalls.SetAimLineActive(true);
            } else {
                _iThrowingBalls.SetAimLineActive(false);
                _iThrowingBalls.ThrowBall();
            }
        }

        private void SingleSwipeDelta(Vector2 swipeDelta) {
            _iThrowingBalls.SetThrowingDirectionAndPower(swipeDelta);
        }
    }   
}

public interface ISwitchGameplayInput {
    public void SetGameplayInputActionMapActive(bool isActive);
    public void SetAllGameplayActive(bool isActive);
}