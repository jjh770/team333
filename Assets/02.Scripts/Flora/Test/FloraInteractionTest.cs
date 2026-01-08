using UnityEngine;

public class FloraInteractionTest : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private FloraInventory _inventory;
    [SerializeField] private FloraInteraction _interaction;
    [SerializeField] private FloraSpeedGaugeController _gaugeController;

    [Header("Test Settings")]
    [SerializeField] private int _addWoodAmount = 1;

    private void OnEnable()
    {
        if (_inventory != null)
        {
            _inventory.OnWoodChanged += OnWoodChanged;
        }

        if (_gaugeController != null)
        {
            _gaugeController.GaugeChanged += OnGaugeChanged;
        }
    }

    private void OnDisable()
    {
        if (_inventory != null)
        {
            _inventory.OnWoodChanged -= OnWoodChanged;
        }

        if (_gaugeController != null)
        {
            _gaugeController.GaugeChanged -= OnGaugeChanged;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddWood();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseWood();
        }
    }

    private void AddWood()
    {
        _inventory.AddWood(_addWoodAmount);
        Debug.Log($"[Test] 나무 추가: +{_addWoodAmount}");
    }

    private void UseWood()
    {
        if (_gaugeController.IsFull)
        {
            Debug.Log("[Test] 나무 사용 실패 - 게이지가 이미 꽉 참");
            return;
        }

        bool success = _interaction.TryFeedWood();
        Debug.Log(success ? "[Test] 나무 사용 성공! 게이지 증가" : "[Test] 나무 사용 실패 - 나무 부족");
    }

    private void OnWoodChanged(float woodCount)
    {
        Debug.Log($"[Test] 현재 나무 개수: {woodCount}");
    }

    private void OnGaugeChanged(float current, float max)
    {
        Debug.Log($"[Test] 게이지: {current:F2} / {max:F2}");
    }
}