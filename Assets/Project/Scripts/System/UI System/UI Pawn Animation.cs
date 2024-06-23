using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPawnAnimation : MonoBehaviour
{
    [SerializeField] private List<Image> footprints; // Use Image components instead
    public float fadeDuration = 1f; // Duration of each fade in/out
    public float delayBetweenFootprints = 0.5f; // Delay between each footprint animation

    private WaitForSeconds waitForSeconds;

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(fadeDuration + delayBetweenFootprints);
        StartCoroutine(AnimateFootprints());
    }

    private IEnumerator AnimateFootprints()
    {
        while (true) // Loop indefinitely
        {
            for (int i = 0; i < footprints.Count; i++)
            {
                // Fade in the current footprint
                footprints[i].DOFade(1f, fadeDuration);

                // If not the first footprint, start fading out the previous footprint
                if (i > 0)
                {
                    footprints[i - 1].DOFade(0f, fadeDuration);
                }

                // Wait for the fade in to complete plus the delay
                yield return waitForSeconds;

                // Special handling for the last footprint to ensure smooth looping
                if (i == footprints.Count - 1)
                {
                    // Start fading out the last footprint
                    footprints[i].DOFade(0f, fadeDuration);

                    // Ensure the first footprint starts fading in before the last one fades out completely
                    // This prevents snapping
                    footprints[0].DOFade(1f, fadeDuration);

                    // Wait for the last fade out and the first fade in to complete before restarting the loop
                    yield return waitForSeconds;
                }
            }
        }
    }
}