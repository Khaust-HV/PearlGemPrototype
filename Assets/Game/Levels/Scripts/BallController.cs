using System.Collections;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class BallController : MonoBehaviour, IControlTheBall {
    private bool _isBallActive;

    private BallColorType _ballColorType;
    private BallMaterialControl _ballMaterialControl;

    private Rigidbody _rbBall;
    private MeshRenderer _mrBall;
    private SphereCollider _scBall;

    #region DI
        private BallThrowingConfigs _ballThrowingConfigs;
        private IControlBallsPool _iControlBallPool;
    #endregion

    [Inject]
    private void Construct(BallThrowingConfigs ballThrowingConfigs, IControlBallsPool iControlBallPool) {
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

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public void SetPosition(Vector3 position, Transform parentObject) {
        transform.SetParent(parentObject);
        transform.localPosition = position;
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
        SetPhysicsActive(false);

        StopAllCoroutines();

        StartCoroutine(BallDestroyStarted());
    }

    private IEnumerator BallDestroyStarted() {
        yield return new WaitForSeconds(_ballThrowingConfigs.BallDestroyTime);

        RestoreAndHide();
    }

    public void SetPhysicsActive(bool isActive) {
        if (isActive) {
            _rbBall.isKinematic = false;

            StartCoroutine(BallLifeStarted());
        } else {
            _rbBall.isKinematic = true;
            _scBall.enabled = false;
        }
    }

    private IEnumerator BallLifeStarted() {
        yield return new WaitForSeconds(_ballThrowingConfigs.BallLifeTime);

        BallDestroyEnable();
    }

    public void SetForceTheBall(Vector3 direction, float forcePower) {
        _scBall.enabled = true;

        _rbBall.AddForce(direction * forcePower, ForceMode.Impulse);
    }

    private void RestoreAndHide() {
        _mrBall.enabled = false;

        transform.SetParent(_iControlBallPool.GetBallPoolTransform());
        transform.position = Vector3.zero;

        gameObject.SetActive(false);

        _isBallActive = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("Balls")) {
            // Collision with ball
            
            BallDestroyEnable();
        } else if (other.collider.CompareTag("Plane")) {
            BallDestroyEnable();
        }
    }
}

public interface IControlTheBall {
    public bool IsBallActive();
    public void SetBallColorType(BallColorType colorType);
    public BallColorType GetBallColorType();
    public void SetPosition(Vector3 position);
    public void SetPosition(Vector3 position, Transform parentObject);
    public void SetBallSize(float size);
    public void BallSpawnEnable();
    public void BallDestroyEnable();
    public void SetPhysicsActive(bool isActive);
    public void SetForceTheBall(Vector3 direction, float forcePower);
}