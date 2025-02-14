using System;
using System.Collections;
using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class BallMaterialControl : MonoBehaviour {
    private MeshRenderer _mrBall;

    public event Action BallDestroyed;

    private MaterialPropertyBlock _ballMaterialPropertyBlock;

    #region DI
        VisualEffectsConfigs _visualEffectsConfigs;
        BallThrowingConfigs _ballThrowingConfigs;
    #endregion

    [Inject]
    private void Construct(VisualEffectsConfigs visualEffectsConfigs, BallThrowingConfigs ballThrowingConfigs) {
        // Set DI
        _visualEffectsConfigs = visualEffectsConfigs;
        _ballThrowingConfigs = ballThrowingConfigs;
        
        // Set components
        _ballMaterialPropertyBlock = new MaterialPropertyBlock();

        _mrBall = GetComponent<MeshRenderer>();
        _mrBall.enabled = false;
        _mrBall.material = _visualEffectsConfigs.BallMaterial;

        _ballMaterialPropertyBlock.SetFloat("_NoiseScale", _visualEffectsConfigs.NoiseScaleDissolve);
        _ballMaterialPropertyBlock.SetFloat("_NoiseStrength", _visualEffectsConfigs.NoiseStrengthDissolve);
        _ballMaterialPropertyBlock.SetFloat("_EdgeWidth", _visualEffectsConfigs.EdgeWidthDissolve);
        _ballMaterialPropertyBlock.SetColor("_EdgeColor", _visualEffectsConfigs.EdgeColorDissolve);
        
        _mrBall.SetPropertyBlock(_ballMaterialPropertyBlock);
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

    public void BallSpawnEffectEnable() {
        _mrBall.enabled = true;

        _ballMaterialPropertyBlock.SetFloat("_CutoffHeight", _ballThrowingConfigs.CutoffHeightStart);
    
        StartCoroutine(BallSpawnStarted());
    }

    private IEnumerator BallSpawnStarted() {
        float effectTime = _ballThrowingConfigs.BallSpawnTime;
        float inverseEffectTime = 1f / effectTime;

        float startCutoffHeight = _ballThrowingConfigs.CutoffHeightStart;
        float finishCutoffHeight = _ballThrowingConfigs.CutoffHeightFinish;

        float elapsedTime = 0f;

        while (elapsedTime < effectTime) {
            float currentValue = Mathf.Lerp(startCutoffHeight, finishCutoffHeight, elapsedTime * inverseEffectTime);

            _ballMaterialPropertyBlock.SetFloat("_CutoffHeight", currentValue);

            _mrBall.SetPropertyBlock(_ballMaterialPropertyBlock);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _ballMaterialPropertyBlock.SetFloat("_CutoffHeight", finishCutoffHeight);

        _mrBall.SetPropertyBlock(_ballMaterialPropertyBlock);
    }

    public void BallDestroyEffectEnable() {
        _ballMaterialPropertyBlock.SetFloat("_CutoffHeight", _ballThrowingConfigs.CutoffHeightFinish);

        StartCoroutine(BallDestroyStarted());
    }

    private IEnumerator BallDestroyStarted() {
        float effectTime = _ballThrowingConfigs.BallDestroyTime;
        float inverseEffectTime = 1f / effectTime;

        float startCutoffHeight = _ballThrowingConfigs.CutoffHeightStart;
        float finishCutoffHeight = _ballThrowingConfigs.CutoffHeightFinish;

        float elapsedTime = 0f;

        while (elapsedTime < effectTime) {
            float currentValue = Mathf.Lerp(finishCutoffHeight, startCutoffHeight, elapsedTime * inverseEffectTime);

            _ballMaterialPropertyBlock.SetFloat("_CutoffHeight", currentValue);

            _mrBall.SetPropertyBlock(_ballMaterialPropertyBlock);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _ballMaterialPropertyBlock.SetFloat("_CutoffHeight", finishCutoffHeight);

        _mrBall.SetPropertyBlock(_ballMaterialPropertyBlock);

        _mrBall.enabled = false;

        BallDestroyed?.Invoke();
    }
}

public enum BallColorType {
    Black,
    Red,
    Green,
    Blue,
    Cyan
}