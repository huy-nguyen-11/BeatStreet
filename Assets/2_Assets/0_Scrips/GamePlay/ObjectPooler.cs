using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Transform bulletTF , effectTF, textTF, coinsTF /* , boomerangTF , sawTF , shotgunTF , bulletTurretTF, coinTF , plasmaTF , textTF , hommingTF*/;

    // Start is called before the first frame update
    void Start()
    {
        //set size
        //SetSize();
        InstantiateObject();
    }

    //public void SetSize()
    //{
    //    int indexEquipMissile = DataManager.GetStateEquip("missile");
    //    //cannon
    //    if (indexEquipMissile == 0)
    //    {
    //        int featureLevel = DataManager.GetFeatureLevel("CANNON");
    //        // Define the valid index ranges for each feature level.
    //        int[][] validRanges = new int[][]
    //        {
    //        new int[] { 0 },              // Level 1
    //        new int[] { 0, 1, 2 },        // Level 2
    //        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, // Level 3
    //        new int[] { 3, 4, 5, 6, 7, 8, 12, 13 },  // Level 4
    //        new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }  // Level 5
    //        };

    //        for (int i = 0; i < 15; i++)
    //        {
    //            pools[i].size = 0;
    //        }

    //        if (featureLevel >= 1 && featureLevel <= 5)
    //        {
    //            foreach (int index in validRanges[featureLevel - 1])
    //            {
    //                pools[index].size = 15;
    //            }
    //        }
    //    }
    //    else if (indexEquipMissile == 1)
    //    {
    //        int featureLevel = DataManager.GetFeatureLevel("BOOMERANG");
    //        if(featureLevel < 3)
    //        {
    //            pools[19].size = 15;
    //        }else if( featureLevel ==3)
    //        {
    //            pools[19].size = 30;
    //        }else if(featureLevel >3)
    //        {
    //            pools[19].size = 45;
    //        }
    //    }
    //    else if(indexEquipMissile == 2)
    //    {

    //    }
    //    else if(indexEquipMissile == 3)
    //    {

    //    }
       

    //}

    public void InstantiateObject()
    {
        // instantiate object
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                //set parent
                if (obj.tag == "Bullet")
                {
                    obj.transform.parent = bulletTF;
                }
                else if (obj.tag == "Effect")
                {
                    obj.transform.parent = effectTF;
                }
                else if (obj.tag == "Text")
                {
                    obj.transform.parent = textTF;
                }
                else if (obj.tag == "Coin")
                {
                    obj.transform.parent = coinsTF;
                }
                else
                {
                    obj.transform.parent = transform;
                }
                //else if(obj.tag == "shotgun")
                //{
                //    obj.transform.parent = shotgunTF;
                //}
                //else if (obj.tag == "boomerang")
                //{
                //    obj.transform.parent = boomerangTF;
                //}
                //else if (obj.tag == "saw")
                //{
                //    obj.transform.parent = sawTF;
                //}
                //else if (obj.tag == "bullet")
                //{
                //    obj.transform.parent = bulletTurretTF;
                //}
                //else if (obj.tag == "coin")
                //{
                //    obj.transform.parent = coinTF;
                //}
                //else if (obj.tag == "plasma")
                //{
                //    obj.transform.parent = plasmaTF;
                //}
                //else if (obj.tag == "text")
                //{
                //    obj.transform.parent = textTF;
                //}
                //else if (obj.tag == "missile homming")
                //{
                //    obj.transform.parent = hommingTF;
                //}
                //else
                //{
                //    obj.transform.parent = transform;
                //}

                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("pool wiht tag" + tag + " doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // repawn object.
        IpooledObject pooledObject = objectToSpawn.GetComponent<IpooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj, float delay)
    {
        StartCoroutine(ReturnAfterDelay(tag, obj, delay));
    }

    private IEnumerator ReturnAfterDelay(string tag, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
