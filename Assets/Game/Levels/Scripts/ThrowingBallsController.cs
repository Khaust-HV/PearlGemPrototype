using System;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class ThrowingBallsController : IThrowingBalls {
    #region Ball Throwing Configs
        private float _currentBallSize;
        private float _nextBallSize;
        private float _ballMass;

        private float _aimSensitivity;
        private float _minHorizontalAimValue;
        private float _maxHorizontalAimValue;
        private float _minVerticalAimValue;
        private float _maxVerticalAimValue;

        private float _throwingPowerSensitivity;
        private float _minThrowingPower;
        private float _maxThrowingPower;

        private int _aimLinePointsNumber;
        private float _aimLineTimeStep;
    #endregion

    public event Action NeedNewBall;

    private Vector3 _currentBallPosition;
    private Vector3 _nextBallPosition;

    private LineRenderer _lrAimLine;
    private Transform _trAimLine;
    private float _horizontalAimValue;
    private float _verticalAimValue;
    private float _oldVerticalAimValue;

    private IControlTheBall _currentBall;
    private IControlTheBall _nextBall;

    private float _currentThrowingPower;

    #region DI
        private BallThrowingConfigs _ballThrowingConfigs;
        private IControlTheCamera _iControlTheCamera;
    #endregion

    [Inject]
    private void Construct(BallThrowingConfigs ballThrowingConfigs, IControlTheCamera iControlTheCamera) {
        // Set DI
        _ballThrowingConfigs = ballThrowingConfigs;
        _iControlTheCamera = iControlTheCamera;

        // Set configurations
        _currentBallSize = _ballThrowingConfigs.CurrentBallSize;
        _nextBallSize = _ballThrowingConfigs.NextBallSize;
        _ballMass = _ballThrowingConfigs.BallMass;

        _aimSensitivity = _ballThrowingConfigs.AimSensitivity;
        _minHorizontalAimValue = _ballThrowingConfigs.MinHorizontalAimValue;
        _maxHorizontalAimValue = _ballThrowingConfigs.MaxHorizontalAimValue;
        _minVerticalAimValue = _ballThrowingConfigs.MinVerticalAimValue;
        _maxVerticalAimValue = _ballThrowingConfigs.MaxVerticalAimValue;

        _throwingPowerSensitivity = _ballThrowingConfigs.ThrowingPowerSensitivity;
        _minThrowingPower = _ballThrowingConfigs.MinThrowingPower;
        _maxThrowingPower = _ballThrowingConfigs.MaxThrowingPower;

        _aimLinePointsNumber = _ballThrowingConfigs.AimLinePointsNumber;
        _aimLineTimeStep = _ballThrowingConfigs.AimLineTimeStep;

        // Set components
        _lrAimLine = _iControlTheCamera.GetAimLine();
        _lrAimLine.enabled = false;

        _trAimLine = _lrAimLine.GetComponent<Transform>();

    }

    public void SetThrowingPosition() {
        _currentBallPosition = _trAimLine.position;
        _nextBallPosition = _iControlTheCamera.GetNextBallPosition();
    }

    public void SetAbilityToThrowBallsActive(bool isActive) {
        if (isActive) {
            NeedNewBall?.Invoke();
        } else {
            _currentBall.BallDestroyEnable();
            _nextBall.BallDestroyEnable();

            _currentBall = null;
            _nextBall = null;
        }
    }

    public void SetNewBall(IControlTheBall iControlTheBall) {
        if (_currentBall == null) {
            _currentBall = iControlTheBall;
            _currentBall.SetPosition(_currentBallPosition);
            _currentBall.SetBallSize(_currentBallSize);
            _currentBall.BallSpawnEnable();

            _lrAimLine.colorGradient = _currentBall.GetBallColorType() switch {
                BallColorType.Black => GetGradient(Color.black),
                BallColorType.Red => GetGradient(Color.red),
                BallColorType.Green => GetGradient(Color.green),
                BallColorType.Blue => GetGradient(Color.blue),
                BallColorType.Cyan => GetGradient(Color.cyan),
                _ => throw new System.Exception("Invalid color type")
            };

            NeedNewBall?.Invoke();
        } else {
            _nextBall = iControlTheBall;
            _nextBall.SetPosition(_nextBallPosition);
            _nextBall.SetBallSize(_nextBallSize);
            _nextBall.BallSpawnEnable();
        }
    }

    public void SetAimLineActive(bool isActive) {
        _lrAimLine.enabled = isActive;
    }

    public void ThrowBall() {
        Vector3 direction = (Quaternion.Euler(_verticalAimValue, _horizontalAimValue, 0f) * Vector3.forward).normalized;

        _currentBall.SetBallLayer(3);
        _currentBall.SetColliderActive(true);
        _currentBall.SetPhysicsActive(true);
        _currentBall.SetForceTheBall(direction, _currentThrowingPower);

        Debug.Log($"The ball is thrown with a force of {_currentThrowingPower}");

        _currentBall = _nextBall;

        _currentBall.SetPosition(_currentBallPosition);
        _currentBall.SetBallSize(_currentBallSize);
        _currentBall.BallSpawnEnable();

        _lrAimLine.colorGradient = _currentBall.GetBallColorType() switch {
            BallColorType.Black => GetGradient(Color.black),
            BallColorType.Red => GetGradient(Color.red),
            BallColorType.Green => GetGradient(Color.green),
            BallColorType.Blue => GetGradient(Color.blue),
            BallColorType.Cyan => GetGradient(Color.cyan),
            _ => throw new System.Exception("Invalid color type")
        };

        _nextBall = null;

        _currentThrowingPower = 0f;
        _horizontalAimValue = 0f;
        _verticalAimValue = 0f;
        _oldVerticalAimValue = 0f;

        _lrAimLine.positionCount = 0;

        NeedNewBall?.Invoke();
    }

    public void SetThrowingDirectionAndPower(Vector2 swipeDelta) {
        float timeDelta = Time.deltaTime;

        _horizontalAimValue += swipeDelta.x * _aimSensitivity * timeDelta;
        _verticalAimValue += swipeDelta.y * _aimSensitivity * timeDelta;

        _horizontalAimValue = Mathf.Clamp(_horizontalAimValue, _minHorizontalAimValue, _maxHorizontalAimValue);
        _verticalAimValue = Mathf.Clamp(_verticalAimValue, _minVerticalAimValue, _maxVerticalAimValue);

        float angleDelta = _verticalAimValue - _oldVerticalAimValue + 15f;
        _currentThrowingPower -= angleDelta * _throwingPowerSensitivity * timeDelta;
        _currentThrowingPower = Mathf.Clamp(_currentThrowingPower, _minThrowingPower, _maxThrowingPower);

        Debug.Log($"Vertical: {_verticalAimValue}, Power: {_currentThrowingPower}");

        DrawAimLine();
    }

    private void DrawAimLine() {
        Vector3 velocity = CalculateLaunchVelocity();

        Vector3 currentPosition = _trAimLine.position;

        _lrAimLine.positionCount = _aimLinePointsNumber;

        float gravity = Mathf.Abs(Physics.gravity.y);
        
        for (int i = 0; i < _aimLinePointsNumber; i++) {
            _lrAimLine.SetPosition(i, currentPosition);
            
            velocity.y -= gravity * _aimLineTimeStep;
            currentPosition += velocity * _aimLineTimeStep;
        }
    }

    private Vector3 CalculateLaunchVelocity() {
        float forceToVelocity = _currentThrowingPower / _ballMass;
        
        Vector3 direction = Quaternion.Euler(_verticalAimValue, _horizontalAimValue, 0f) * Vector3.forward;

        return direction.normalized * forceToVelocity;
    }

    Gradient GetGradient(Color color) { // Interim solution
        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 1f), new GradientColorKey(color, 0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 1f), new GradientAlphaKey(color.a, 0f) }
        );

        return gradient;
    }
}

public interface IThrowingBalls {
    public void SetThrowingPosition();
    public void SetAbilityToThrowBallsActive(bool isActive);
    public event Action NeedNewBall;
    public void SetNewBall(IControlTheBall iControlTheBall);
    public void SetAimLineActive(bool isActive);
    public void ThrowBall();
    public void SetThrowingDirectionAndPower(Vector2 swipeDelta);
}