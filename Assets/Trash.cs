using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public static Trash Instance;
    private WaitForSeconds _wait = new WaitForSeconds(5);
    private void Awake()
    {
        Instance = this;
    }
    
    public void TrashObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        StartCoroutine(SafelyDestroy(obj));
    }
    
    IEnumerator SafelyDestroy(GameObject obj)
    {
        yield return _wait;
        Destroy(obj);
    }
}
