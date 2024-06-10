using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) => gameObject.SetActive(false);
}