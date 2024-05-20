using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask = 6;
    [SerializeField] private Material _outlineMaterial;

    [SerializeField] private bool _autoInteract;
    [SerializeField] private bool _useEvents;
    [SerializeField] private string _promptMessage;
    [SerializeField] private bool _possessable;
    [SerializeField] private MonoBehaviour _possessableScript;

    private int? _pendingLayerChange = null;
    private Renderer _renderer;

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

            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayedLayerChange());
            else
                _pendingLayerChange = _interactableLayerMask;
        }
    }

    private void OnEnable()
    {
        if (_pendingLayerChange.HasValue)
        {
            gameObject.layer = _pendingLayerChange.Value;
            _pendingLayerChange = null;
        }
        else
            gameObject.layer = _interactableLayerMask;
    }

    private IEnumerator DelayedLayerChange()
    {
        yield return null;
        gameObject.layer = _interactableLayerMask;
    }

    internal void Initialize(Material outlineMaterial)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

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
            PossessableScript.Possess();
        }
        else
            Logging.Log($"(Virtual) Interacting with {gameObject.name}");
    }
}