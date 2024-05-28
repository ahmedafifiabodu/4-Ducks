using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Logging.Log(other.name);
            gameObject.SetActive(false);
        
    }
}
