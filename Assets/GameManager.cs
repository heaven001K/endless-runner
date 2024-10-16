using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Color platformColor;
    public bool colorEntierPlatform;
    
    public int coins;
    private void Awake()
    {
        instance = this;
    }

    public void RestartLevel() => SceneManager.LoadScene(0);

}
