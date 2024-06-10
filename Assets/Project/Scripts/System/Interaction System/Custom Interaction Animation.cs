using DG.Tweening;
using UnityEngine;

public class CustomInteractionAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 1f; // Duration of the animation
    private Sequence sequence; // Store the sequence

    // Method to start the animation
    internal void StartInteractableAnimation(GameObject targetObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a scaling up tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale, duration / 2));

        // Set the sequence to loop 2 times
        sequence.SetLoops(2, LoopType.Yoyo);

        // Start the sequence
        sequence.Play();
    }

    internal void StartCatInteractJumpingUpAnimation(GameObject targetObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        Vector3 originalPosition = targetObject.transform.position;
        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a jump tween to the sequence
        sequence.Append(targetObject.transform.DOJump(originalPosition + new Vector3(0, 1, 0), 0.5f, 1, duration / 2));

        // Add a scaling up tween to the sequence
        sequence.Join(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2));

        // Add a rotation tween to the sequence
        sequence.Join(targetObject.transform.DORotate(new Vector3(0, 360, 0), duration / 2, RotateMode.FastBeyond360));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale, duration / 2));

        // Add a move back to original position tween to the sequence
        sequence.Append(targetObject.transform.DOMove(originalPosition, duration / 2));

        // Start the sequence
        sequence.Play();
    }

    public void StartDestructableObjectAnimation(GameObject targetObject)
    {
        // If the sequence is active, don't start a new animation
        if (sequence != null && sequence.IsActive()) return;

        if (targetObject == null)
            targetObject = gameObject;

        Vector3 originalScale = targetObject.transform.localScale;

        // Create a sequence
        sequence = DOTween.Sequence();

        // Add a scaling up tween to the sequence
        sequence.Append(targetObject.transform.DOScale(originalScale * 1.2f, duration / 2));

        // Add a scaling down tween to the sequence
        sequence.Append(targetObject.transform.DOScale(Vector3.zero, duration / 2));

        // Add a callback to the sequence to deactivate the object when the animation is done
        sequence.OnComplete(() => targetObject.SetActive(false));

        // Start the sequence
        sequence.Play();
    }
}