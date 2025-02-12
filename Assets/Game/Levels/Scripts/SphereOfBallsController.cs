using GameConfigs;
using UnityEngine;
using Zenject;

public sealed class SphereOfBallsController : MonoBehaviour, ICreateSphereOfBalls {
    private IControlTheLayerSphere[] _layersSphereStorage;
    private IControlTheSegmentLayer[] _segmentLayersStorage;

    #region DI
        private DiContainer _container;
        private SphereOfBallsConfigs _sphereOfBallsConfigs;
        private BallThrowingConfigs _ballThrowingConfigs;
    #endregion

    [Inject]
    private void Construct (
        DiContainer container, 
        SphereOfBallsConfigs sphereOfBallsConfigs, 
        BallThrowingConfigs ballThrowingConfigs
        ) {
        // Set DI
        _container = container;
        _sphereOfBallsConfigs = sphereOfBallsConfigs;
        _ballThrowingConfigs = ballThrowingConfigs;

        transform.position = _sphereOfBallsConfigs.SpherePosition;

        _layersSphereStorage = new IControlTheLayerSphere[_sphereOfBallsConfigs.MaxNumberOfLayersSphere];
        CreateObjControllers(_sphereOfBallsConfigs.LayerSpherePrefab, _layersSphereStorage, transform);

        _segmentLayersStorage = new IControlTheSegmentLayer[_layersSphereStorage.Length * _sphereOfBallsConfigs.MaxNumberOfSegmentsOnOneLayer];
        CreateObjControllers(_sphereOfBallsConfigs.SegmentLayerPrefab, _segmentLayersStorage, transform);
    }

    private void CreateObjControllers<I>(GameObject prefabObj, I[] storageObj, Transform parentObj) where I : class { // Abstract Factory method
        for (int i = 0; i < storageObj.Length; i++) {
            GameObject obj = _container.InstantiatePrefab(prefabObj, parentObj.position, Quaternion.identity, parentObj);

            Component[] components = obj.GetComponents<Component>();
            foreach (Component comp in components) {
                if (comp is I interfaceComponent) {
                    storageObj[i] = interfaceComponent;
                    break;
                }
            }
        }
    }

    public void CreateSphere() {
        int layerNumber = Random.Range(_sphereOfBallsConfigs.MinNumberOfLayersSphere, _sphereOfBallsConfigs.MaxNumberOfLayersSphere);

        float radiusSphere = _sphereOfBallsConfigs.StartRadiusSphere;

        for (int i = 0; i < layerNumber; i++) {
            int ballsNumber = Mathf.FloorToInt(Mathf.PI * Mathf.Pow(radiusSphere / (_ballThrowingConfigs.CurrentBallSize / 2), 2));

            IControlTheSegmentLayer[] sigments = new IControlTheSegmentLayer[Random.Range(_sphereOfBallsConfigs.MinNumberOfSegmentsOnOneLayer, _sphereOfBallsConfigs.MaxNumberOfSegmentsOnOneLayer)];

            for (int j = 0; j < sigments.Length; j++) {
                foreach (var segment in _segmentLayersStorage) {
                    if (!segment.IsSegmentLayerActive()) {
                        segment.SetSegmentEnable();

                        sigments[j] = segment;

                        break;
                    }
                }
            }
        
            _layersSphereStorage[i].PlaceBallsOnLayerSphere(ballsNumber, radiusSphere, sigments);

            radiusSphere += _sphereOfBallsConfigs.LayerDistanceSphere;
        }
    }

    public void SpawnLayersSphere() {
        foreach (var layerSphere in _layersSphereStorage) {
            if (layerSphere.IsLayerSphereActive()) layerSphere.LayerSphereEnable();
        }
    }

    public void DestroySphere() {
        foreach (var layerSphere in _layersSphereStorage) {
            if (layerSphere.IsLayerSphereActive()) layerSphere.DestroyLayerSphere();
        }
    }
}

public interface ICreateSphereOfBalls {
    public void CreateSphere();
    public void SpawnLayersSphere();
    public void DestroySphere();
}