using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private void OnEnable() => StartCoroutine(DeactivateAfterDelay());

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        ObjectPool.SharedInstance.ReturnToPool(2, gameObject);
    }
}