using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask = 6;
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Renderer[] renderers;

    [SerializeField] private bool _interact;
    [SerializeField] private bool _autoInteract;
    [SerializeField] private bool _useEvents;
    [SerializeField] private string _promptMessage;

    private Renderer _renderer;

    public LayerMask InteractableLayerMask
    { get => _interactableLayerMask; set { _interactableLayerMask = value; } }

    public Material OutlineMaterial
    { get => _outlineMaterial; set { _outlineMaterial = value; } }

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

    protected virtual string OnLook() => _promptMessage;

    private void Awake() => Initialize(_outlineMaterial);

    private void OnValidate()
    {
        if (this != null)
        {
            if (!Application.isPlaying)
            {
                Initialize(_outlineMaterial);

                // Create an array to store the results
                Collider[] results = new Collider[100]; // Adjust the size as needed

                // Get all colliders within a large sphere
                int numResults = Physics.OverlapSphereNonAlloc(transform.position, 10000f, results); // Set the radius to a large enough value

                for (int i = 0; i < numResults; i++)
                {
                    // Check if the collider's game object is on the player layer
                    if (results[i].gameObject.layer == LayerMask.NameToLayer("Player")) // Replace "Player" with your player layer name
                    {
                        if (results[i].gameObject.TryGetComponent<PlayerInteract>(out var playerInteract))
                        {
                            // Capture the gameObject in a local variable
                            var localGameObject = gameObject;

                            // Delay the layer change until after OnValidate has finished
                            UnityEditor.EditorApplication.delayCall += () =>
                            {
                                if (localGameObject != null) // Check if the GameObject is not null
                                {
                                    localGameObject.layer = LayerMaskToLayerNumber(playerInteract.InteractableLayerMask);
                                }
                            };
                            break;
                        }
                    }
                }
            }
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

    internal void BaseInteract(PlayerType _playerType) => Interact(_playerType);

    protected virtual void Interact(PlayerType _playerType)
    {
        if (_useEvents)
        {
            if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                _events.onInteract.Invoke();
        }
    }
}