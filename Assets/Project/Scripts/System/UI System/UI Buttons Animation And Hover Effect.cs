using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))] // Ensure there's an EventTrigger component
public class UIButtonsAnimationAndHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform buttonRectTransform; // Assign in inspector
    [SerializeField] private TMP_Text buttonText; // Assign your TextMeshPro text component in inspector
    [SerializeField] private float animationDuration = 0.3f; // Duration of the animation
    [SerializeField] private Vector3 expandedScale = new(); // Target scale when expanded
    [SerializeField] private Color outlineColor = Color.white; // Outline color
    [SerializeField] private float outlineWidth = 0.1f; // Outline width

    private Material originalMaterial; // To store the original material
    private Material outlineMaterial; // Material with outline

    private void Start()
    {
        if (buttonRectTransform == null)
            buttonRectTransform = GetComponent<RectTransform>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        // Store the original material
        originalMaterial = buttonText.fontMaterial;

        // Create a new material instance for the outline
        outlineMaterial = new Material(originalMaterial);
        EnableOutline(outlineMaterial, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up animation
        buttonRectTransform.DOScale(expandedScale, animationDuration).SetEase(Ease.InElastic);

        // Apply the outline material
        buttonText.fontMaterial = outlineMaterial;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale down animation
        buttonRectTransform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutElastic);

        // Revert to the original material
        buttonText.fontMaterial = originalMaterial;
    }

    private void EnableOutline(Material material, bool enable)
    {
        if (enable)
        {
            material.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
            material.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
        }
        else
        {
            material.SetFloat(ShaderUtilities.ID_OutlineWidth, 0);
        }
    }
}