using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloraSkillChanger : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private FloraSkillBase _skillPrefab;

    private const string FloraTag = "Flora";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FloraTag)) return;
        if (!other.TryGetComponent<FloraSkillController>(out var skillController)) return;

        skillController.SetSkill(_skillPrefab);

        Destroy(gameObject);
    }
}