using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEfectHit : MonoBehaviour, IpooledObject
{
    public float timeToDisappear = 1f;

    public void OnObjectSpawn()
    {
       this.gameObject.SetActive(true);
        Invoke("OnRecycle", timeToDisappear);   
    }

    void OnRecycle()
    {
        this.gameObject.SetActive(false);
    }
}
