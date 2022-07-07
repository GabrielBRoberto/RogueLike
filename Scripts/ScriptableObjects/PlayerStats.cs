using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Stats/Player", order = 1)]
public class PlayerStats : GeneralStats
{
    [Header("LifeSteal")]
    public float maxLifeSteal;
    public float actualLifeSteal;

    [Header("Luck")]
    public int maxLuck;
    public int actualLuck;

    [Header("Aim Sensitivity")]
    public float sensitivity;

    [Header("Jump")]
    public float jumpForce;

    [Header("Money")]
    public int money;
}
