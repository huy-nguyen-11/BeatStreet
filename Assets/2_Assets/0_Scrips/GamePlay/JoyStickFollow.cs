using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, -1.5f, 0); // vị trí dưới chân

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position/* + offset*/;
        }
    }
}
