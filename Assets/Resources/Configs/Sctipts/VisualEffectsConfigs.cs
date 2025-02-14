using UnityEngine;
using UnityEngine.VFX;

namespace GameConfigs {
    [CreateAssetMenu(menuName = "Configs/VisualEffectsConfigs", fileName = "VisualEffectsConfigs")]
    public sealed class VisualEffectsConfigs : ScriptableObject {
        [field: Header("Base material settings")]
        [field: SerializeField] public Material BallMaterial { get; private set; }
        [field: Space(10)]

        [field: Header("Dissolve effect settings")]
        [field: SerializeField] public float NoiseScaleDissolve { get; private set; }
        [field: SerializeField] public float NoiseStrengthDissolve { get; private set; }
        [field: SerializeField] public float EdgeWidthDissolve { get; private set; }
        [field: ColorUsage(true, true)] // For HDR
        [field: SerializeField] public Color EdgeColorDissolve { get; private set; }
        [field: Space(10)]

        [field: Header("Ball effect settings")]
        [field: SerializeField] public VisualEffectAsset BallEffectAsset { get; private set; }
        [field: SerializeField] public Texture2D ParticleTexture { get; private set; }        
        [field: SerializeField] public int ParticlesNumber { get; private set; }
    }
}