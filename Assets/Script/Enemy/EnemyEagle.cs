using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEagle: Enemy 
{
    public float speed;
    public float radius;

    private Transform playerTransform;
    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        if(playerTransform != null)
        {
            float distance = (transform.position - playerTransform.position).sqrMagnitude;
            if(distance < radius)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
            }
        }
    }
}
