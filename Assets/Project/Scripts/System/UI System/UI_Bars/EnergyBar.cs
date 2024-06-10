using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private Image _image;

    private EnergySystem _energySystem;

    private void Start()
    {
        _energySystem = ServiceLocator.Instance.GetService<EnergySystem>();

        _energySystem.OnEnergyChanged.AddListener(UpdateEnergyBar);
        _image.fillAmount = _energySystem.EnergyPrecentage;
    }

    private void UpdateEnergyBar()
    {
        if (_energySystem != null)
            _image.fillAmount = _energySystem.EnergyPrecentage;
    }

    private void OnDisable() => _energySystem.OnEnergyChanged.RemoveListener(UpdateEnergyBar);

    private void OnDestroy() => _energySystem.OnEnergyChanged.RemoveListener(UpdateEnergyBar);
}