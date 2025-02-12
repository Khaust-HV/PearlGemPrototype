using UnityEngine;
using Zenject;
using Managers;
using GameConfigs;

public sealed class GameplaySceneInstaller : MonoInstaller { // Dependency injection
    [Header("Configurations")]
    [SerializeField] private BallThrowingConfigs _ballThrowingConfigs;
    [SerializeField] private VisualEffectsConfigs _visualEffectsConfigs;
    [SerializeField] private SphereOfBallsConfigs _sphereOfBallsConfigs;
    [Header("DI prefabs")]
    [SerializeField] private GameObject _cameraControllerPrefab;
    [SerializeField] private GameObject _sphereOfBallsControllerPrefab;

    public override void InstallBindings() {
        Application.targetFrameRate = 120;

        ConfigsBind();

        ManagersInit();

        OtherDependencesInit();
    }

    private void ConfigsBind() {
        Container.Bind<BallThrowingConfigs>().FromInstance(_ballThrowingConfigs).AsSingle().NonLazy();
        Container.Bind<VisualEffectsConfigs>().FromInstance(_visualEffectsConfigs).AsSingle().NonLazy();
        Container.Bind<SphereOfBallsConfigs>().FromInstance(_sphereOfBallsConfigs).AsSingle().NonLazy();
    }

    private void ManagersInit() {
        Container.BindInterfacesTo<InputManager>().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerManager>().AsSingle().NonLazy();
    }

    private void OtherDependencesInit() {
        Container.BindInterfacesTo<ThrowingBallsController>().AsSingle().NonLazy();
        Container.BindInterfacesTo<BallsPool>().AsSingle().NonLazy();
        
        Container.BindInterfacesTo<CameraController>().FromComponentInNewPrefab(_cameraControllerPrefab).AsSingle().NonLazy();
        Container.BindInterfacesTo<SphereOfBallsController>().FromComponentInNewPrefab(_sphereOfBallsControllerPrefab).AsSingle().NonLazy();
    }
}