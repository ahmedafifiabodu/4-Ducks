using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            InstantiateTurret();

            gameObject.SetActive(false);
        }
    }

    private void InstantiateTurret()
    {
        GameObject Turret = ObjectPool.SharedInstance.GetPooledObject(2);
        if (Turret != null)
        {
            Turret.SetActive(true);
            Turret.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        }
    }

}
