using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class BallMaterialControl : MonoBehaviour {
    private MeshRenderer _mrBall;

    private MaterialPropertyBlock _ballMaterialPropertyBlock;

    #region DI
        VisualEffectsConfigs _visualEffectsConfigs;
    #endregion

    [Inject]
    private void Construct(VisualEffectsConfigs visualEffectsConfigs) {
        // Set DI
        _visualEffectsConfigs = visualEffectsConfigs;
        
        // Set components
        _ballMaterialPropertyBlock = new MaterialPropertyBlock();

        _mrBall = GetComponent<MeshRenderer>();
        _mrBall.material = _visualEffectsConfigs.BallMaterial;
    }

    public void SetColorType(BallColorType colorType) {
        Color color = colorType switch{
            BallColorType.Black => Color.black,
            BallColorType.Red => Color.red,
            BallColorType.Green => Color.green,
            BallColorType.Blue => Color.blue,
            BallColorType.Cyan => Color.cyan,
            _ => throw new System.Exception("Invalid color type")
        };

        _ballMaterialPropertyBlock.SetColor("_BaseColor", color);

        _mrBall.SetPropertyBlock(_ballMaterialPropertyBlock);
    }
}

public enum BallColorType {
    Black,
    Red,
    Green,
    Blue,
    Cyan
}