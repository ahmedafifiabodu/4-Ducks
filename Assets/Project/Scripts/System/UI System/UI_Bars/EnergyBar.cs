using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private EnergySystem _energySystem;
    [SerializeField] private Image _image;

    private void OnEnable()
    {
        Logging.Log(_energySystem.OnEnergyChanged);
        _energySystem.OnEnergyChanged.AddListener(UpdateEnergyBar);
        _image.fillAmount = _energySystem.EnergyPrecentage;
    }
    private void Start()
    {
        Logging.Log("Here 3");
        _image.fillAmount = _energySystem.EnergyPrecentage;
    }
    private void UpdateEnergyBar()
    {
        Logging.Log("Here 1");
        if (_energySystem != null)
        {
            Logging.Log("Here 2");
            _image.fillAmount = _energySystem.EnergyPrecentage;
        }
    }
    private void OnDisable()
    {
        _energySystem.OnEnergyChanged.RemoveListener(UpdateEnergyBar);
    }
}
