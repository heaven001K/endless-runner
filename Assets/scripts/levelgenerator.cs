using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelpart;
    [SerializeField] private Vector3 nextPartPosition;
    [SerializeField] private float distancetoSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform player;

    void Update()
    {
        DeletePlatform();
        GeneratePlatform();
    }


    private void GeneratePlatform()
    {
        while (Vector2.Distance(player.transform.position, nextPartPosition) < distancetoSpawn)
        {
           
            Transform part = levelpart[Random.Range(0, levelpart.Length)];
            Vector2 newposition = new Vector2(nextPartPosition.x - part.Find("StartPoint").position.x, 0);
            Transform new_part = Instantiate(part, newposition, transform.rotation, transform);
            nextPartPosition = new_part.Find("EndPoint").position;

        } 
    }

    private void DeletePlatform()
    {
        if (transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);
            if (Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }
}