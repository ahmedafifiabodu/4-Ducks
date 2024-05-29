using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private LayerMask _layersNotToBeInteractedWith;
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private bool _interact;
    [SerializeField] private bool _autoInteract;
    [SerializeField] private bool _useEvents;
    [SerializeField] private string _promptMessage;

    private Renderer _renderer;

    public LayerMask LayersNotToBeInteractedWith
    { get => _layersNotToBeInteractedWith; set { _layersNotToBeInteractedWith = value; } }

    public bool InteractProperty
    { get => _interact; set { _interact = value; } }

    public bool AutoInteract
    { get => _autoInteract; set { _autoInteract = value; } }

    public string PromptMessage
    { get => _promptMessage; set { _promptMessage = value; } }

    public bool UseEvents
    { get => _useEvents; set { _useEvents = value; } }

    internal Material[] OriginalMaterials { get; private set; }
    internal Material[] MaterialsWithOutline { get; private set; }

    private void Awake()
    {
        Initialize(_outlineMaterial);
    }

    private void OnValidate()
    {
        if (this != null)
            if (!Application.isPlaying)
                Initialize(_outlineMaterial);
    }

    private int LayerMaskToLayerNumber(LayerMask layerMask)
    {
        int layerNumber = 0;
        int mask = layerMask.value;

        while (mask > 0)
        {
            mask >>= 1;
            layerNumber++;
        }

        return layerNumber - 1;
    }

    internal void Initialize(Material outlineMaterial)
    {
        if (AutoInteract || !AutoInteract || !InteractProperty)
            return;

        if (outlineMaterial == null)
        {
            Logging.LogError("outlineMaterial is null");
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                OriginalMaterials = renderer.sharedMaterials;

                List<Material> materialsWithoutDuplicates = new(renderer.sharedMaterials);
                materialsWithoutDuplicates.RemoveAll(material => material == outlineMaterial);

                MaterialsWithOutline = new Material[materialsWithoutDuplicates.Count + 1];
                materialsWithoutDuplicates.CopyTo(MaterialsWithOutline, 0);
                MaterialsWithOutline[^1] = outlineMaterial;

                _renderer = renderer;
                break;
            }
        }
    }

    internal void ApplyOutline(bool _outlineEnabled)
    {
        if (AutoInteract)
            return;

        if (_outlineEnabled && _renderer != null) // Check if the outline is enabled and _renderer is not null
        {
            // Check if the outline material is already applied
            if (_outlineMaterial != null && !System.Array.Exists(_renderer.sharedMaterials, material => material == _outlineMaterial))
                _renderer.sharedMaterials = MaterialsWithOutline;
        }
        else if (!_outlineEnabled)
            RemoveOutline();
    }

    internal void RemoveOutline()
    {
        if (AutoInteract)
            return;

        if (_renderer != null)
            _renderer.sharedMaterials = OriginalMaterials;
    }

    internal void BaseInteract(PlayerType _playerType)
    {
        if (_playerType != null)
        {
            Logging.Log($"LayersNotToBeInteractedWith at interaction: {LayersNotToBeInteractedWith.value}");
            Logging.Log($"Player's layer: {_playerType.gameObject.layer}");
            int playerLayerMask = 1 << _playerType.gameObject.layer;
            Logging.Log($"Bitwise operation result: {(LayersNotToBeInteractedWith.value & playerLayerMask)}");

            // If LayersNotToBeInteractedWith is set (i.e., it's not zero), and the player's layer is in LayersNotToBeInteractedWith
            if (LayersNotToBeInteractedWith != 0 && (LayersNotToBeInteractedWith.value & playerLayerMask) == playerLayerMask)
            {
                return;
            }
        }

        Interact(_playerType);
    }

    protected virtual void Interact(PlayerType _playerType)
    {
        if (_useEvents)
        {
            if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                _events.onInteract.Invoke();
        }
    }
}