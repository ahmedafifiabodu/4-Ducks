using DG.Tweening;
using UnityEngine;

public class CustomInteractionAnimation : MonoBehaviour
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

    internal void StartTurretShootAnimation(Transform turretTransform)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        Vector3 originalPosition = turretTransform.position;

        // Create a sequence
        sequence = DOTween.Sequence();

        // push back distance
        sequence.Append(turretTransform.DOMove(originalPosition - new Vector3(0, 0, 0.5f), 0.1f).SetEase(Ease.OutQuad));

        // shake strength
        sequence.Append(turretTransform.DOShakePosition(0.2f, new Vector3(1f, 1f, 1f), 10, 90, false, true).SetEase(Ease.OutQuad));

        // Add a move back to original position tween to the sequence
        sequence.Append(turretTransform.DOMove(originalPosition, 0.1f).SetEase(Ease.OutQuad));

        // Start the sequence
        sequence.Play();
    }
}