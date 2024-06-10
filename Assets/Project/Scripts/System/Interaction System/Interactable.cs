using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Serialized fields are private variables that can be set in the Unity editor
    [SerializeField] private LayerMask _layersInteractedWith; // Layers that cannot interact with this object

    [SerializeField] private Material _outlineMaterial; // Material used for the outline effect
    [SerializeField] private Renderer[] renderers; // Array of renderers used for the outline effect

    [SerializeField] private bool _interact; // Determines if the object can be interacted with
    [SerializeField] private bool _autoInteract; // Determines if the object interacts automatically
    [SerializeField] private bool _useEvents; // Determines if the object uses events for interaction
    [SerializeField] private string _promptMessage; // Message displayed when the object can be interacted with

    [SerializeField] private bool _useParticleEffect = false; // Determines if the object uses particle effects
    [SerializeField] private ParticleSystem _interactionParticals; // Reference to the ParticleSystem component

    private Renderer _renderer; // Renderer used for the outline effect
    private ObjectPool _objectPool;

    // Public properties for private variables
    public LayerMask LayersInteractedWith
    { get => _layersInteractedWith; set { _layersInteractedWith = value; } }

    public bool InteractProperty
    { get => _interact; set { _interact = value; } }

    public bool AutoInteract
    { get => _autoInteract; set { _autoInteract = value; } }

    public string PromptMessage
    { get => _promptMessage; set { _promptMessage = value; } }

    public bool UseEvents
    { get => _useEvents; set { _useEvents = value; } }

    public bool UseParticleEffect
    { get => _useParticleEffect; set { _useParticleEffect = value; } }

    public ParticleSystem InteractionParticals
    { get => _interactionParticals; set { _interactionParticals = value; } }

    // Properties for the original and outline materials
    internal Material[] OriginalMaterials { get; private set; }

    internal Material[] MaterialsWithOutline { get; private set; }

    // Called when the object is first initialized
    private void Awake() => Initialize(_outlineMaterial);

    private void Start() => _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

    // Called when a value is changed in the Unity editor
    private void OnValidate()
    {
        if (this != null)
            if (!Application.isPlaying)
                Initialize(_outlineMaterial);
    }

    // Initializes the outline effect
    internal void Initialize(Material outlineMaterial)
    {
        // If the object cannot be interacted with, return
        if (AutoInteract || !AutoInteract || !InteractProperty)
            return;

        // If the outline material is null, log an error and return
        if (outlineMaterial == null)
        {
            Logging.LogError("outlineMaterial is null");
            return;
        }

        // For each renderer in the renderers array
        foreach (Renderer renderer in renderers)
        {
            // If the renderer is not null
            if (renderer != null)
            {
                // Store the original materials
                OriginalMaterials = renderer.sharedMaterials;

                // Create a list of materials without duplicates
                List<Material> materialsWithoutDuplicates = new(renderer.sharedMaterials);
                materialsWithoutDuplicates.RemoveAll(material => material == outlineMaterial);

                // Create an array of materials with the outline material
                MaterialsWithOutline = new Material[materialsWithoutDuplicates.Count + 1];
                materialsWithoutDuplicates.CopyTo(MaterialsWithOutline, 0);
                MaterialsWithOutline[^1] = outlineMaterial;

                // Store the renderer
                _renderer = renderer;
                break;
            }
        }
    }

    // Applies the outline effect
    internal void ApplyOutline(bool _outlineEnabled)
    {
        // If the object interacts automatically, return
        if (AutoInteract)
            return;

        // If the outline is enabled and _renderer is not null
        if (_outlineEnabled && _renderer != null)
        {
            // If the outline material is not already applied
            if (_outlineMaterial != null && !System.Array.Exists(_renderer.sharedMaterials, material => material == _outlineMaterial))
                // Apply the outline material
                _renderer.sharedMaterials = MaterialsWithOutline;
        }
        else if (!_outlineEnabled)
            // Remove the outline effect
            RemoveOutline();
    }

    // Removes the outline effect
    internal void RemoveOutline()
    {
        // If the object interacts automatically, return
        if (AutoInteract)
            return;

        // If _renderer is not null, remove the outline material
        if (_renderer != null)
            _renderer.sharedMaterials = OriginalMaterials;
    }

    internal void BaseInteract(ObjectType _objectType)
    {
        // Check if the object's layer is included in the layers that can interact with this object
        if (((1 << _objectType.gameObject.layer) & _layersInteractedWith) != 0)
            Interact(_objectType);
        else
            // Log a warning message if the object's layer is not included in the layers that can interact with this object
            Logging.LogWarning($"Object {_objectType.name} is not interactable");
    }

    // Handles interaction with the object
    protected virtual void Interact(ObjectType _objectType)
    {
        // Play the particle effect
        if (UseParticleEffect)
        {
            if (_objectPool == null)
                _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

            // Get the particle system from the object pool
            GameObject particleInstanceObject = _objectPool.GetPooledObject(InteractionParticals.gameObject);

            // If a particle system was found in the pool
            if (particleInstanceObject != null)
            {
                // Get the ParticleSystem component
                ParticleSystem particleInstance = particleInstanceObject.GetComponent<ParticleSystem>();

                // Set the position of the particle system to the ghost's position
                particleInstance.transform.position = _objectType.transform.position;

                // Play the particle system
                particleInstance.Play();

                // Set the particle system's game object to inactive after it finishes playing
                DOVirtual.DelayedCall(particleInstance.main.duration, () => particleInstance.gameObject.SetActive(false));
            }
        }

        // Check if the object uses events for interaction
        if (_useEvents)
        {
            // Try to get the InteractableEvents component attached to the object
            if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                // Invoke the onInteract event of the InteractableEvents component
                _events.onInteract.Invoke();
        }
    }
}