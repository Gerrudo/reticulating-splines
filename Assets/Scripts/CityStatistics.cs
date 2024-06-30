using UnityEngine;
using TMPro;

public class CityStatistics : Singleton<CityStatistics>
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI populationText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI waterText;
    [SerializeField] private TextMeshProUGUI coalText;

    City city;

    protected override void Awake()
    {
        base.Awake();

        city = City.GetInstance();
    }

    public void UpdateUI()
    {
        //Maybe we could have the UI read a value instead of updating it individually?

        //Update Toolbar
        dayText.text = $"Day: {city.day}";

        //Update Panel
        populationText.text = $"Population: {city.population}";
        moneyText.text = $"Money: ${city.money}";
        powerText.text = $"Power: {city.power}MW";
        waterText.text = $"Water: {city.water}kL";
        coalText.text = $"Coal: {city.coal} Ton";
    }
}