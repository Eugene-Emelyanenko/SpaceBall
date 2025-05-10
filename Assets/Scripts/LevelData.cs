using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Level Data")]
[Serializable]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public string levelName;
    public Sprite previewSprite;
    public Sprite backgroundSprite;
    public Color obstacleColor;
    public AudioClip backgroundClip;

    public string GetFullName() => $"Level {levelNumber}. {levelName}";
}
