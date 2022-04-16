using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    public bool settings_AdaptGenerationToScreen;
    public int settings_PixelsPerTile;
    public GamePlay game_Play;

    public int world_X;
    public int world_Y;
    
        [Space]

    public int world_Seed;
    public bool world_RandomSeed;
    
        [Space]

    [Range(0.0f, 100.0f)]
    public float mountains_Density;
    [Range(0.0f, 100.0f)]
    public float mountains_SecondaryDensity;
    public Enum.Size mountains_Size;

        [Space]

    [Range(0.0f, 100.0f)]
    public float forests_Density;
    public Enum.Size forests_Size;

        [Space]

    [Range(0, 5)]
    public int lakes_MaxNumber;
    public int lake_MinRepetitions;
    public int lake_MaxRepetitions;
    public Enum.Size lakes_Size;

        [Space]

    [Range(0, 5)]
    public int rivers_Number;
    [Range(50, 250)]
    public int rivers_MinLength;
    [Range(50, 250)]
    public int rivers_MaxLength;
    [Range(1.0f, 100.0f)]
    public float rivers_Straightness;

        [Space]

    public Enum.ErosionForce erosion_GlobalForce;
    public Enum.ErosionForce erosion_River;

        [Space]

    [Range(0, 5)]
    public int roads_Number;
    [Range(0, 50)]
    public int road_MaxSegmentLength;
    [Range(0, 50)]
    public int road_MinSegmentLength;

        [Space]

    /*[Range(0, 10)]
    public float cities_MaxNumber;*/

    [Range(0.0f, 100.0f)]
    public float cities_Density;
    public Enum.Size cities_Size;

        [Space]

    public GameObject tile_Plain;
    public GameObject tile_Mountain;
    public GameObject tile_Forest;
    public GameObject tile_Road;
    public GameObject tile_City;
    public GameObject tile_Lake;
    public GameObject tile_River;
    public GameObject plain_Plane;

    //----//
    
    public Tile[,] world_Matrix;

    //----//

    public TouchController touch_Controller;

    //----//

    private bool world_Ready = false;

    //----//

    public void SetWorldSeed()
    {
        if(world_RandomSeed)
        {
            world_Seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Random.InitState(world_Seed);
    }

    public void InitializeWorld()
    {
        if(settings_AdaptGenerationToScreen)
        {
            world_X = (int)(Screen.width / settings_PixelsPerTile);
            world_Y = (int)(Screen.height / settings_PixelsPerTile);
        }

        world_Matrix = new Tile[world_X, world_Y];

        for(int i = 0; i < world_X; i++)
        {
            for(int j = 0; j < world_Y; j++)
            {
                world_Matrix[i, j] = new Tile(i, j);
            }
        }
    }

    private void Start()
    {
        GenerateWorld();

        touch_Controller.SetCameraLimits(Vector2.zero, new Vector2((world_X - 1) * tile_Plain.transform.localScale.x, (world_Y - 1) * tile_Plain.transform.localScale.y));
    }

    public void GenerateWorld()
    {
        world_Ready = false;

        DeleteLevel();
        SetWorldSeed();

        InitializeWorld();

        AddMountains();
        AddForests();
        AddLakes();

        AddRoads();
        AddRivers();

        AddErosion();

        DrawLevel();

        world_Ready = true;

        game_Play.AddTroops();
        game_Play.DrawTroops();

    }

    //*-*-*-*//

    public bool IsLevelReady()
    {
        return world_Ready;
    }

    public Enum.TileType TileType(int tile_CoordinateX, int tile_CoordinateY)
    {
        return world_Matrix[tile_CoordinateX, tile_CoordinateY].tile_Type;
    }

    public int WorldXSize()
    {
        return world_X;
    }

    public int WorldYSize()
    {
        return world_Y;
    }

    //*-*-*-*//

    private void AddRivers()
    {
        List<Enum.TileType> river_TilesProibited = new List<Enum.TileType>();
        river_TilesProibited.Add(Enum.TileType.Road);
        river_TilesProibited.Add(Enum.TileType.Lake);

        Vector2 river_Coordinates;
        List<Enum.Cardinal> river_DirectionProibited = new List<Enum.Cardinal>();
        Enum.Cardinal river_PrevDirection = Enum.Cardinal.None;

        int river_Counter = 0;

        //----//

        RiverNew:

        int river_Length = Random.Range(rivers_MinLength, rivers_MaxLength);

        Enum.Cardinal river_MainDirection = (Enum.Cardinal)(Random.Range(0, 4) * 2);

        switch(river_MainDirection)
        {
            case Enum.Cardinal.East:    river_Coordinates = new Vector2(0, Random.Range(0, world_Y));

                                        river_DirectionProibited.Add(Enum.Cardinal.SouthWest);
                                        river_DirectionProibited.Add(Enum.Cardinal.West);
                                        river_DirectionProibited.Add(Enum.Cardinal.NorthWest);

                                        break;

            case Enum.Cardinal.South:   river_Coordinates = new Vector2(Random.Range(0, world_X), world_Y - 1);

                                        river_DirectionProibited.Add(Enum.Cardinal.NorthWest);
                                        river_DirectionProibited.Add(Enum.Cardinal.North);
                                        river_DirectionProibited.Add(Enum.Cardinal.NorthEast);

                                        break; 

            case Enum.Cardinal.West:    river_Coordinates = new Vector2(world_X - 1, Random.Range(0, world_Y));

                                        river_DirectionProibited.Add(Enum.Cardinal.NorthEast);
                                        river_DirectionProibited.Add(Enum.Cardinal.East);
                                        river_DirectionProibited.Add(Enum.Cardinal.SouthEast);

                                        break;

            case Enum.Cardinal.North:   river_Coordinates = new Vector2(Random.Range(0, world_X), 0);

                                        river_DirectionProibited.Add(Enum.Cardinal.SouthEast);
                                        river_DirectionProibited.Add(Enum.Cardinal.South);
                                        river_DirectionProibited.Add(Enum.Cardinal.SouthWest);

                                        break; 

            default:                    river_Coordinates = new Vector2(0, 0);

                                        break;
        }

        //Debug.Log("Direzione principale fiume " + river_Counter  + ": " + river_MainDirection + " | Coordinate: " + river_Coordinates);

        world_Matrix[(int)river_Coordinates.x, (int)river_Coordinates.y].tile_Type = Enum.TileType.River;

        int river_ActualLength = 1;
        int river_LoopController = 0;

        //----//

        RiverDirection:

            //***//

        river_LoopController++;

        if(river_LoopController > 10000)
        {
            goto RiverEnd;
        }

            //***//

        Enum.Cardinal river_Direction = (Enum.Cardinal)(Random.Range(0, 8));

        if(river_DirectionProibited.Contains(river_Direction) && river_Direction == river_PrevDirection)
        {
            goto RiverDirection;
        }
        else
        {
            river_PrevDirection = river_Direction;

            if(river_Direction != river_MainDirection)
            {
                if(Random.Range(0.0f, 100.0f) > 35.0f)
                {
                    river_Direction = river_MainDirection;
                    river_PrevDirection = river_MainDirection;

                }
            }
        }

        //river_ActualLength--;

        //----//

        RiverAddTile:

        AddRiverErosion((int)river_Coordinates.x, (int)river_Coordinates.y);

        if(river_ActualLength > river_Length)
        {
            goto RiverEnd;
        }

        switch(river_Direction)
                {
                    case Enum.Cardinal.East:        if(river_Coordinates.x + 1 < world_X)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x + 1, (int)river_Coordinates.y].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x + 1, (int)river_Coordinates.y].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x + 1, river_Coordinates.y);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;

                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.East)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }

                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.SouthEast:   if(river_Coordinates.x + 1 < world_X && river_Coordinates.y - 1 >= 0)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x + 1, (int)river_Coordinates.y - 1].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x + 1, (int)river_Coordinates.y - 1].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x + 1, river_Coordinates.y - 1);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                        
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.East || river_MainDirection == Enum.Cardinal.South)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }

                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.South:       if(river_Coordinates.y - 1 >= 0)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x, (int)river_Coordinates.y - 1].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x, (int)river_Coordinates.y - 1].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x, river_Coordinates.y - 1);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                            
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.South)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }

                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.SouthWest:   if(river_Coordinates.x - 1 >= 0 && river_Coordinates.y - 1 >= 0)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x - 1, (int)river_Coordinates.y - 1].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x - 1, (int)river_Coordinates.y - 1].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x - 1, river_Coordinates.y - 1);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                            
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.South || river_MainDirection == Enum.Cardinal.West)
                                                        {
                                                            goto RiverEnd;
                                                        }

                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.West:        if(river_Coordinates.x - 1 >= 0)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x - 1, (int)river_Coordinates.y].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x - 1, (int)river_Coordinates.y].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x - 1, river_Coordinates.y);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                            
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.West)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }
                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                        
                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.NorthWest:   if(river_Coordinates.x - 1 >= 0 && river_Coordinates.y + 1 < world_Y)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x - 1, (int)river_Coordinates.y + 1].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x - 1, (int)river_Coordinates.y + 1].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x - 1, river_Coordinates.y + 1);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                            
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.West || river_MainDirection == Enum.Cardinal.North)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }
                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                        
                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.North:       if(river_Coordinates.y + 1 < world_Y)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x, (int)river_Coordinates.y + 1].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x, (int)river_Coordinates.y + 1].tile_Type = Enum.TileType.River;
                                                            }

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                            
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.North)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }
                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);

                                                        goto RiverDirection;
                                                    }

                    case Enum.Cardinal.NorthEast:   if(river_Coordinates.x + 1 < world_X && river_Coordinates.y + 1 < world_Y)
                                                    {
                                                        if(Random.Range(0.0f, 100.0f) <= rivers_Straightness)
                                                        {
                                                            if(!river_TilesProibited.Contains(world_Matrix[(int)river_Coordinates.x + 1, (int)river_Coordinates.y + 1].tile_Type))
                                                            {
                                                                world_Matrix[(int)river_Coordinates.x + 1, (int)river_Coordinates.y + 1].tile_Type = Enum.TileType.River;
                                                            }

                                                            river_Coordinates = new Vector2(river_Coordinates.x + 1, river_Coordinates.y + 1);

                                                            //Debug.Log("Tile coordinate: " + river_Coordinates);

                                                            river_ActualLength++;
                                                            
                                                            goto RiverAddTile;
                                                        }
                                                        else
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverDirection;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(river_MainDirection == Enum.Cardinal.East || river_MainDirection == Enum.Cardinal.North)
                                                        {
                                                            //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);
                                                            goto RiverEnd;
                                                        }
                                                        //Debug.Log("Segmento fiume " + river_Counter  + ": " + river_Direction + " | Tile posizionate: " + river_ActualLength);

                                                        goto RiverDirection;
                                                    }

                    default:                        goto RiverEnd;
                }



        //----//

        RiverEnd:

        river_Counter++;

        if(river_Counter < rivers_Number)
        {
            goto RiverNew;
        }

        //----//
    }

    private void AddRiverErosion(int pos_X, int pos_Y)
    {
        if(erosion_River != Enum.ErosionForce.None && pos_X > 1 && pos_X < world_X - 2 && pos_Y > 1 && pos_Y < world_Y - 2)
        {
            if(world_Matrix[pos_X + 1, pos_Y].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 50.0f * (int)erosion_River)
            {
                world_Matrix[pos_X + 1, pos_Y].tile_Type = Enum.TileType.Plain;

                if(world_Matrix[pos_X + 2, pos_Y].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 25.0f * (int)erosion_River)
                {
                    world_Matrix[pos_X + 2, pos_Y].tile_Type = Enum.TileType.Plain;
                }      
            }

            if(world_Matrix[pos_X - 1, pos_Y].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 50.0f * (int)erosion_River)
            {
                world_Matrix[pos_X - 1, pos_Y].tile_Type = Enum.TileType.Plain;

                if(world_Matrix[pos_X - 2, pos_Y].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 25.0f * (int)erosion_River)
                {
                    world_Matrix[pos_X - 2, pos_Y].tile_Type = Enum.TileType.Plain;
                }      
            }

            //----//

            if(world_Matrix[pos_X, pos_Y + 1].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 50.0f * (int)erosion_River)
            {
                world_Matrix[pos_X, pos_Y + 1].tile_Type = Enum.TileType.Plain;

                if(world_Matrix[pos_X, pos_Y + 2].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 25.0f * (int)erosion_River)
                {
                    world_Matrix[pos_X, pos_Y + 2].tile_Type = Enum.TileType.Plain;
                }     
            }

            if(world_Matrix[pos_X, pos_Y - 1].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 50.0f * (int)erosion_River)
            {
                world_Matrix[pos_X, pos_Y - 1].tile_Type = Enum.TileType.Plain;

                if(world_Matrix[pos_X, pos_Y - 2].tile_Type == Enum.TileType.Mountain && Random.Range(0.0f, 100.0f) <= 25.0f * (int)erosion_River)
                {
                    world_Matrix[pos_X, pos_Y - 2].tile_Type = Enum.TileType.Plain;
                }     
            }
        }
    }


    //*-*-*-*//

    private void AddMountains()
    {
        for(int i = 0; i < world_X; i++)
        {
            for(int j = 0; j < world_Y; j++)
            {
                if(Random.Range(0.0f, 100.0f) <= mountains_Density && world_Matrix[i, j].tile_Type != Enum.TileType.Mountain)
                {
                    int mountain_MinExtensionX;
                    int mountain_MinExtensionY;

                    int mountain_MaxExtensionX;
                    int mountain_MaxExtensionY;

                    switch(mountains_Size)
                    {
                        case Enum.Size.Small:   mountain_MinExtensionX = (int)((float)(world_X / 100) * 0); 
                                                mountain_MinExtensionY = (int)((float)(world_Y / 100) * 0);

                                                mountain_MaxExtensionX = (int)((float)(world_X / 100) * 5); 
                                                mountain_MaxExtensionY = (int)((float)(world_Y / 100) * 5);

                                                break;

                        case Enum.Size.Medium:  mountain_MinExtensionX = (int)((float)(world_X / 100) * 5); 
                                                mountain_MinExtensionY = (int)((float)(world_Y / 100) * 5);

                                                mountain_MaxExtensionX = (int)((float)(world_X / 100) * 10); 
                                                mountain_MaxExtensionY = (int)((float)(world_Y / 100) * 10);

                                                break;

                        case Enum.Size.Big:     mountain_MinExtensionX = (int)((float)(world_X / 100) * 10); 
                                                mountain_MinExtensionY = (int)((float)(world_Y / 100) * 10);

                                                mountain_MaxExtensionX = (int)((float)(world_X / 100) * 15); 
                                                mountain_MaxExtensionY = (int)((float)(world_Y / 100) * 15);

                                                break;

                        default:                mountain_MinExtensionX = 0; 
                                                mountain_MinExtensionY = 0;

                                                mountain_MaxExtensionX = 0; 
                                                mountain_MaxExtensionY = 0;
                        
                                                break;

                    }

                    int x_Min = i;
                    int y_Min = j;

                    int x_Max;
                    int x_Delta = Random.Range(mountain_MinExtensionX, mountain_MaxExtensionX + 1);
                    if(i + x_Delta < world_X)
                    {
                        x_Max = x_Min + x_Delta;
                    }
                    else
                    {
                        x_Max = world_X - 1;
                    }

                    int y_Max;
                    int y_Delta = Random.Range(mountain_MinExtensionY, mountain_MaxExtensionY + 1);
                    if(j + y_Delta < world_Y)
                    {
                        y_Max = y_Min + y_Delta;
                    }
                    else
                    {
                        y_Max = world_Y - 1;
                    }

                    for(int x = x_Min; x <= x_Max; x++)
                    {
                        for(int y = y_Min; y <= y_Max; y++)
                        {
                            if(Random.Range(0.0f, 100.0f) <= mountains_SecondaryDensity)
                            {
                                world_Matrix[x, y].tile_Type = Enum.TileType.Mountain;
                            }
                        }
                    }
                }
            }
        }
    }

    //*-*-*-*//

    private void AddForests()
    {
        for(int i = 0; i < world_X - 1; i++)
        {
            for(int j = 0; j < world_Y - 1; j++)
            {
                if(Random.Range(0.0f, 100.0f) <= forests_Density)
                {
                    int forest_MaxExtensionX;
                    int forest_MaxExtensionY;

                    switch(forests_Size)
                    {
                        case Enum.Size.Small:   forest_MaxExtensionX = (int)(world_X / 60) + 1; 
                                                forest_MaxExtensionY = (int)(world_Y / 60) + 1;
                                                break;

                        case Enum.Size.Medium:  forest_MaxExtensionX = (int)(world_X / 30) + 2; 
                                                forest_MaxExtensionY = (int)(world_Y / 30) + 2;
                                                break;

                        case Enum.Size.Big:     forest_MaxExtensionX = (int)(world_X / 15) + 3; 
                                                forest_MaxExtensionY = (int)(world_Y / 15) + 3;
                                                break;

                        default:                forest_MaxExtensionX = 0; 
                                                forest_MaxExtensionY = 0;
                                                break;                        
                    }

                    int x_Min = i;
                    int y_Min = j;

                    int x_Max;
                    int x_Delta = Random.Range(0, forest_MaxExtensionX + 1);
                    if(i + x_Delta < world_X)
                    {
                        x_Max = x_Min + x_Delta;
                    }
                    else
                    {
                        x_Max = world_X - 1;
                    }

                    int y_Max;
                    int y_Delta = Random.Range(0, forest_MaxExtensionY + 1);
                    if(j + y_Delta < world_Y)
                    {
                        y_Max = y_Min + y_Delta;
                    }
                    else
                    {
                        y_Max = world_Y - 1;
                    }

                    //----//

                    for(int k = x_Min; k <= x_Max; k++)
                    {
                        int y_Start = Random.Range(y_Min, y_Max + 1);
                        int y_End = Random.Range(y_Start, y_Max + 1);

                        for(int h = y_Start; h <= y_End; h++)
                        {
                            if(world_Matrix[k, h].tile_Type == Enum.TileType.Plain)
                            {
                                world_Matrix[k, h].tile_Type = Enum.TileType.Forest;
                            }
                        }
                    }
                    for(int k = y_Min; k <= y_Max; k++)
                    {
                        int x_Start = Random.Range(x_Min, x_Max + 1);
                        int x_End = Random.Range(x_Start, x_Max + 1);

                        for(int h = x_Start; h <= x_End; h++)
                        {
                            if(world_Matrix[h, k].tile_Type == Enum.TileType.Plain)
                            {
                                world_Matrix[h, k].tile_Type = Enum.TileType.Forest;
                            }
                        }
                    }
                }
            }
        }
    }

    //*-*-*-*//

    private void AddLakes()
    {
        for(int k = 0; k < lakes_MaxNumber; k++)
        {
            int lake_MaxExtensionX;
            int lake_MaxExtensionY;

            int lake_MinExtensionX;
            int lake_MinExtensionY;

            switch(lakes_Size)
            {
                case Enum.Size.Small:   lake_MinExtensionX = (int)((float)(world_X / 100) * 0); 
                                        lake_MinExtensionY = (int)((float)(world_X / 100) * 0);

                                        lake_MaxExtensionX = (int)((float)(world_X / 100) * 10); 
                                        lake_MaxExtensionY = (int)((float)(world_X / 100) * 10);
                                        break;

                case Enum.Size.Medium:  lake_MinExtensionX = (int)((float)(world_X / 100) * 10); 
                                        lake_MinExtensionY = (int)((float)(world_X / 100) * 10);

                                        lake_MaxExtensionX = (int)((float)(world_X / 100) * 20); 
                                        lake_MaxExtensionY = (int)((float)(world_X / 100) * 20);
                                        break;

                case Enum.Size.Big:     lake_MinExtensionX = (int)((float)(world_X / 100) * 20); 
                                        lake_MinExtensionY = (int)((float)(world_X / 100) * 20);

                                        lake_MaxExtensionX = (int)((float)(world_X / 100) * 30); 
                                        lake_MaxExtensionY = (int)((float)(world_X / 100) * 30);
                                        break;

                default:                lake_MinExtensionX = 0; 
                                        lake_MinExtensionY = 0;

                                        lake_MaxExtensionX = 0; 
                                        lake_MaxExtensionY = 0;
                                        break;                        
                }

                int lake_MinExtension = Mathf.Min(lake_MinExtensionX, lake_MinExtensionY);
                int lake_MaxExtension = Mathf.Min(lake_MaxExtensionX, lake_MaxExtensionY);

                int x_Delta = Random.Range(lake_MinExtension, lake_MaxExtension + 1);
                int y_Delta = Random.Range(lake_MinExtension, lake_MaxExtension + 1);

                int debug_LoopController = 0;

                RandomStart:

                if(debug_LoopController > 1000)
                {
                    goto StopLakeGeneration;
                }

                Vector2 lake_StartPoint = new Vector2(Random.Range(0, world_X - x_Delta), Random.Range(0, world_Y - y_Delta));

                if(world_Matrix[(int)lake_StartPoint.x, (int)lake_StartPoint.y].tile_Type == Enum.TileType.Lake)
                {
                    debug_LoopController++;
                    goto RandomStart;
                }

                //----//

                int x_Min = (int)lake_StartPoint.x;
                int y_Min = (int)lake_StartPoint.y;

                //----//

                int x_Max = x_Min + x_Delta;
                int y_Max = y_Min + y_Delta;

                //----//

                int x_SubDelta = x_Max - x_Min;
                int y_SubDelta = y_Max - y_Min;

                //----//

                for(int i = lake_MinRepetitions; i < lake_MaxRepetitions; i++)
                {
                    int lake_SubExtension = Random.Range(0, Mathf.Min(x_SubDelta, y_SubDelta));
                    Vector2 lake_SubStart = new Vector2(Random.Range(x_Min, x_Max - lake_SubExtension + 1), Random.Range(y_Min, y_Max - lake_SubExtension + 1));

                    for(int x = (int)lake_SubStart.x; x < (int)lake_SubStart.x + lake_SubExtension; x++)
                    {
                        for(int y = (int)lake_SubStart.y; y < (int)lake_SubStart.y + lake_SubExtension; y++)
                        {
                            world_Matrix[x, y].tile_Type = Enum.TileType.Lake;
                        }
                    }
                }
            }
        //}

        StopLakeGeneration:

        ;
    }

    //*-*-*-*//

    private void AddErosion()
    {
        List<Enum.TileType> erosion_TilesToErode = new List<Enum.TileType>();
        erosion_TilesToErode.Add(Enum.TileType.Mountain);
        erosion_TilesToErode.Add(Enum.TileType.Forest);
        erosion_TilesToErode.Add(Enum.TileType.Lake);

        for(int i = 1; i < world_X - 1; i++)
        {
            for(int j = 1; j < world_Y - 1; j++)
            {
               if(erosion_TilesToErode.Contains(world_Matrix[i, j].tile_Type) && erosion_GlobalForce != Enum.ErosionForce.None)
               {
                   int counter_SameTile = 0;

                   if(world_Matrix[i - 1, j].tile_Type == world_Matrix[i, j].tile_Type)
                   {
                       counter_SameTile++;
                   }
                   if(world_Matrix[i + 1, j].tile_Type == world_Matrix[i, j].tile_Type)
                   {
                       counter_SameTile++;
                   }
                   if(world_Matrix[i, j - 1].tile_Type == world_Matrix[i, j].tile_Type)
                   {
                       counter_SameTile++;
                   }
                   if(world_Matrix[i, j + 1].tile_Type == world_Matrix[i, j].tile_Type)
                   {
                       counter_SameTile++;
                   }

                   if(counter_SameTile < (int)erosion_GlobalForce)
                    {
                        world_Matrix[i, j].tile_Type = Enum.TileType.Plain;
                    }
               }
            }
        }
    }

    //*-*-*-*//

    private void AddCity(int i, int j) // da chiamare solo dentro AddRoads()
    {
        if(Random.Range(0.0f, 100.0f) <= cities_Density && world_Matrix[i, j].tile_Type != Enum.TileType.City)
        {
            int city_MaxExtensionX;
            int city_MaxExtensionY;

            int city_MinExtensionX;
            int city_MinExtensionY;

            switch(cities_Size)
            {
                case Enum.Size.Small:   city_MinExtensionX = (int)((float)(world_X / 100) * 0) + 5; 
                                        city_MinExtensionY = (int)((float)(world_Y / 100) * 0) + 5;

                                        city_MaxExtensionX = (int)((float)(world_X / 100) * 5) + 5; 
                                        city_MaxExtensionY = (int)((float)(world_Y / 100) * 5) + 5;
                                        break;

                case Enum.Size.Medium:  city_MinExtensionX = (int)((float)(world_X / 100) * 5) + 5; 
                                        city_MinExtensionY = (int)((float)(world_Y / 100) * 5) + 5;

                                        city_MaxExtensionX = (int)((float)(world_X / 100) * 10) + 5; 
                                        city_MaxExtensionY = (int)((float)(world_Y / 100) * 10) + 5;
                                        break;

                case Enum.Size.Big:     city_MinExtensionX = (int)((float)(world_X / 100) * 10) + 5; 
                                        city_MinExtensionY = (int)((float)(world_Y / 100) * 10) + 5;

                                        city_MaxExtensionX = (int)((float)(world_X / 100) * 20) + 5; 
                                        city_MaxExtensionY = (int)((float)(world_Y / 100) * 20) + 5;
                                        break;

                default:                city_MinExtensionX = 0; 
                                        city_MinExtensionY = 0;

                                        city_MaxExtensionX = 0; 
                                        city_MaxExtensionY = 0;
                                        break;                        
            }

            //----//

            int x_Delta = Random.Range(city_MinExtensionX, city_MaxExtensionX + 1);

            int x_Min = i - (int)(x_Delta / 2);

            if(x_Min < 0)
            {
                x_Min = 0;
            }

            int x_Max = x_Min + x_Delta;

            if(x_Max >= world_X)
            {
                x_Max = world_X - 1;
                x_Min = x_Max - x_Delta;
            }
                    
            //----//

            int y_Delta = Random.Range(city_MinExtensionY, city_MaxExtensionY + 1);

            int y_Min = j - (int)(y_Delta / 2);

            if(y_Min < 0)
            {
                y_Min = 0;
            }

            int y_Max = y_Min + y_Delta;

            if(y_Max >= world_Y)
            {
                y_Max = world_Y - 1;
                y_Min = y_Max - y_Delta;
            }

            //----//

            for(int x = x_Min + 1; x < x_Max; x++)
            {
                for(int y = y_Min + 1; y < y_Max; y++)
                {
                    if(world_Matrix[x, y].tile_Type != Enum.TileType.River)
                    {
                        world_Matrix[x, y].tile_Type = Enum.TileType.City; 
                    }
                }
            }

            //----//

            for(int x = x_Min; x <= x_Max; x++)
            {
                world_Matrix[x, y_Min].tile_Type = Enum.TileType.Road; 
                world_Matrix[x, y_Max].tile_Type = Enum.TileType.Road; 
            }

            for(int y = y_Min ; y <= y_Max; y++)
            {
                world_Matrix[x_Min, y].tile_Type = Enum.TileType.Road; 
                world_Matrix[x_Max, y].tile_Type = Enum.TileType.Road;
            }

            //----//

            int x_Med = x_Min + (int)(x_Delta/ 2);
            int y_Med = y_Min + (int)(y_Delta/ 2);

            //----//

            if(Random.Range(0.0f, 100.0f) <= 150.0f)
            {
                for(int k = x_Min + 1; k < x_Max; k++)
                {
                    world_Matrix[k, y_Med].tile_Type = Enum.TileType.Road; 
                }
            }

            if(Random.Range(0.0f, 100.0f) <= 150.0f)
            {
                for(int k = y_Min + 1; k < y_Max; k++)
                {
                    world_Matrix[x_Med, k].tile_Type = Enum.TileType.Road; 
                }
            }

            //----//

        }
    }

    //*-*-*-*//

    private void AddRoads()
    {
        Enum.Cardinal road_PrevStartingSide = Enum.Cardinal.None;

        for(int k = 0; k < roads_Number; k++)
        {
            NewRoad:

            Enum.Cardinal road_StartingSide = (Enum.Cardinal) (Random.Range(0, 4) * 2);
            if(road_PrevStartingSide == road_StartingSide)
            {
                goto NewRoad;
            }
            road_PrevStartingSide = road_StartingSide;


            Vector2 road_StartPosition;
            List<Enum.Cardinal> directions_Allowed = new List<Enum.Cardinal>();
            Enum.Cardinal road_Direction;

            switch(road_StartingSide)
            {
                case Enum.Cardinal.East:    road_StartPosition = new Vector2(Random.Range(0, world_X), world_Y - 1);

                                            directions_Allowed.Add(Enum.Cardinal.NorthWest);
                                            directions_Allowed.Add(Enum.Cardinal.SouthWest);
                                            directions_Allowed.Add(Enum.Cardinal.West);

                                            NewDirectionFromEast:

                                            road_Direction = (Enum.Cardinal) (Random.Range(0, 9));

                                            if(directions_Allowed.Contains(road_Direction))
                                            {
                                                int road_SegmentLength = Random.Range(road_MinSegmentLength, road_MaxSegmentLength);

                                                switch(road_Direction)
                                                {
                                                    case Enum.Cardinal.NorthWest:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {            
                                                                                        if(road_StartPosition.x - h >= 0 && road_StartPosition.y - h >= 0)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y - h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y - h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x - h, (int) road_StartPosition.y - h);
                                                                                            }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromEast;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x - (road_SegmentLength - 1), road_StartPosition.y - (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromEast;

                                                                                                        
                                                    case Enum.Cardinal.SouthWest:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x + h < world_X && road_StartPosition.y - h >= 0)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y - h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y - h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x + h, (int) road_StartPosition.y - h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromEast;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x + (road_SegmentLength - 1), road_StartPosition.y - (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromEast;

                                                    case Enum.Cardinal.West:         for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.y - h >= 0)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x, (int) road_StartPosition.y - h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x, (int) road_StartPosition.y - h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x, (int) road_StartPosition.y - h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromEast;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x, road_StartPosition.y - (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromEast;
                                                }
                                            }
                                            else
                                            {
                                                goto NewDirectionFromEast;
                                            }

                                            EndRoadFromEast:
                                            break;

                //----//

                case Enum.Cardinal.South:   road_StartPosition = new Vector2(world_X - 1, Random.Range(0, world_Y));

                                            directions_Allowed.Add(Enum.Cardinal.NorthWest);
                                            directions_Allowed.Add(Enum.Cardinal.NorthEast);
                                            directions_Allowed.Add(Enum.Cardinal.North);

                                            NewDirectionFromSouth:

                                            road_Direction = (Enum.Cardinal) (Random.Range(0, 9));

                                            if(directions_Allowed.Contains(road_Direction))
                                            {
                                                int road_SegmentLength = Random.Range(road_MinSegmentLength, road_MaxSegmentLength);

                                                switch(road_Direction)
                                                {
                                                    case Enum.Cardinal.NorthWest:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x - h >= 0 && road_StartPosition.y - h >= 0)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y - h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y - h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x - h, (int) road_StartPosition.y - h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromSouth;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x - (road_SegmentLength - 1), road_StartPosition.y - (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromSouth;

                                                                                                
                                                    case Enum.Cardinal.NorthEast:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x - h >= 0 && road_StartPosition.y + h < world_Y)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y + h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y + h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x - h, (int) road_StartPosition.y + h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromSouth;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x - (road_SegmentLength - 1), road_StartPosition.y + (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromSouth;

                                                    case Enum.Cardinal.North:       for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x - h >= 0)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x - h, (int) road_StartPosition.y);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromSouth;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x - (road_SegmentLength - 1), road_StartPosition.y);
                                                                                    goto NewDirectionFromSouth;
                                                }
                                            }
                                            else
                                            {
                                                goto NewDirectionFromSouth;
                                            }

                                            EndRoadFromSouth:
                                            break;

                //----//

                case Enum.Cardinal.West:    road_StartPosition = new Vector2(Random.Range(0, world_X), 0);

                                            directions_Allowed.Add(Enum.Cardinal.NorthEast);
                                            directions_Allowed.Add(Enum.Cardinal.SouthEast);
                                            directions_Allowed.Add(Enum.Cardinal.East);

                                            NewDirectionFromWest:

                                            road_Direction = (Enum.Cardinal) (Random.Range(0, 9));

                                            if(directions_Allowed.Contains(road_Direction))
                                            {
                                                int road_SegmentLength = Random.Range(road_MinSegmentLength, road_MaxSegmentLength);

                                                switch(road_Direction)
                                                {
                                                    case Enum.Cardinal.NorthEast:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x - h >= 0 && road_StartPosition.y + h < world_Y)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y + h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x - h, (int) road_StartPosition.y + h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x - h, (int) road_StartPosition.y + h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromWest;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x - (road_SegmentLength - 1), road_StartPosition.y + (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromWest;

                                                                                                
                                                    case Enum.Cardinal.SouthEast:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x + h < world_X && road_StartPosition.y + h < world_Y)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y + h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y + h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x + h, (int) road_StartPosition.y + h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromWest;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x + (road_SegmentLength - 1), road_StartPosition.y + (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromWest;

                                                    case Enum.Cardinal.East:        for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.y + h < world_Y)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x, (int) road_StartPosition.y + h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x, (int) road_StartPosition.y + h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x, (int) road_StartPosition.y + h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromWest;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x, road_StartPosition.y + (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromWest;
                                                }
                                            }
                                            else
                                            {
                                                goto NewDirectionFromWest;
                                            }

                                            EndRoadFromWest:
                                            break;

                //----//

                case Enum.Cardinal.North:   road_StartPosition = new Vector2(0, Random.Range(0, world_Y));

                                            directions_Allowed.Add(Enum.Cardinal.SouthWest);
                                            directions_Allowed.Add(Enum.Cardinal.SouthEast);
                                            directions_Allowed.Add(Enum.Cardinal.South);

                                            NewDirectionFromNorth:

                                            road_Direction = (Enum.Cardinal) (Random.Range(0, 9));

                                            if(directions_Allowed.Contains(road_Direction))
                                            {
                                                int road_SegmentLength = Random.Range(road_MinSegmentLength, road_MaxSegmentLength);

                                                switch(road_Direction)
                                                {
                                                    case Enum.Cardinal.SouthWest:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x + h < world_X && road_StartPosition.y - h >= 0)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y - h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y - h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x + h, (int) road_StartPosition.y - h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromNorth;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x + (road_SegmentLength - 1), road_StartPosition.y - (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromNorth;

                                                                                                
                                                    case Enum.Cardinal.SouthEast:   for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x + h < world_X && road_StartPosition.y + h < world_Y)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y + h].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y + h].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x + h, (int) road_StartPosition.y + h);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromNorth;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x + (road_SegmentLength - 1), road_StartPosition.y + (road_SegmentLength - 1));
                                                                                    goto NewDirectionFromNorth;

                                                    case Enum.Cardinal.South:       for(int h = 0; h < road_SegmentLength; h++)
                                                                                    {
                                                                                        if(road_StartPosition.x + h < world_X)
                                                                                        {
                                                                                            if(world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y].tile_Type != Enum.TileType.City)
                                                                                            {
                                                                                                world_Matrix[(int) road_StartPosition.x + h, (int) road_StartPosition.y].tile_Type = Enum.TileType.Road;

                                                                                                AddCity((int) road_StartPosition.x + h, (int) road_StartPosition.y);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            goto EndRoadFromNorth;
                                                                                        }
                                                                                    }

                                                                                    road_StartPosition = new Vector2(road_StartPosition.x + (road_SegmentLength - 1), road_StartPosition.y);
                                                                                    goto NewDirectionFromNorth;
                                                }
                                            }
                                            else
                                            {
                                                goto NewDirectionFromNorth;
                                            }

                                            EndRoadFromNorth:
                                            break;
            }
        }

        //----//

        for(int i = 2; i < world_X - 2; i++)
        {
            for(int j = 2; j < world_Y - 2; j++)
            {
                if(world_Matrix[i, j].tile_Type == Enum.TileType.Road)
                {

                    if(world_Matrix[i + 1, j].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 30.0f)
                    {
                        world_Matrix[i + 1, j].tile_Type = Enum.TileType.Plain;

                        if(world_Matrix[i + 2, j].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 15.0f)
                        {
                            world_Matrix[i + 2, j].tile_Type = Enum.TileType.Plain;
                        }      
                    }

                    if(world_Matrix[i - 1, j].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 30.0f)
                    {
                        world_Matrix[i - 1, j].tile_Type = Enum.TileType.Plain;

                        if(world_Matrix[i - 2, j].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 15.0f)
                        {
                            world_Matrix[i - 2, j].tile_Type = Enum.TileType.Plain;
                        }      
                    }

                    //----//

                    if(world_Matrix[i, j + 1].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 30.0f)
                    {
                        world_Matrix[i, j + 1].tile_Type = Enum.TileType.Plain;

                        if(world_Matrix[i, j + 2].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 15.0f)
                        {
                            world_Matrix[i, j + 2].tile_Type = Enum.TileType.Plain;
                        }     
                    }

                    if(world_Matrix[i, j - 1].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 50.0f)
                    {
                        world_Matrix[i, j - 1].tile_Type = Enum.TileType.Plain;

                        if(world_Matrix[i, j - 2].tile_Type == Enum.TileType.Lake && Random.Range(0.0f, 100.0f) <= 30.0f)
                        {
                            world_Matrix[i, j - 2].tile_Type = Enum.TileType.Plain;
                        }     
                    }
                }
            }
        }
    }

    //*-*-*-*//

    public void DrawLevel()
    {
        GameObject level_Parent = new GameObject("Level");

        GameObject map_Parent = new GameObject("Map");
        map_Parent.transform.parent = level_Parent.transform;

        GameObject plain_GameObject = Instantiate(plain_Plane, new Vector3((world_X * tile_Plain.transform.localScale.x / 2) - (tile_Plain.transform.localScale.x / 2), (world_Y * tile_Plain.transform.localScale.z / 2) - (tile_Plain.transform.localScale.z / 2), 0.1f), Quaternion.identity * Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        plain_GameObject.name = "Plain";
        plain_GameObject.transform.localScale = new Vector3((world_X + 0.5f) * tile_Plain.transform.localScale.x / 10, 1.0f, (world_Y + 0.5f) * tile_Plain.transform.localScale.z / 10);
        plain_GameObject.transform.parent = map_Parent.transform;

        foreach(GameCore.Tile tile_Level in world_Matrix)
        {
            switch(tile_Level.tile_Type)
            {
                /*case Enum.TileType.Plain:     Instantiate(tile_Plain, new Vector3(tile_Level.tile_X * tile_Plain.transform.localScale.x, tile_Level.tile_Y * tile_Plain.transform.localScale.z), Quaternion.identity, level_Parent.transform);
                                                break;*/

                case Enum.TileType.Mountain:    Instantiate(tile_Mountain, new Vector3(tile_Level.tile_X * tile_Mountain.transform.localScale.x, tile_Level.tile_Y * tile_Mountain.transform.localScale.z), Quaternion.identity, map_Parent.transform);
                                                break;
                
                case Enum.TileType.Forest:      Instantiate(tile_Forest, new Vector3(tile_Level.tile_X * tile_Forest.transform.localScale.x, tile_Level.tile_Y * tile_Forest.transform.localScale.z), Quaternion.identity, map_Parent.transform);
                                                break;

                case Enum.TileType.Road:        Instantiate(tile_Road, new Vector3(tile_Level.tile_X * tile_Road.transform.localScale.x, tile_Level.tile_Y * tile_Road.transform.localScale.z), Quaternion.identity, map_Parent.transform);
                                                break;

                case Enum.TileType.City:        Instantiate(tile_City, new Vector3(tile_Level.tile_X * tile_City.transform.localScale.x, tile_Level.tile_Y * tile_City.transform.localScale.z), Quaternion.identity, map_Parent.transform);
                                                break;

                case Enum.TileType.Lake:        Instantiate(tile_Lake, new Vector3(tile_Level.tile_X * tile_Lake.transform.localScale.x, tile_Level.tile_Y * tile_Lake.transform.localScale.z), Quaternion.identity, map_Parent.transform);
                                                break;

                case Enum.TileType.River:       Instantiate(tile_River, new Vector3(tile_Level.tile_X * tile_River.transform.localScale.x, tile_Level.tile_Y * tile_River.transform.localScale.z), Quaternion.identity, map_Parent.transform);
                                                break;
            }
        }
    }

    //*-*-*-*//

    public void DeleteLevel()
    {
        DestroyImmediate(GameObject.Find("Level"));
    }

    //*-*-*-*//

    public class Tile
    {
        public int tile_X;
        public int tile_Y;
        //private bool tile_IsFree = true;
        public Enum.TileType tile_Type = Enum.TileType.Plain;

        public Tile(int coordinate_X, int coordinate_Y) // costruttore
        {
            tile_X = coordinate_X;
            tile_Y = coordinate_Y;
        }

    }
}
