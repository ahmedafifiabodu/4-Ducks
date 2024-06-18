using DG.Tweening;
using UnityEngine;

public class CustomCloudsAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 5f; // Duration of one cycle of the animation
    [SerializeField] private float moveDistance = 5f; // Horizontal movement distance
    [SerializeField] private float scaleMultiplier = 1.2f; // How much the cloud scales up

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Sequence cloudSequence;

    private void OnEnable()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;

        StartCloudAnimation();
    }

    private void OnDisable()
    {
        if (cloudSequence != null)
            cloudSequence.Kill();
    }

    private void StartCloudAnimation()
    {
        // Reset the cloud's position and scale to ensure it starts from the original state
        transform.position = originalPosition;
        transform.localScale = originalScale;

        // Create a new sequence for the cloud animation
        cloudSequence = DOTween.Sequence();

        // Move the cloud back and forth horizontally with a smooth return to the original position
        cloudSequence.Append(transform.DOMoveX(originalPosition.x + moveDistance, duration).SetEase(Ease.InOutQuad));
        cloudSequence.Append(transform.DOMoveX(originalPosition.x - moveDistance, duration * 2).SetEase(Ease.InOutQuad)); // Longer duration for a smooth return
        cloudSequence.Append(transform.DOMoveX(originalPosition.x, duration).SetEase(Ease.InOutQuad)); // Smooth transition back to the original position

        // Simultaneously, expand and shrink the cloud to create a pulsating effect
        // Adjust the timing to ensure it matches the movement for a smooth conclusion
        cloudSequence.Join(transform.DOScale(originalScale * scaleMultiplier, duration).SetEase(Ease.InOutQuad));
        cloudSequence.Join(transform.DOScale(originalScale, duration * 2).SetEase(Ease.InOutQuad)); // Match the movement's longer duration for a smooth effect

        // Set the sequence to loop indefinitely
        cloudSequence.SetLoops(-1, LoopType.Restart);

        // Optionally, add a slight rotation to the cloud for a more dynamic effect
        // Ensure the rotation also concludes smoothly
        cloudSequence.Join(transform.DORotate(new Vector3(0, 0, 10), duration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad));
        cloudSequence.Join(transform.DORotate(Vector3.zero, duration * 2, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad)); // Reset rotation smoothly

        // Start the sequence
        cloudSequence.Play();
    }
}