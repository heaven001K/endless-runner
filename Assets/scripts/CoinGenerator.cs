using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private int amountOfCoins;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int maxCoins;
    [SerializeField] private int minCoins;

    [SerializeField] private SpriteRenderer[] coinImage;
    void Start()    
    {
        for (int i = 0; i < coinImage.Length; i++)
        {
            coinImage[i].sprite = null;
        }
        int additionaOffset = amountOfCoins / 2;
        amountOfCoins = Random.Range(minCoins, maxCoins);
        for (int i = 0; i < amountOfCoins; i++)
        {
            
            Vector3 offset = new Vector3(i - additionaOffset, 0, 0);
            Instantiate(coinPrefab,transform.position + offset,Quaternion.identity,transform);
            
        }
    }

}
