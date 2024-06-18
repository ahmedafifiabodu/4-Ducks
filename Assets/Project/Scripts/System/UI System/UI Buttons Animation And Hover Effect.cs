using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))] // Ensure there's an EventTrigger component
public class UIButtonsAnimationAndHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform buttonRectTransform; // Assign in inspector
    [SerializeField] private float animationDuration = 0.3f; // Duration of the animation
    [SerializeField] private Vector3 expandedScale = new(); // Target scale when expanded

    private void Start()
    {
        if (buttonRectTransform == null)
            buttonRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData) => buttonRectTransform.DOScale(expandedScale, animationDuration).SetEase(Ease.InElastic);

    public void OnPointerExit(PointerEventData eventData) => buttonRectTransform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutElastic);
}