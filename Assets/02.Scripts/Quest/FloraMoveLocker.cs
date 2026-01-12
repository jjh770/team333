using UnityEngine;

public class FloraMoveLocker : MonoBehaviour
{
    private FloraInteraction _floraInteraction; 
    
    private const string FloraTag = "Flora";
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FloraTag)) return;

        _floraInteraction = other.GetComponent<FloraInteraction>();
        _floraInteraction.SetMoveLock(true);
    }
}
