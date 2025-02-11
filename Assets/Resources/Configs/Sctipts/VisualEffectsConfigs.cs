using UnityEngine;

namespace GameConfigs {
    [CreateAssetMenu(menuName = "Configs/VisualEffectsConfigs", fileName = "VisualEffectsConfigs")]
    public sealed class VisualEffectsConfigs : ScriptableObject {
        [field: Header("Base material settings")]
        [field: SerializeField] public Material BallMaterial { get; private set; }
    }
}