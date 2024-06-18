using DG.Tweening;
using UnityEngine;

public class CustomNPCAnimation : MonoBehaviour
{
    public float floatDistance = 0.5f; // The distance the ghost will float up and down
    public float floatDuration = 2f; // The duration of one float cycle (up and down)

    // Start is called before the first frame update
    private void Start() => StartIdleFloating(); // Start the idle floating animation

    private void StartIdleFloating()
    {
        // Move the ghost up
        transform.DOMoveY(transform.position.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine) // Smooth start and end
            .SetLoops(-1, LoopType.Yoyo); // Loop the animation back and forth infinitely
    }
}