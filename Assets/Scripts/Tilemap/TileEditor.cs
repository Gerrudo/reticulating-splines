using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.EventSystems;

public class TileEditor : Singleton<TileEditor>
{
    //Credit to https://www.youtube.com/@VelvaryGames for a lot of this code.
    
    private PlayerInput playerInput;

    private new Camera camera;

    [SerializeField] public Tilemap previewMap, defaultMap, terrainMap;
    private TileBase tileBase;
    private BuildingPreset selectedObj;

    private Vector2 mousePosition;
    private Vector3Int currentGridPosition;
    private Vector3Int previousGridPosition;
    
    private bool isPointerOverGameObject;

    private bool holdActive;
    private Vector3Int holdStartPosition;
    private BoundsInt area;

    private City city;

    protected override void Awake()
    {
        base.Awake();

        city = City.GetInstance();

        playerInput = new PlayerInput();

        camera = Camera.main;
    }

    private void Update()
    {
        isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
        
        if (!selectedObj) return;
        
        Vector2 pos = camera.ScreenToWorldPoint(mousePosition);

        var gridPos = previewMap.WorldToCell(pos);

        if (gridPos == currentGridPosition) return;
            
        previousGridPosition = currentGridPosition;
        currentGridPosition = gridPos;

        UpdatePreview();

        if (!holdActive) return;
        HandleDrawing();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.started += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClick;
        
        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
        
        playerInput.Gameplay.MousePosition.performed += OnMouseMove;

        playerInput.Gameplay.KeyboardEsc.performed += OnKeyboardEsc;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Gameplay.MouseLeftClick.performed += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.started += OnLeftClick;
        playerInput.Gameplay.MouseLeftClick.canceled += OnLeftClick;
        
        playerInput.Gameplay.MouseRightClick.performed += OnRightClick;
        
        playerInput.Gameplay.MousePosition.performed += OnMouseMove;

        playerInput.Gameplay.KeyboardEsc.performed += OnKeyboardEsc;
    }

    private BuildingPreset SelectedObj
    {
        set
        {
            //Do not set this to the name of the setter, will crash unity editor lol
            holdActive = false;
            previewMap.ClearAllTiles();
            
            selectedObj = value;

            tileBase = selectedObj ? selectedObj.TileBase : null;

            UpdatePreview();
        }
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePosition = ctx.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (!selectedObj || isPointerOverGameObject) return;
        
        if (ctx.phase == InputActionPhase.Started)
        {
            //TODO: can possibly remove SlowTapInteraction and have only certain tiles able to have click and drag interactions
            if (ctx.interaction is SlowTapInteraction)
            {
                holdActive = true;
            }

            holdStartPosition = currentGridPosition;
            HandleDrawing();
        }
        else
        {
            if (!holdActive) return;
                
            holdActive = false;
            HandleDrawingRelease();
        }
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        SelectedObj = null;
    }

    private void OnKeyboardEsc(InputAction.CallbackContext ctx)
    {
        SelectedObj = null;
    }

    public void ObjectSelected(BuildingPreset obj)
    {
        SelectedObj = obj;
    }

    private void UpdatePreview()
    {
        previewMap.SetTile(previousGridPosition, null);
        previewMap.SetTile(currentGridPosition, tileBase);
    }

    private void HandleDrawing()
    {
        if (!selectedObj) return;

        switch (selectedObj.PlacementType)
        {
            case PlacementType.Line:
                RenderLine();
                
                break;
            case PlacementType.Rectangle:
                RenderRectangle();
                
                break;
        }
    }

    private void HandleDrawingRelease()
    {
        if (!selectedObj) return;
        
        switch (selectedObj.PlacementType)
        {
            case PlacementType.Line:
            case PlacementType.Rectangle:
                foreach (var point in TilemapExtension.AllPositionsWithin2D(area))
                {
                    DrawItem(defaultMap, point, tileBase);
                }
                
                previewMap.ClearAllTiles();
                
                break;
            case PlacementType.Single: default:
                DrawItem(defaultMap, currentGridPosition, tileBase);
                
                SelectedObj = null;
                
                break;
        }
    }

    private void NewCityTile(Vector3Int position)
    {
        //Don't add new city tiles for tilemap tools
        if (selectedObj && selectedObj.GetType() == typeof(TilemapTool)) return;
        if (!city.CanPlaceNewTile(selectedObj)) return;
                    
        //Must provide the point, not currentGridPosition
        city.NewTile(position, selectedObj);
    }

    private void RenderRectangle()
    {
        previewMap.ClearAllTiles();
        
        area.xMin = Mathf.Min(currentGridPosition.x,holdStartPosition.x);
        area.xMax = Mathf.Max(currentGridPosition.x,holdStartPosition.x);
        area.yMin = Mathf.Min(currentGridPosition.y,holdStartPosition.y);
        area.yMax = Mathf.Max(currentGridPosition.y,holdStartPosition.y);
        
        DrawArea(previewMap);
    }

    private void RenderLine()
    {
        previewMap.ClearAllTiles();

        float diffX = Mathf.Abs(currentGridPosition.x - holdStartPosition.x);
        float diffY = Mathf.Abs(currentGridPosition.y - holdStartPosition.y);

        var isLineHorizontal = diffX >= diffY;

        if (isLineHorizontal)
        {
            area.xMin = Mathf.Min(currentGridPosition.x,holdStartPosition.x);
            area.xMax = Mathf.Max(currentGridPosition.x,holdStartPosition.x);
            area.yMin = holdStartPosition.y;
            area.yMax = holdStartPosition.y;
        }
        else
        {
            area.xMin = currentGridPosition.x;
            area.xMax = currentGridPosition.x;
            area.yMin = Mathf.Min(currentGridPosition.y,holdStartPosition.y);
            area.yMax = Mathf.Max(currentGridPosition.y,holdStartPosition.y);
        }
        
        DrawArea(previewMap);
    }

    private void DrawArea(Tilemap tilemap)
    {
        for (var x = area.xMin; x <= area.xMax; x++)
        {
            for (var y = area.yMin; y <= area.yMax; y++)
            {
                DrawItem(tilemap, new Vector3Int(x, y, 0), tileBase);
            }
        }
    }

    public void DrawItem(Tilemap tilemap, Vector3Int gridPosition, TileBase tile)
    {
        if (selectedObj && tilemap != previewMap && selectedObj.GetType() == typeof(TilemapTool))
        {
            var tool = (TilemapTool)selectedObj;
            
            tool.Use(gridPosition, tilemap);
        }
        else
        {
            //If the placement is allowed, we won't even draw the item.
            if (!PlacementAllowed(gridPosition)) return;
            
            //TODO: Replace with a better condition check
            if (tilemap == defaultMap)
            {
                NewCityTile(gridPosition);
            }
            
            tilemap.SetTile(gridPosition, tile);

            //Required for our network tile rules
            tilemap.RefreshAllTiles();   
        }
    }

    private bool PlacementAllowed(Vector3Int gridPosition)
    {
        var hasTerrainTile = terrainMap.HasTile(gridPosition - new Vector3Int(1, 1, 0));
        var hasBuildingTile = defaultMap.HasTile(gridPosition);

        return hasTerrainTile && !hasBuildingTile;
    }
}