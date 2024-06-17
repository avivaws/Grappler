using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BackGround_Controller : MonoBehaviour
{
    public Transform[] backGrounds;
    public Transform player;
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<backGrounds.Length;i++)
        {
            float xDiff = player.position.x - backGrounds[i].position.x;
            float yDiff = player.position.y - backGrounds[i].position.y;
            if (60 < Math.Abs(xDiff))
            {
                moveX(i, xDiff>0?1:-1);
            }
            if (60 < Math.Abs(yDiff))
            {
                moveY(i, yDiff > 0 ? 1 : -1);
            }
        }

    }

    private void moveX(int i,int sign)
    {
        backGrounds[i].position = new Vector3(backGrounds[i].position.x + (sign * 60 * 2), backGrounds[i].position.y, backGrounds[i].position.z);
    }

    private void moveY(int i, int sign)
    {
        backGrounds[i].position = new Vector3(backGrounds[i].position.x, backGrounds[i].position.y + (sign * 60 * 2) , backGrounds[i].position.z);
    }
}
