using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum WaveObjectType
{
    Star = 5000,
    Coin = 15000,
    Apple = 1000,
    Bomb = -20000,
    X = 5,
    Wall = 0
}
public class WaveObject : MonoBehaviour
{
    [SerializeField] private WaveObjectType waveObjectType;
    [SerializeField] private float duration;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MoveTo(Vector3 endPoint)
    {
        transform.DOMove(endPoint, duration)
             .SetEase(Ease.Linear)
             .OnComplete(() => Destroy(gameObject));
    }

    public void ScaleTo(Vector3 endScale)
    {
        transform.DOScale(endScale, duration)
                     .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.CompareTag("Player"))
        {
            GameManager gameManager = collision.gameObject.GetComponent<GameManager>();

            if (waveObjectType == WaveObjectType.Wall)
            {
                gameManager.GameOver();
            }
            else
            {
                if(waveObjectType == WaveObjectType.X)
                    gameManager.MultiplyScore((int)waveObjectType);
                else
                    gameManager.AddScore((int)waveObjectType);
                Destroy(gameObject);
            }
        }
    }

    public void StopAnimation()
    {
        transform.DOKill();
    }

    public void ChangeWallColor(Color color)
    {
        if(waveObjectType == WaveObjectType.Wall)
        {
            spriteRenderer.color = color;
        }
    }
}
