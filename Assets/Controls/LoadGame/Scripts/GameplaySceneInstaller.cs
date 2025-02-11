using UnityEngine;
using Zenject;
using Managers;
using GameConfigs;

public sealed class GameplaySceneInstaller : MonoInstaller { // Dependency injection
    [Header("Configurations")]
    [SerializeField] private BallThrowingConfigs _ballThrowingConfigs;
    [SerializeField] private VisualEffectsConfigs _visualEffectsConfigs;
    [Header("DI prefabs")]
    [SerializeField] private GameObject _cameraControllerPrefab;

    public override void InstallBindings() {
        Application.targetFrameRate = 120;

        ConfigsBind();

        ManagersInit();

        OtherDependencesInit();
    }

    private void ConfigsBind() {
        Container.Bind<BallThrowingConfigs>().FromInstance(_ballThrowingConfigs).AsSingle().NonLazy();
        Container.Bind<VisualEffectsConfigs>().FromInstance(_visualEffectsConfigs).AsSingle().NonLazy();
    }

    private void ManagersInit() {
        Container.BindInterfacesTo<InputManager>().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerManager>().AsSingle().NonLazy();
    }

    private void OtherDependencesInit() {
        Container.BindInterfacesTo<ThrowingBallsController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BallsPool>().AsSingle().NonLazy();
        
        Container.BindInterfacesTo<CameraController>().FromComponentInNewPrefab(_cameraControllerPrefab).AsSingle().NonLazy();
    }
}