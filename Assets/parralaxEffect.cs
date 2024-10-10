using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallax;
    private float length;
    private float xPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        xPosition = transform.position.x; // Початкова позиція об'єкта
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Ширина спрайта
    }

    void Update()
    {
        float DistanceMoved = cam.transform.position.x * (1 - parallax);
        float DistanceToMove = cam.transform.position.x * parallax;

        transform.position = new Vector3(xPosition + DistanceToMove, transform.position.y);

        if (DistanceMoved > xPosition + length)
        {
            xPosition += length;
        }
    } 

}