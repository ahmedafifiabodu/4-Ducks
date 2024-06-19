using DG.Tweening;
using UnityEngine;

public class CustomDoTweenAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 1f; // Duration of the animation
    private Sequence sequence; // Store the sequence

    // Start an interactable animation
    public void StartInteractableAnimation(GameObject targetObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Get the original scale of the object
        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a scaling up tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2).SetEase(Ease.OutQuad));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale, duration / 2).SetEase(Ease.InQuad));

        // Set the sequence to loop 2 times
        sequence.SetLoops(2, LoopType.Yoyo);

        // Start the sequence
        sequence.Play();
    }

    // Start an interactable animation and set the GameObject to false when the animation is done
    public void StartInteractableAnimationAndSetGameObjectToFalse(GameObject targetObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Get the original scale of the object
        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a scaling up tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2).SetEase(Ease.OutQuad));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale, duration / 2).SetEase(Ease.InQuad));

        // Set the sequence to loop 2 times
        sequence.SetLoops(2, LoopType.Yoyo);

        // Add a callback to deactivate the GameObject when the sequence is complete
        sequence.OnComplete(() => targetObject.SetActive(false));

        // Start the sequence
        sequence.Play();
    }

    public void StartCatInteractJumpingUpAnimation()
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Get the cat object
        GameObject targetObject = ServiceLocator.Instance.GetService<Cat>().gameObject;

        // Get the original position of the object
        Vector3 originalPosition = targetObject.transform.position;

        // Get the original scale of the object
        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a jump tween to the sequence
        sequence.Append(targetObject.transform.DOJump(originalPosition + new Vector3(0, 1, 0), 0.5f, 1, duration / 2).SetEase(Ease.OutQuad));

        // Add a scaling up tween to the sequence
        sequence.Join(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2).SetEase(Ease.OutQuad));

        // Add a rotation tween to the sequence
        sequence.Join(targetObject.transform.DORotate(new Vector3(0, 360, 0), duration / 2, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale, duration / 2).SetEase(Ease.InQuad));

        // Add a move back to original position tween to the sequence
        sequence.Append(targetObject.transform.DOMove(originalPosition, duration / 2).SetEase(Ease.InQuad));

        // Start the sequence
        sequence.Play();
    }

    internal void StartTurretShootAnimation(Transform turretTransform)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Get the original local rotation of the turret
        Quaternion originalLocalRotation = turretTransform.localRotation;

        // Create a sequence for the turret shoot animation
        sequence = DOTween.Sequence();

        // Increase the rotation angle for a more pronounced tilt upwards, using local rotation
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-10, 0, 0), duration / 6).SetEase(Ease.OutCubic));

        // Intensify the vibration by increasing the range and frequency, using local rotation
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-8, 0, 0), duration / 12).SetEase(Ease.InOutSine));
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-12, 0, 0), duration / 12).SetEase(Ease.InOutSine));
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-10, 0, 0), duration / 12).SetEase(Ease.InOutSine));
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-9, 0, 0), duration / 12).SetEase(Ease.InOutSine));
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-11, 0, 0), duration / 12).SetEase(Ease.InOutSine));

        // Return the turret to its original local rotation more quickly to simulate a snappy recoil effect
        sequence.Append(turretTransform.DOLocalRotateQuaternion(originalLocalRotation, duration / 6).SetEase(Ease.InCubic));

        // Start the sequence
        sequence.Play();
    }

    public void StartEnemyDeathAnimation(GameObject enemyObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Ensure the enemy object is not null
        if (enemyObject == null) return;

        // Get the original scale of the enemy object
        Vector3 originalScale = enemyObject.transform.localScale;

        // Create a new sequence for the animation
        sequence = DOTween.Sequence();

        // Add a scaling down tween to the sequence
        sequence.Append(enemyObject.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

        // Optionally, add a fading out effect if the enemy has a SpriteRenderer or MeshRenderer
        if (enemyObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            sequence.Join(spriteRenderer.DOFade(0, duration).SetEase(Ease.InQuad));
        else
        {
            if (enemyObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
                sequence.Join(meshRenderer.material.DOFade(0, duration).SetEase(Ease.InQuad));
        }

        // Add a callback to deactivate the enemy object when the animation is done
        sequence.OnComplete(() => enemyObject.SetActive(false));

        // Start the sequence
        sequence.Play();
    }

    public void StartDestructableObjectAnimation(GameObject targetObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // If the target object is null, set it to the current object
        if (targetObject == null)
            targetObject = gameObject;

        // Get the original scale of the object
        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a scaling up tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2).SetEase(Ease.OutQuad));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(Vector3.zero, duration / 2).SetEase(Ease.InQuad));

        // Add a callback to the sequence to deactivate the object when the animation is done
        sequence.OnComplete(() => targetObject.SetActive(false));

        // Start the sequence
        sequence.Play();
    }

    public void StartPlatformImpactAnimation(GameObject platformObject)
    {
        // Ensure the platform object is not null
        if (platformObject == null) return;

        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Get the original scale of the platform
        Vector3 originalScale = platformObject.transform.localScale;

        // Create a new sequence for the animation
        sequence = DOTween.Sequence();

        // Add a slight scaling up tween to the sequence to represent the impact
        sequence.Append(platformObject.transform.DOScale(originalScale * 1.1f, 0.1f).SetEase(Ease.OutQuad));

        // Add a scaling down tween to the sequence to return to the original size
        sequence.Append(platformObject.transform.DOScale(originalScale, 0.2f).SetEase(Ease.InQuad));

        // Optionally, you can set the sequence to auto-kill to clean up resources once the animation is complete
        sequence.SetAutoKill(true);

        // Start the sequence
        sequence.Play();
    }

    public void StartAppearAnimation(GameObject targetObject)
    {
        // Ensure the target object is not null
        if (targetObject == null) return;

        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        // Set the object to active
        targetObject.SetActive(true);

        // Get the original scale of the object
        Vector3 originalScale = targetObject.transform.localScale;

        // Temporarily set the object's scale to zero
        targetObject.transform.localScale = Vector3.zero;

        // Create a new sequence for the animation
        sequence = DOTween.Sequence();

        // Add a scaling up tween to the sequence to animate the object appearing
        sequence.Append(targetObject.transform.DOScale(originalScale, duration).SetEase(Ease.OutBack));

        // Optionally, you can add a fading in effect if the object has a SpriteRenderer or MeshRenderer
        if (targetObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            sequence.Join(spriteRenderer.DOFade(1, duration).SetEase(Ease.InQuad));
        else
        {
            if (targetObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
                sequence.Join(meshRenderer.material.DOFade(1, duration).SetEase(Ease.InQuad));
        }

        // Start the sequence
        sequence.Play();
    }
}