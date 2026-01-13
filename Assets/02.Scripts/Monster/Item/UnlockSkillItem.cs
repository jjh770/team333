using UnityEngine;

public class UnlockSkillItem : MonoBehaviour, IInteractable
{
    public InteractionType Type => InteractionType.Use;
    public Transform Transform
    {
        get
        {
            if (this == null) return null;
            return transform;
        }
    }
    public bool CanInteract => true;

    private PoolManager _poolManager;

    private void Start()
    {
        _poolManager = FindFirstObjectByType<PoolManager>();
    }

    public void Interact(GameObject interactor)
    {
        var playerSkill = interactor.GetComponent<PlayerSkillController>();
        if (playerSkill != null && playerSkill.SkillLevel < playerSkill.MaxSkillLevel)
        {
            playerSkill.UpgradeSkill();
            Destroy(gameObject);
        }
    }
}
