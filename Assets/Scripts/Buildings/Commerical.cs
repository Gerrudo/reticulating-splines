using System;
using System.Collections.Generic;

public class Commercial : IBuildable, IGrowable, IPowerable, IWaterable
{
    public BuildingData Data { get; set; }
    
    public BuildingData NewBuildingData(Preset buildingPreset)
    {
        Data = new BuildingData();

        Data.TileType = buildingPreset.TileType;
        Data.Level1TilBase = buildingPreset.Level1TilBase;
        Data.MaxEmployees = buildingPreset.MaxEmployees;
        Data.TileBase = buildingPreset.TileBase;
        
        Data.Jobs = new List<Guid>();
        
        return Data;
    }
    
    public void UpdateBuilding()
    {
        UpgradeBuilding();
        ConsumePower();
        ConsumeWater();
        SellGoods();
    }
    
    public void UpgradeBuilding()
    {
        if (Data.IsConnectedToRoad && Data.Jobs.Count > 10)
        {
            Data.TileBase = Data.Level1TilBase;
        }
    }

    private void SellGoods()
    {
        Data.GoodsInput = Data.Employees * 10;
    }
    
    public void ConsumePower()
    {
        Data.PowerInput = Data.Employees * 4;
    }

    public void ConsumeWater()
    {
        Data.WaterInput = Data.Employees * 2;
    }
}