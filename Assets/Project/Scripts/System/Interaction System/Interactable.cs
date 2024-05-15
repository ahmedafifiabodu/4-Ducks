using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask = 6;

    [SerializeField] private MonoBehaviour _possessableScript;
    [SerializeField] private bool _possessable;
    [SerializeField] private bool _autoInteract;
    [SerializeField] private bool _useEvents;
    [SerializeField] private string _promptMessage;

    private int? _pendingLayerChange = null;

    public bool AutoInteract
    { get => _autoInteract; set { _autoInteract = value; } }

    public string PromptMessage
    { get => _promptMessage; set { _promptMessage = value; } }

    public bool Possessable
    { get => _possessable; set { _possessable = value; } }

    public bool UseEvents
    { get => _useEvents; set { _useEvents = value; } }

    internal IPossessable PossessableScript
    {
        get => _possessableScript as IPossessable;
    }

    internal Material[] OriginalMaterials { get; private set; }
    internal Material[] MaterialsWithOutline { get; private set; }

    protected virtual string OnLook() => _promptMessage;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
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

                MaterialsWithOutline = new Material[renderer.sharedMaterials.Length + 1];
                renderer.sharedMaterials.CopyTo(MaterialsWithOutline, 0);
                MaterialsWithOutline[^1] = outlineMaterial;
            }
        }
    }

    internal void ApplyOutline()
    {
        if (TryGetComponent<Renderer>(out var renderer))
            renderer.sharedMaterials = MaterialsWithOutline;
    }

    internal void RemoveOutline()
    {
        if (TryGetComponent<Renderer>(out var renderer))
            renderer.sharedMaterials = OriginalMaterials;
    }

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