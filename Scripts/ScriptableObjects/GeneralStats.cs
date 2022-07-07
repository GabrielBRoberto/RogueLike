using UnityEngine;

public class GeneralStats : ScriptableObject
{
    [Header("Health")]
    public float maxHealth;
    public float actualHealth;
    public float regenHealthRate;

    [Header("Shield")]
    public float maxShield;
    public float actualShield;
    public float maxRegenShieldRate;
    public float actualRegenShieldRate;

    [Header("Resistence")]
    public float maxResistence;
    public float actualResistence;

    [Header("Speed")]
    public float speed;

    [Header("Damage")]
    public float maxDamage;
    public float actualDamage;
}
