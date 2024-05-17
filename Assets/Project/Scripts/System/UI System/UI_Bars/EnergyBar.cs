using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private EnergySystem _energySystem;
    [SerializeField] private Image _image;

    private void OnEnable()
    {
        _energySystem.OnEnergyChanged.AddListener(UpdateEnergyBar);
        _image.fillAmount = _energySystem.EnergyPrecentage;
    }

    private void UpdateEnergyBar()
    {
        if (_energySystem != null)
        {
            _image.fillAmount = _energySystem.EnergyPrecentage;
        }
    }
    private void OnDisable()
    {
        _energySystem.OnEnergyChanged.RemoveListener(UpdateEnergyBar);
    }
}
