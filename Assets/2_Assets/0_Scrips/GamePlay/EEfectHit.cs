using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEfectHit : MonoBehaviour, IpooledObject
{
    public void OnObjectSpawn()
    {
       this.gameObject.SetActive(true);
        Invoke("OnRecycle", 0.15f);
    }

    void OnRecycle()
    {
        this.gameObject.SetActive(false);
    }
}
