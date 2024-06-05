using UnityEngine;

public class TurretBullet : Interactable
{
    //private void OnTriggerEnter(Collider other) => gameObject.SetActive(false);

    protected override void Interact(ObjectType _objectType)
    {
        base.Interact(_objectType);
        gameObject.SetActive(false);
    }

}