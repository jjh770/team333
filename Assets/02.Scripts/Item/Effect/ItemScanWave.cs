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
    private float _timer;
    private bool _isScanning;
    private float _scanProgress;

    private static readonly int ScanColor = Shader.PropertyToID("_ScanColor");
    private static readonly int ScanPosition = Shader.PropertyToID("_ScanPosition");
    private static readonly int ScanWidth = Shader.PropertyToID("_ScanWidth");
    private static readonly int ScanSoftness = Shader.PropertyToID("_ScanSoftness");
    private static readonly int Intensity = Shader.PropertyToID("_Intensity");

    private static Shader _scanShader;

    void Awake()
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

    void SetupScanMaterial()
    {
        if (_renderer == null) return;

        // 쉐이더 캐싱
        if (_scanShader == null)
        {
            _scanShader = Shader.Find("Custom/ItemScanWave");
        }

        if (_scanShader == null)
        {
            Debug.LogError("ItemScanWave: Custom/ItemScanWave shader not found!");
            return;
        }

        // 스캔 머티리얼 생성
        _scanMaterial = new Material(_scanShader);
        _scanMaterial.SetColor(ScanColor, _scanColor);
        _scanMaterial.SetFloat(ScanWidth, _scanWidth);
        _scanMaterial.SetFloat(ScanSoftness, _scanSoftness);
        _scanMaterial.SetFloat(Intensity, 0f); // 시작은 안보이게

        // 기존 머티리얼에 스캔 머티리얼 추가
        AddOverlayMaterial();
    }

    void AddOverlayMaterial()
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

    void Update()
    {
        if (_scanMaterial == null) return;

        // 들고 있으면 스캔 중지
        if (_item != null && _item.IsHeld)
        {
            if (_isScanning)
            {
                StopScan();
            }
            _timer = 0f;
            return;
        }

        _timer += Time.deltaTime;

        if (!_isScanning && _timer >= _waveInterval)
        {
            StartScan();
        }

        if (_isScanning)
        {
            _scanProgress += Time.deltaTime / _waveDuration;

            if (_scanProgress >= 1f)
            {
                StopScan();
            }
            else
            {
                UpdateScan();
            }
        }
    }

    void StartScan()
    {
        _isScanning = true;
        _scanProgress = 0f;
        _timer = 0f;
        _scanMaterial.SetFloat(Intensity, _intensity);
    }

    void UpdateScan()
    {
        // 스캔 위치 계산 (왼쪽 위 → 오른쪽 아래 대각선)
        float currentPos = Mathf.Lerp(_scanStart, _scanEnd, _scanProgress);
        _scanMaterial.SetFloat(ScanPosition, currentPos);

        // 시작과 끝에서 페이드 인/아웃
        float fadeMultiplier = 1f;
        if (_scanProgress < 0.1f)
        {
            fadeMultiplier = _scanProgress / 0.1f;
        }
        else if (_scanProgress > 0.9f)
        {
            fadeMultiplier = (1f - _scanProgress) / 0.1f;
        }

        _scanMaterial.SetFloat(Intensity, _intensity * fadeMultiplier);
    }

    void StopScan()
    {
        _isScanning = false;
        _scanProgress = 0f;
        _scanMaterial.SetFloat(Intensity, 0f);
    }

    void OnDestroy()
    {
        if (_scanMaterial != null)
        {
            Destroy(_scanMaterial);
        }
    }

    void OnDisable()
    {
        if (_scanMaterial != null)
        {
            _scanMaterial.SetFloat(Intensity, 0f);
        }
    }

    // 에디터에서 값 변경 시 실시간 반영
    void OnValidate()
    {
        if (_scanMaterial != null)
        {
            _scanMaterial.SetColor(ScanColor, _scanColor);
            _scanMaterial.SetFloat(ScanWidth, _scanWidth);
            _scanMaterial.SetFloat(ScanSoftness, _scanSoftness);
        }
    }

    // 외부에서 스캔 범위 자동 계산 요청 시
    public void AutoCalculateBounds()
    {
        if (_renderer != null)
        {
            var bounds = _renderer.localBounds;
            // 3D 대각선 범위: X - Y + Z
            _scanStart = bounds.min.x - bounds.max.y + bounds.min.z;
            _scanEnd = bounds.max.x - bounds.min.y + bounds.max.z;
        }
    }
}
