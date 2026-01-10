using UnityEngine;

public class FloraSkillChanger : ItemBase
{
    [Header("Skill Settings")]
    [SerializeField] private FloraSkillBase _skillPrefab;

    private const string FloraTag = "Flora";

    private void OnCollisionEnter(Collision other)
    {
        if (_isHeld) return;

        if (!other.gameObject.CompareTag(FloraTag)) return;
        if (!other.gameObject.TryGetComponent<FloraInteraction>(out var interaction)) return;

        interaction.SetSkill(_skillPrefab);

        Destroy(gameObject);
    }
}