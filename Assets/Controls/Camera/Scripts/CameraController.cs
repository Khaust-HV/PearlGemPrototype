using UnityEngine;

public sealed class CameraController : MonoBehaviour, IControlTheCamera {
    [SerializeField] private LineRenderer _lrAimLine;
    [SerializeField] private Transform _trNextBallPoint;

    public LineRenderer GetAimLine() {
        return _lrAimLine;
    }

    public Vector3 GetNextBallPosition() {
        return _trNextBallPoint.position;
    }
}

public interface IControlTheCamera {
    public LineRenderer GetAimLine();
    public Vector3 GetNextBallPosition();
}