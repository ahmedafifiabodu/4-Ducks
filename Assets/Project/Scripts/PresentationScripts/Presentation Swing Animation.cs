using DG.Tweening;
using UnityEngine;

public class PresentationSwingAnimation : MonoBehaviour
{
    [SerializeField] private float swingDuration = 1.0f; // Duration of one swing
    [SerializeField] private float swingAngle = 30.0f; // Angle of the swing

    private void Start() => SwingLogo();

    private void SwingLogo()
    {
        // Create a sequence for the swinging animation
        Sequence swingSequence = DOTween.Sequence();

        // Add a rotation to the left (negative angle) and then to the right (positive angle) to the sequence
        swingSequence.Append(transform.DORotate(new Vector3(0, 0, -swingAngle), swingDuration).SetEase(Ease.InOutQuad));
        swingSequence.Append(transform.DORotate(new Vector3(0, 0, swingAngle), swingDuration * 2).SetEase(Ease.InOutQuad));
        swingSequence.Append(transform.DORotate(new Vector3(0, 0, 0), swingDuration).SetEase(Ease.InOutQuad));

        // Set the sequence to loop indefinitely
        swingSequence.SetLoops(-1, LoopType.Restart);

        // Optional: Add a slight delay between loops to simulate the natural pause at the peak of each swing
        swingSequence.AppendInterval(0.2f);
    }
}