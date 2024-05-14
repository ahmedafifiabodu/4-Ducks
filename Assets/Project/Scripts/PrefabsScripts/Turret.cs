using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        ObjectPool.SharedInstance.ReturnToPool(2,gameObject);
        //gameObject.SetActive(false);
    }
}
