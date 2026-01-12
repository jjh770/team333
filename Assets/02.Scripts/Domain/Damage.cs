using UnityEngine;

public readonly struct Damage
{
    public float Value { get; }
    public GameObject Attacker { get; }
    public bool IsNockBack { get; }

    public Damage(float value, GameObject attacker, bool isKnockback)
    {
        Value = value;
        Attacker = attacker;
        IsNockBack = isKnockback;
    }
}
