﻿using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    Road,
    Residential,
    Commerical,
    Industrial,
    Generator,
    WaterTower,
    CoalMine
}

[CreateAssetMenu(fileName = "PlaceableTile", menuName = "PlaceableTile")]
public class PlaceableTile : ScriptableObject
{
    [SerializeField] private TileType tileType;
    [SerializeField] private TileBase tileBase;
    [SerializeField] private TileBase level1Tilebase;
    [SerializeField] private int moneyPerDay;
    [SerializeField] private int costToBuild;
    private bool isConnectedToRoad;

    public TileBase TileBase
    {
        get
        {
            return tileBase;
        }
    }

    public TileBase Level1Tilebase
    {
        get
        {
            return level1Tilebase;
        }
    }

    public TileType TileType
    {
        get
        {
            return tileType;
        }
    }

    public int MoneyPerDay
    {
        get
        {
            return moneyPerDay;
        }
    }

    public int CostToBuild {
        get
        {
            return costToBuild;
        }
    }

    public bool IsConnectedToRoad
    {
        get
        {
            return isConnectedToRoad;
        }
        set
        {
            isConnectedToRoad = value;
        }
    }
}