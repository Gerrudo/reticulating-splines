public class Commercial : Building
{
    public Commercial(BuildingPreset buildingPreset)
    {
        TileBase = buildingPreset.TileBase;
        TileType = buildingPreset.TileType;
    }
    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}