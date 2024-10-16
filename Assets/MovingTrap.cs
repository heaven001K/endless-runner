using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : trap
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform[] movePoints;
    private int i;

    private void Start() => transform.position = movePoints[0].position;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoints[i].position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoints[i].position) < .25f)
        {
            i++;
            if (i >= movePoints.Length)
            {
                i = 0;
            }
        }

        if (transform.position.x > movePoints[i].position.x)
        {
            transform.Rotate(new Vector3(0,0,rotationSpeed * Time.deltaTime));
            
        }
        else
        {
            transform.Rotate(new Vector3(0,0,-rotationSpeed * Time.deltaTime));
        }
    }



    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
