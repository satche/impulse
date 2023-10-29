using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    // Movement sensibility
    // 0 = nullify all received value
    // 1 = don't change the received value
    [Range(0f, 1f)]
    public float movementSensibility = 0.5f;
}
