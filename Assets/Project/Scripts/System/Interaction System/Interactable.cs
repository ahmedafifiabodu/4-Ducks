using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask = 6;
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private bool _autoInteract;
    [SerializeField] private bool _useEvents;
    [SerializeField] private string _promptMessage;
    [SerializeField] private bool _possessable;
    [SerializeField] private MonoBehaviour _possessableScript;

    private int? _pendingLayerChange = null;
    private Renderer _renderer;

    public LayerMask InteractableLayerMask
    { get => _interactableLayerMask; set { _interactableLayerMask = value; } }

    public Material OutlineMaterial
    { get => _outlineMaterial; set { _outlineMaterial = value; } }

    public bool AutoInteract
    { get => _autoInteract; set { _autoInteract = value; } }

    public string PromptMessage
    { get => _promptMessage; set { _promptMessage = value; } }

    public bool UseEvents
    { get => _useEvents; set { _useEvents = value; } }

    public bool Possessable
    { get => _possessable; set { _possessable = value; } }

    internal IPossessable PossessableScript
    {
        get => _possessableScript as IPossessable;
    }

    internal Material[] OriginalMaterials { get; private set; }
    internal Material[] MaterialsWithOutline { get; private set; }

    protected virtual string OnLook() => _promptMessage;

    private void Awake() => Initialize(_outlineMaterial);

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            Initialize(_outlineMaterial);

            //if (gameObject.activeInHierarchy)
            //    StartCoroutine(DelayedLayerChange());
            //else
            //    _pendingLayerChange = _interactableLayerMask;
        }
    }

    private void OnEnable()
    {
        int layer = _pendingLayerChange ?? LayerMaskToLayerNumber(_interactableLayerMask);

        if (layer >= 0 && layer <= 31)
        {
            gameObject.layer = layer;
            _pendingLayerChange = null;
        }
        else
        {
            Logging.LogError("Invalid layer value: " + layer);
        }
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

    private IEnumerator DelayedLayerChange()
    {
        yield return null;
        gameObject.layer = LayerMaskToLayerNumber(_interactableLayerMask);
    }

    internal void Initialize(Material outlineMaterial)
    {
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
        if (_outlineEnabled && _renderer != null) // Check if the outline is enabled and _renderer is not null
        {
            // Check if the outline material is already applied
            if (_outlineMaterial != null && !System.Array.Exists(_renderer.sharedMaterials, material => material == _outlineMaterial))
                _renderer.sharedMaterials = MaterialsWithOutline;
        }
        else if (!_outlineEnabled)
            RemoveOutline();
    }

    internal void RemoveOutline() => _renderer.sharedMaterials = OriginalMaterials;

    internal void BaseInteract(PlayerType _playerType)
    {
        if (_useEvents)
        {
            if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                _events.onInteract.Invoke();
        }
        else
            Interact(_playerType);
    }

    private void Interact(PlayerType _playerType)
    {
        if (_playerType.Ghost != null)
        {
            _playerType.Ghost.gameObject.SetActive(false);
            PossessableScript.Possess();
        }
        else
            Logging.Log($"(Virtual) Interacting with {gameObject.name}");
    }
}