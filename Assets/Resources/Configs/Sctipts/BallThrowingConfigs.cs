using UnityEngine;

namespace GameConfigs {
    [CreateAssetMenu(menuName = "Configs/BallThrowingConfigs", fileName = "BallThrowingConfigs")]
    public sealed class BallThrowingConfigs : ScriptableObject {
        [field: Header("Ball settings")]
        [field: SerializeField] public GameObject BallPrefab { get; private set; }
        [field: SerializeField] public float CurrentBallSize { get; private set; }
        [field: SerializeField] public float NextBallSize { get; private set; }
        [field: SerializeField] public float BallMass { get; private set; }
        [field: SerializeField] public float BallLifeTime { get; private set; }
        [field: SerializeField] public float BallSpawnTime { get; private set; }
        [field: SerializeField] public float BallDestroyTime { get; private set; }
        [field: Space(10)]

        [field: Header("Ball throw settings")]
        [field: SerializeField] public float AimSensitivity { get; private set; }
        [field: SerializeField] public float MinHorizontalAimValue { get; private set; }
        [field: SerializeField] public float MaxHorizontalAimValue { get; private set; }
        [field: SerializeField] public float MinVerticalAimValue { get; private set; }
        [field: SerializeField] public float MaxVerticalAimValue { get; private set; }
        [field: Space(10)]
        [field: SerializeField] public float ThrowingPowerSensitivity { get; private set; }
        [field: SerializeField] public float MinThrowingPower { get; private set; }
        [field: SerializeField] public float MaxThrowingPower { get; private set; }
        [field: Space(10)]

        [field: Header("Aim line settings")]
        [field: SerializeField] public int AimLinePointsNumber { get; private set; }
        [field: SerializeField] public float AimLineTimeStep { get; private set; }
    }
}