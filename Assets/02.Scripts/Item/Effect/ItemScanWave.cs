using System.Collections;
using UnityEngine;

public class ItemScanWave : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float _waveInterval = 3f;
    [SerializeField] private float _waveDuration = 0.3f;
    [SerializeField] private Color _scanColor = Color.white;
    [SerializeField] private float _intensity = 0.8f;

    [Header("Scan Band Settings")]
    [SerializeField] private float _scanWidth = 0.1f;
    [SerializeField] private float _scanSoftness = 0.1f;

    [Header("Position Range (3D Diagonal)")]
    [SerializeField] private float _scanStart = -1.5f;
    [SerializeField] private float _scanEnd = 1.5f;

    private Renderer _renderer;
    private Material _scanMaterial;
    private ItemBase _item;
    private Coroutine _scanCoroutine;
    private bool _isActive;

    private static readonly int ScanColorId = Shader.PropertyToID("_ScanColor");
    private static readonly int ScanPositionId = Shader.PropertyToID("_ScanPosition");
    private static readonly int ScanWidthId = Shader.PropertyToID("_ScanWidth");
    private static readonly int ScanSoftnessId = Shader.PropertyToID("_ScanSoftness");
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");

    private static Shader _scanShader;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            _renderer = GetComponentInChildren<Renderer>();
        }

        _item = GetComponent<ItemBase>();
        if (_item == null)
        {
            _item = GetComponentInParent<ItemBase>();
        }

        SetupScanMaterial();
    }

    private void OnEnable()
    {
        if (_item != null)
        {
            _item.OnHeld += HandleHeld;
            _item.OnDropped += HandleDropped;
            _item.OnLockChanged += HandleLockChanged;

            // 초기 상태 확인
            if (!_item.IsHeld && !_item.IsLocked)
            {
                StartScanning();
            }
        }
        else
        {
            // ItemBase가 없으면 항상 스캔
            StartScanning();
        }
    }

    private void OnDisable()
    {
        if (_item != null)
        {
            _item.OnHeld -= HandleHeld;
            _item.OnDropped -= HandleDropped;
            _item.OnLockChanged -= HandleLockChanged;
        }

        StopScanning();

        if (_scanMaterial != null)
        {
            _scanMaterial.SetFloat(IntensityId, 0f);
        }
    }

    private void HandleHeld()
    {
        StopScanning();
    }

    private void HandleDropped()
    {
        if (_item == null || !_item.IsLocked)
        {
            StartScanning();
        }
    }

    private void HandleLockChanged(bool isLocked)
    {
        if (isLocked)
        {
            StopScanning();
        }
        else if (_item == null || !_item.IsHeld)
        {
            StartScanning();
        }
    }

    private void StartScanning()
    {
        if (_isActive || _scanMaterial == null) return;

        _isActive = true;
        _scanCoroutine = StartCoroutine(ScanRoutine());
    }

    private void StopScanning()
    {
        if (!_isActive) return;

        _isActive = false;

        if (_scanCoroutine != null)
        {
            StopCoroutine(_scanCoroutine);
            _scanCoroutine = null;
        }

        if (_scanMaterial != null)
        {
            _scanMaterial.SetFloat(IntensityId, 0f);
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (_isActive)
        {
            // 인터벌 대기
            yield return new WaitForSeconds(_waveInterval);

            if (!_isActive) yield break;

            // 스캔 애니메이션
            float progress = 0f;
            while (progress < 1f && _isActive)
            {
                progress += Time.deltaTime / _waveDuration;

                float currentPos = Mathf.Lerp(_scanStart, _scanEnd, progress);
                _scanMaterial.SetFloat(ScanPositionId, currentPos);

                // 페이드 인/아웃
                float fadeMultiplier = 1f;
                if (progress < 0.1f)
                {
                    fadeMultiplier = progress / 0.1f;
                }
                else if (progress > 0.9f)
                {
                    fadeMultiplier = (1f - progress) / 0.1f;
                }

                _scanMaterial.SetFloat(IntensityId, _intensity * fadeMultiplier);

                yield return null;
            }

            _scanMaterial.SetFloat(IntensityId, 0f);
        }
    }

    private void SetupScanMaterial()
    {
        if (_renderer == null) return;

        if (_scanShader == null)
        {
            _scanShader = Shader.Find("Custom/ItemScanWave");
        }

        if (_scanShader == null)
        {
            Debug.LogError("ItemScanWave: Custom/ItemScanWave shader not found!");
            return;
        }

        _scanMaterial = new Material(_scanShader);
        _scanMaterial.SetColor(ScanColorId, _scanColor);
        _scanMaterial.SetFloat(ScanWidthId, _scanWidth);
        _scanMaterial.SetFloat(ScanSoftnessId, _scanSoftness);
        _scanMaterial.SetFloat(IntensityId, 0f);

        AddOverlayMaterial();
    }

    private void AddOverlayMaterial()
    {
        if (_renderer == null || _scanMaterial == null) return;

        var currentMaterials = _renderer.sharedMaterials;
        var newMaterials = new Material[currentMaterials.Length + 1];

        for (int i = 0; i < currentMaterials.Length; i++)
        {
            newMaterials[i] = currentMaterials[i];
        }

        newMaterials[currentMaterials.Length] = _scanMaterial;
        _renderer.materials = newMaterials;
    }

    private void OnDestroy()
    {
        if (_scanMaterial != null)
        {
            Destroy(_scanMaterial);
        }
    }

    private void OnValidate()
    {
        if (_scanMaterial != null)
        {
            _scanMaterial.SetColor(ScanColorId, _scanColor);
            _scanMaterial.SetFloat(ScanWidthId, _scanWidth);
            _scanMaterial.SetFloat(ScanSoftnessId, _scanSoftness);
        }
    }

    public void AutoCalculateBounds()
    {
        if (_renderer != null)
        {
            var bounds = _renderer.localBounds;
            _scanStart = bounds.min.x - bounds.max.y + bounds.min.z;
            _scanEnd = bounds.max.x - bounds.min.y + bounds.max.z;
        }
    }
}
