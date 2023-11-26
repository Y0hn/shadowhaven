using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscilateBehavior : MonoBehaviour
{
    public bool oscX = false;
    public bool oscY = false;
    public float force = 0;

    bool moveRevX = false;
    bool moveRevY = false;

    bool isMoving = false;

    // Timer  
    float timerMove = 0;
    public float timerTick = 1;


    void Update()
    {
        if (Time.time > timerMove && transform.gameObject.activeSelf)
        {
            isMoving = true;
            float rev;

            if (oscX)
            {
                if (moveRevX)
                    rev = -1;
                else
                    rev = 1;

                transform.localPosition = new Vector2(transform.position.x + force * rev, transform.position.y);
                moveRevX = !moveRevX;
            }
            if (oscY)
            {
                if (moveRevY)
                    rev = -1;
                else
                    rev = 1;

                transform.localPosition = new Vector2(transform.position.x, transform.position.y + force * rev);
                moveRevY = !moveRevY;
            }

            timerMove = Time.time + timerTick;
        }
        else if (isMoving)
        {
            transform.localPosition = Vector3.zero;
        }
    }
}
