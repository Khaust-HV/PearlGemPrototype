using UnityEngine;

namespace GameConfigs {
    [CreateAssetMenu(menuName = "Configs/SphereOfBallsConfigs", fileName = "SphereOfBallsConfigs")]
    public sealed class SphereOfBallsConfigs : ScriptableObject {
        
        [field: Header("Layer sphere settings")]
        [field: SerializeField] public GameObject LayerSpherePrefab { get; private set; }
        [field: SerializeField] public Vector3 SpherePosition { get; private set; }
        [field: SerializeField] public int MinNumberOfLayersSphere { get; private set; }
        [field: SerializeField] public int MaxNumberOfLayersSphere { get; private set; }
        [field: SerializeField] public float StartRadiusSphere { get; private set; }
        [field: SerializeField] public float LayerDistanceSphere { get; private set; }
        [field: SerializeField] public float MinLayerRotationSpeedSphere { get; private set; }
        [field: SerializeField] public float MaxLayerRotationSpeedSphere { get; private set; }
        [field: Space(10)]

        [field: Header("Segments Layer settings")]
        [field: SerializeField] public GameObject SegmentLayerPrefab { get; private set; }
        [field: SerializeField] public int MinNumberOfSegmentsOnOneLayer { get; private set; }
        [field: SerializeField] public int MaxNumberOfSegmentsOnOneLayer { get; private set; }
        [field: SerializeField] public float NeighborDistanceToBalls { get; private set; }
        [field: SerializeField] public float SegmentDestoryTime { get; private set; }
    }
}