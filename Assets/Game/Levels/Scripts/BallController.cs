using System;
using System.Collections;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class BallController : MonoBehaviour, IControlTheBall {
    public event Action<IControlTheBall> HitTheRightBall;

    private bool _isBallActive;

    private BallColorType _ballColorType;
    private BallMaterialControl _ballMaterialControl;

    private bool isThrownBall;

    private Rigidbody _rbBall;
    private MeshRenderer _mrBall;
    private SphereCollider _scBall;

    #region DI
        private BallThrowingConfigs _ballThrowingConfigs;
        private IControlTheBallsPool _iControlBallPool;
    #endregion

    [Inject]
    private void Construct(BallThrowingConfigs ballThrowingConfigs, IControlTheBallsPool iControlBallPool) {
        // Set DI
        _ballThrowingConfigs = ballThrowingConfigs;
        _iControlBallPool = iControlBallPool;

        // Set components
        _rbBall = GetComponent<Rigidbody>();
        _rbBall.mass = _ballThrowingConfigs.BallMass;
        _rbBall.isKinematic = true;

        _mrBall = GetComponent<MeshRenderer>();
        _mrBall.enabled = false;

        _scBall = GetComponent<SphereCollider>();
        _scBall.enabled = false;

        _ballMaterialControl = GetComponent<BallMaterialControl>();
    }

    public bool IsBallActive() {
        return _isBallActive;
    }

    public void SetBallColorType(BallColorType colorType) {
        _isBallActive = true;

        _ballColorType = colorType;

        _ballMaterialControl.SetColorType(_ballColorType);
    }

    public BallColorType GetBallColorType() {
        return _ballColorType;
    }

    public Vector3 GetBallPosition() {
        return transform.position;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public void SetPosition(Vector3 position, Transform parentObject) {
        transform.SetParent(parentObject);
        transform.localPosition = position;
    }

    public void SetParent(Transform parentObject) {
        transform.SetParent(parentObject);
    }

    public void SetBallSize(float size) {
        transform.localScale = new Vector3(size, size, size);
    }

    public void BallSpawnEnable() {
        gameObject.SetActive(true);

        StartCoroutine(BallSpawnStarted());
    }

    private IEnumerator BallSpawnStarted() {
        yield return new WaitForSeconds(_ballThrowingConfigs.BallSpawnTime);

        _mrBall.enabled = true;
    }

    public void BallDestroyEnable() {
        SetColliderActive(false);
        SetPhysicsActive(false);

        isThrownBall = false;

        StopAllCoroutines();

        StartCoroutine(BallDestroyStarted());
    }

    private IEnumerator BallDestroyStarted() {
        yield return new WaitForSeconds(_ballThrowingConfigs.BallDestroyTime);

        RestoreAndHide();
    }

    public void SetBallLayer(int layerIndex) {
        gameObject.layer = layerIndex;
    }

    public void SetPhysicsActive(bool isActive) {
        if (isActive) {
            _rbBall.isKinematic = false;

            StartCoroutine(BallLifeStarted());
        } else {
            _rbBall.isKinematic = true;
        }
    }

    public void SetColliderActive(bool isActive) {
        _scBall.enabled = isActive;
    }

    private IEnumerator BallLifeStarted() {
        yield return new WaitForSeconds(_ballThrowingConfigs.BallLifeTime);

        BallDestroyEnable();
    }

    public void SetForceTheBall(Vector3 direction, float forcePower) {
        _rbBall.AddForce(direction * forcePower, ForceMode.Impulse);

        isThrownBall = true;
    }

    private void RestoreAndHide() {
        _mrBall.enabled = false;

        transform.SetParent(_iControlBallPool.GetBallPoolTransform());
        transform.position = Vector3.zero;

        gameObject.SetActive(false);

        _isBallActive = false;
    }

    private void OnCollisionEnter(Collision other) {
        Collider cObj = other.collider;

        if (cObj.CompareTag("Balls")) {
            if (!isThrownBall) {
                if (cObj.GetComponent<BallController>().GetBallColorType() == _ballColorType) HitTheRightBall?.Invoke(this);
            } else BallDestroyEnable();
        } else if (cObj.CompareTag("Plane")) {
            BallDestroyEnable();
        }
    }
}

public interface IControlTheBall {
    public event Action<IControlTheBall> HitTheRightBall;
    public bool IsBallActive();
    public void SetBallColorType(BallColorType colorType);
    public BallColorType GetBallColorType();
    public Vector3 GetBallPosition();
    public void SetPosition(Vector3 position);
    public void SetPosition(Vector3 position, Transform parentObject);
    public void SetParent(Transform parentObject);
    public void SetBallSize(float size);
    public void BallSpawnEnable();
    public void BallDestroyEnable();
    public void SetBallLayer(int layerIndex);
    public void SetPhysicsActive(bool isActive);
    public void SetColliderActive(bool isActive);
    public void SetForceTheBall(Vector3 direction, float forcePower);
}