using UnityEngine;

public readonly struct Damage
{
    public float Value { get; }
    public GameObject Attacker { get; }
    
    public Damage(float value, GameObject attacker)
    {
        Value = value;
        Attacker = attacker;
    }
}
