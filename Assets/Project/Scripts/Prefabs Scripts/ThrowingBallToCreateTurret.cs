using UnityEngine;

public class ThrowingBallToCreateTurret : MonoBehaviour
{
    private ObjectPool objectPool;

    private void Start() => objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstant.Tag.Ground))
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
        GameObject Turret = objectPool.GetPooledObject(2);

        if (Turret != null)
        {
            Turret.SetActive(true);
            Turret.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}