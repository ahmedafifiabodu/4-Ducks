using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private HealthSystem _healthSystem;

    private void OnEnable()
    {
        _healthSystem.OnHealthChanged.AddListener(UpdateHealthBar);
    }
    private void Start()
    {
        _image.fillAmount = _healthSystem.HealthPrecentage;
    }

    private void UpdateHealthBar()
    {
        if (_healthSystem != null)
        {
            _image.fillAmount = _healthSystem.HealthPrecentage;
        }
    }
    private void OnDisable()
    {
        _healthSystem.OnHealthChanged.RemoveListener(UpdateHealthBar);
    }
}
