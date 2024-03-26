using System.Collections;
using Tools;
using UnityEngine;

namespace Components
{
    public class Trash : MonoSingleton<Trash>
    {
        private WaitForSeconds _wait = new WaitForSeconds(5);
    
        public void TrashObject(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            StartCoroutine(SafelyDestroy(obj));
            Debug.Break();
        }
    
        IEnumerator SafelyDestroy(GameObject obj)
        {
            yield return _wait;
            Destroy(obj);
        }

        public void TrashObject(GameObject obj, float time)
        {
            StartCoroutine(DelayedTrash(obj, time));
        }
        
        IEnumerator DelayedTrash(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            StartCoroutine(SafelyDestroy(obj));
        }
    }
}
