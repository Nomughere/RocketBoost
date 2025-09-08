using UnityEngine;
using TMPro;

public class FuelSystem : MonoBehaviour
{
    public float maxFuel = 100f;
    public float currentFuel;
    public float drainRate = 10f;

    public TextMeshProUGUI fuelText;

    private bool canUseSpace = true;
    public bool CanUseSpace => canUseSpace;

    void Start()
    {
        currentFuel = maxFuel;
        UpdateFuelUI();
    }

    // Added multiplier parameter for boost
    public void ConsumeFuel(float multiplier = 1f)
    {
        if (!canUseSpace) return;

        currentFuel -= drainRate * multiplier * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        UpdateFuelUI();

        if (currentFuel <= 0)
            canUseSpace = false;
    }

    void UpdateFuelUI()
    {
        fuelText.text = "Fuel: " + Mathf.RoundToInt(currentFuel);
    }

    public void Refuel(float amount)
    {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, maxFuel);
        if (currentFuel > 0)
            canUseSpace = true;

        UpdateFuelUI();
    }
}