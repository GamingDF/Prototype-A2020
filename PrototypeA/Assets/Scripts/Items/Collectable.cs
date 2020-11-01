using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string itemType;
    GameObject originalPrefab;

    public void SetPrefab(GameObject prefab){
        originalPrefab = prefab;
    }

    public GameObject GetPrefab(){
        return originalPrefab;
    }
}
