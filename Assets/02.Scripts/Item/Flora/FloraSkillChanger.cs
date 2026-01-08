using UnityEngine;

public class FloraSkillChanger : ItemBase
{
    [Header("Skill Settings")]
    [SerializeField] private FloraSkillBase _skillPrefab;

    private const string FloraTag = "Flora";

    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag(FloraTag)) return;
        if (!other.collider.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.SetSkill(_skillPrefab);

        Destroy(gameObject);
    }
}