public class CatHealthSystem : HealthSystem
{
    private void OnEnable() => HealthCrystal._onHealthCrystalCollected.AddListener(Heal);

    private void OnDisable() => HealthCrystal._onHealthCrystalCollected.RemoveListener(Heal);
}