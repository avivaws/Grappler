using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform camera1;
    // Start is called before the first frame update
    void Start()
    {
        camera1.position = new Vector3(player.position.x, player.position.y+5, -10);
        
    }

    // Update is called once per frame
    void Update()
    {
        camera1.position = new Vector3(player.position.x, player.position.y+5, -10);
    }
}
