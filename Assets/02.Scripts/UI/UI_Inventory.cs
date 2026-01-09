using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _woodCount;
    [SerializeField] private FloraInventory _floraInventory;
   
    private void OnEnable()
    {
        _floraInventory.OnWoodChanged += UpdateUI;
    }

    private void OnDisable()
    {
        _floraInventory.OnWoodChanged -= UpdateUI;
    }
    
    private void UpdateUI(float value)
    {
        _woodCount.text = ((int)value).ToString();
    }
}
