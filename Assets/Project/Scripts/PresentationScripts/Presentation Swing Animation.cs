using DG.Tweening;
using UnityEngine;

public class PresentationSwingAnimation : MonoBehaviour
{
    [SerializeField] private float swingDuration = 1.0f; // Duration of one swing
    [SerializeField] private float swingAngle = 30.0f; // Angle of the swing
    [SerializeField] private Axis swingAxis = Axis.Z; // Default swing axis

    // Define an enum for the axes
    public enum Axis
    {
        X,
        Y,
        Z
    }

    private void Start() => SwingLogo();

    private void SwingLogo()
    {
        // Determine the rotation axis based on the selected axis
        Vector3 rotationAxis = swingAxis switch
        {
            Axis.X => new Vector3(swingAngle, 0, 0),
            Axis.Y => new Vector3(0, swingAngle, 0),
            Axis.Z => new Vector3(0, 0, swingAngle),
            _ => new Vector3(0, 0, swingAngle),
        };

        // Get the current rotation of the object
        Vector3 currentRotation = transform.eulerAngles;

        // Calculate the target rotations based on the current rotation
        Vector3 targetRotationLeft = currentRotation - rotationAxis;
        Vector3 targetRotationRight = currentRotation + rotationAxis;

        // Create a sequence for the swinging animation
        Sequence swingSequence = DOTween.Sequence();

        // Add a rotation to the left (negative angle) and then to the right (positive angle) to the sequence
        // Adjusted to start from the current rotation
        swingSequence.Append(transform.DORotate(targetRotationLeft, swingDuration).SetEase(Ease.InOutQuad));
        swingSequence.Append(transform.DORotate(targetRotationRight, swingDuration * 2).SetEase(Ease.InOutQuad));
        swingSequence.Append(transform.DORotate(currentRotation, swingDuration).SetEase(Ease.InOutQuad));

        // Set the sequence to loop indefinitely
        swingSequence.SetLoops(-1, LoopType.Restart);

        // Optional: Add a slight delay between loops to simulate the natural pause at the peak of each swing
        swingSequence.AppendInterval(0.2f);
    }
}