using UnityEngine;

public class FloraMoveLocker : MonoBehaviour
{
    private const string FloraTag = "Flora";
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FloraTag)) return;

        if (other.TryGetComponent<FloraInteraction>(out var floraInteraction))
        {
            floraInteraction.SetMoveLock(true);
        }
    }
}
