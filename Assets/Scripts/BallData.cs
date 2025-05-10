using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball Data", menuName = "Ball Data")]
[Serializable]
public class BallData : ScriptableObject
{
    public int BallNumber;
    public string ballName;
    public Sprite ballSprite;
}
