using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Header("Mesh Related")]
    [SerializeField] private float _meshRefreshRate = 0.1f; // 잔상 생성 간격 (초)
    [SerializeField] private float _meshLifetime = 3f; // 잔상 유지 시간 (초)
    [SerializeField] private Transform _positionToSpawn; // 잔상 생성 위치 기준점

    [Header("Shader Related")]
    [SerializeField] private Material _defaultMat; // 잔상에 적용할 머티리얼
    [SerializeField] private string _shaderVarRef; // 페이드 효과에 사용할 셰이더 프로퍼티 이름
    [SerializeField] private float _shaderVarRate = 0.1f; // 셰이더 값(알파값) 감소량 (프레임당)
    [SerializeField] private float _shaderVarRefreshRate = 0.05f; // 셰이더 값 업데이트 간격 (초)

    [Header("Pool Settings")]
    [SerializeField] private int _initialPoolSize = 30; 

    private PlayerDash _playerDash;
    private bool _isTrailActive;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;

    private Queue<TrailObject> _pool = new();
    private Transform _poolParent;

    private int _shaderPropertyId;
    private float _initialShaderValue;
    private WaitForSeconds _meshRefreshWait;
    private WaitForSeconds _shaderRefreshWait;

    private class TrailObject
    {
        public GameObject GameObject;
        public MeshRenderer MeshRenderer;
        public MeshFilter MeshFilter;
        public Mesh Mesh;
        public MaterialPropertyBlock PropertyBlock;
    }

    private void Awake()
    {
        _playerDash = GetComponent<PlayerDash>();

        if (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Length == 0)
        {
            _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        _shaderPropertyId = Shader.PropertyToID(_shaderVarRef);
        _initialShaderValue = _defaultMat.GetFloat(_shaderPropertyId);
        _meshRefreshWait = new WaitForSeconds(_meshRefreshRate);
        _shaderRefreshWait = new WaitForSeconds(_shaderVarRefreshRate);

        InitializePool();
    }

    private void InitializePool()
    {
        _poolParent = new GameObject("TrailPool").transform;
        _poolParent.SetParent(transform);

        for (int i = 0; i < _initialPoolSize; i++)
        {
            _pool.Enqueue(CreateTrailObject());
        }
    }

    private TrailObject CreateTrailObject()
    {
        var trailObject = new TrailObject();

        trailObject.GameObject = new GameObject("Trail");
        trailObject.GameObject.transform.SetParent(_poolParent);
        trailObject.MeshRenderer = trailObject.GameObject.AddComponent<MeshRenderer>();
        trailObject.MeshFilter = trailObject.GameObject.AddComponent<MeshFilter>();
        trailObject.Mesh = new Mesh();
        trailObject.PropertyBlock = new MaterialPropertyBlock();

        trailObject.MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trailObject.MeshRenderer.sharedMaterial = _defaultMat;
        trailObject.MeshFilter.mesh = trailObject.Mesh;

        trailObject.GameObject.SetActive(false);

        return trailObject;
    }

    private TrailObject GetFromPool()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }

        return CreateTrailObject();
    }

    private void ReturnToPool(TrailObject trailObject)
    {
        trailObject.GameObject.SetActive(false);
        trailObject.GameObject.transform.SetParent(_poolParent);
        _pool.Enqueue(trailObject);
    }

    private void OnEnable()
    {
        _playerDash.OnDashStart += StartTrail;
    }

    private void OnDisable()
    {
        _playerDash.OnDashStart -= StartTrail;
    }

    private void StartTrail(float duration)
    {
        if (!_isTrailActive)
        {
            StartCoroutine(ActiveTrail(duration));
        }
    }

    private IEnumerator ActiveTrail(float timeActive)
    {
        _isTrailActive = true;

        while (timeActive > 0)
        {
            timeActive -= _meshRefreshRate;

            for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
            {
                TrailObject trail = GetFromPool();

                trail.GameObject.transform.SetParent(null);
                trail.GameObject.transform.SetPositionAndRotation(_positionToSpawn.position, _positionToSpawn.rotation);

                _skinnedMeshRenderers[i].BakeMesh(trail.Mesh);

                trail.PropertyBlock.SetFloat(_shaderPropertyId, _initialShaderValue);
                trail.MeshRenderer.SetPropertyBlock(trail.PropertyBlock);
                trail.GameObject.SetActive(true);

                StartCoroutine(AnimateAndReturn(trail));
            }

            yield return _meshRefreshWait;
        }

        _isTrailActive = false;
    }

    private IEnumerator AnimateAndReturn(TrailObject trail)
    {
        float valueToAnimate = _initialShaderValue;

        float elapsed = 0f;
        while (elapsed < _meshLifetime && valueToAnimate > 0)
        {
            valueToAnimate -= _shaderVarRate;
            trail.PropertyBlock.SetFloat(_shaderPropertyId, valueToAnimate);
            trail.MeshRenderer.SetPropertyBlock(trail.PropertyBlock);
            elapsed += _shaderVarRefreshRate;
            yield return _shaderRefreshWait;
        }

        ReturnToPool(trail);
    }
}
