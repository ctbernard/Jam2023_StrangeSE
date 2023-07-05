using Godot;
using System;
using Godot.Collections;
using Array = System.Array;

public struct tilePoolEntry {
  public SourceBlock tile;
  public Vector2 coor;
  public int tileSetTile;
  public int currentBlock;

  public override string ToString() =>
    "Coordinates: " + coor + " Tile type: " + tileSetTile; // + " Tile type name: " + tile;
}

public class Floor : TileMap {
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";
  private Node2D game;


  private Array<Vector2> activationLocations = new Array<Vector2>();

  // private ResourcePreloader resourcePreloader = new ResourcePreloader();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {

    game = GetNode<Node2D>(GetParent()._ImportPath);

    foreach (Vector2 location in activationLocations) {
      placeActivationTile(location);
    }

    tilePoolEntry[] fullTiles = MapTilePool(true);

    //testing outside of physics
    foreach (tilePoolEntry entry in fullTiles) {
      // GD.PrintRaw(entry);
      if (entry.tile != null) {
        entry.tile.spread();
      }
    }

    // GD.Print(fullTiles[0].coor + ", " + fullTiles[0].tileSetTile);

    // spread();

    poop(2);
    GD.Print("tileset fire source: " + TileSet.FindTileByName("fire_source"));
  }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }

  private void placeActivationTile(Vector2 vec) {
    // TODO change from -1 to global variable of activation tile number
    // TODO see if same circle/shape2d can be used for seperate Area2D
    SetCellv(vec, -1);
    CircleShape2D circle = new CircleShape2D();
    circle.Radius = 10;
    CollisionShape2D shape2d = new CollisionShape2D();
    shape2d.Shape = circle;
    Area2D activationRange = new Area2D();
    activationRange.AddChild(shape2d);
    activationRange.Position = vec;
#if DEBUG

#endif
  }

  private void activationAvailable() {
    //   if () {
    //
    //   }
  }

  public bool spriteOnTile(Node2D sprite, Vector2 vec) {
    /*
     *
     */

    if (WorldToMap(sprite.Position) == WorldToMap(vec)) {
      // GD.Print(sprite.Name + " and " + vec.ToString() + "have collided");
      return true;
    }

    return false;
  }

  // public Array<tilePoolEntry> getFullMapTiles() {
  public tilePoolEntry[] MapTilePool(bool startUp) {
    //TODO change to checkTileEntry method and split loop into the class
    //Pulls all map tiles for a single room to help recreate rooms
    Array<Vector2> usedCells = new Array<Vector2>(GetUsedCells());
    tilePoolEntry[] roomTilePool = new tilePoolEntry[usedCells.Count];

    // object[] array = new object[usedCells.Count];
    for (int i = 0; i < usedCells.Count; i++) {
      tilePoolEntry entry = new tilePoolEntry();
      entry.coor = usedCells[i];
      entry.tileSetTile = GetCellv(usedCells[i]);
      if (startUp) {
        switch (TileSet.TileGetName(entry.tileSetTile)) {
          case "norm_block":
            entry.tile = new NormBlock(this, entry.coor);
            createBlock(MapToWorld(entry.coor), 0);
            GD.Print("Normal block at " + entry.coor + " successfully attached to tile pool");
            break;
          case "box_block":
            entry.tile = new BoxBlock(this, entry.coor);
            createBlock(MapToWorld(entry.coor), 1);
            GD.Print("Box block at " + entry.coor + " successfully attached to tile pool");
            break;
          case "water_source":
            entry.tile = new WaterBlock(this, entry.coor, true);
            createBlock(MapToWorld(entry.coor), 2);
            GD.Print("Water block at " + entry.coor + " successfully attached to tile pool");
            break;
          case "fire_source":
            entry.tile = new FireBlock(this, entry.coor);
            createBlock(MapToWorld(entry.coor), 3);
            GD.Print("Fire block at " + entry.coor + " successfully attached to tile pool");
            break;
          case "growth_source":
            entry.tile = new GrowthBlock(this, entry.coor);
            createBlock(MapToWorld(entry.coor), 4);
            GD.Print("Growth block at " + entry.coor + " successfully attached to tile pool");
            break;
          default:
            // GD.PrintRaw("Block at " + entry.coor + " was not properly attached to tile pool: ");
            // GD.Print(TileSet.TileGetName(entry.tileSetTile));
            break;
        }
      }

      roomTilePool[i] = entry;
    }

    return roomTilePool;
  }

  private void createBlock(Vector2 position, int blockID) {
    GetParent().CallDeferred("createBlock", position, blockID);
    SetCellv(WorldToMap( position ), TileSet.FindTileByName("floor"));
  }
  public void getFullMapBlocks() {
    //Pulls all map blocks for a single room to help recreate rooms
  }

  /**** Tile Management and Functionality ****/

  public void poop(int id) {
    Godot.Collections.Array tilesArray = GetUsedCellsById(id);
    foreach (Vector2 tile in tilesArray) {
      // tile.
      // GD.Print("Another tile: " + tile);
    }
  }
}

public abstract class Tile {
  protected TileMap tileMap;
  protected TileSet tileSet;
  protected Vector2 cell;

  protected Tile(TileMap tileMap, Vector2 cell) {
    this.tileMap = tileMap;
    this.cell = cell;
    tileSet = tileMap.TileSet;
  }
  // this.Array
  // public Vector2 toRight() {
  //
  // }

  public void lineSetTiles(Vector2 start, Vector2 end) {
    Vector2 diff = end - start;
  }

  // public void areaSetTiles() {
  //
  // }
}

public class SourceBlock : Tile {
  // protected virtual Vector2[] changeMatrix;

  //TODO
  public SourceBlock(TileMap tileMap, Vector2 cell) : base(tileMap, cell) {
  }
  // public String checkTile() {
    //Block - Block interactions
    // return tileMap.TileSet.TileGetName(tileMap.GetCellv(cell));
    // if () {
    //   switch (tile) {
    //     case "fire_spread":
    //       GD.Print("You are burning!!");
    //       break;
    //   }
    // }
  // }
  public bool[] checkSpreadObstruct(Vector2[] spreadVectors) {
     bool[] spreadCheck = new bool[spreadVectors.Length];
    string[] obstructions = { "wall", "box", "fire_source", "water_source" };
    for (int i = 0; i < spreadCheck.Length; i++) {
      foreach (string obstruct in obstructions) {
        spreadCheck[i] = tileMap.TileSet.FindTileByName(obstruct) == tileMap.GetCellv(spreadVectors[i] + cell);
        // GD.Print("Spread check for "  + i + " is false. Tile type: " + tileMap.GetCellv(spreadVectors[i]));

        if (spreadCheck[i]) {
          // GD.Print("Spread check for "  + i + " is true. Tile type: " + tileMap.GetCellv(spreadVectors[i]));
          break;
        }

      }
    }

    return spreadCheck;
  }


  public virtual void spread() {
    // bool[] checkedObstructs = checkSpreadObstruct(changeMatrix);
  }

  //**Unused vectors
  private Vector2[] cardinal = { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right };

  private bool checkTileFree(Vector2 target) {
    if (tileMap.GetCellv(target) < 1) {
      return true;
    }

    return false;
  }

  private bool[] checkCardinally(Vector2 target) {
    bool[] cardinalChecks = new bool[4];
    for (int i = 0; i < 4; i++) {
      cardinalChecks[i] = checkTileFree(target + cardinal[i]);
    }

    return cardinalChecks;
  }
}

public class NormBlock : SourceBlock {
  public NormBlock(TileMap tileMap, Vector2 cell) : base(tileMap, cell)
  {
  }

  public override void spread() {
    // Vector2 spreadingTile = cell;
    // Vector2[] changeMatrix;

  }
}

public class BoxBlock : SourceBlock {
  public BoxBlock(TileMap tileMap, Vector2 cell) : base(tileMap, cell)
  {
  }

  public override void spread() {
    // Vector2 spreadingTile = cell;
    // Vector2[] changeMatrix;

  }
}



public class WaterBlock : SourceBlock {
  //Determining orientation of block (either left-right or up-down)
  private bool horizontal;

  public WaterBlock(TileMap tileMap, Vector2 cell, bool horizontal) : base(tileMap, cell) =>
    this.horizontal = horizontal;

  public override void spread() {
    //Recursive spread implementation
    Vector2 spreadingTile = cell;
    Vector2[] changeMatrix = { Vector2.Up, Vector2.Down };
    if (horizontal) {
      //Spread left-right
      changeMatrix[0] = Vector2.Left;
      changeMatrix[1] = Vector2.Right;
      tileMap.SetCellv((changeMatrix[0] + spreadingTile), tileSet.FindTileByName("water_left_spread"));
      tileMap.SetCellv((changeMatrix[1] + spreadingTile), tileSet.FindTileByName("water_right_spread"));
    }
    else {
      tileMap.SetCellv((changeMatrix[0] + spreadingTile), tileSet.FindTileByName("water_up_spread"));
      tileMap.SetCellv((changeMatrix[1] + spreadingTile), tileSet.FindTileByName("water_down_spread"));
    }


    // foreach (Vector2 vec in changeMatrix) {
    //   //TODO guard statements for blocking spread and interactions
    //   tileMap.SetCellv((vec + spreadingTile), tileSet.FindTileByName("water_up_spread"));
    //   //TODO check around (same orientation) new tiles and if no obstructions call spread for new tiles
    // }
  }
}

public class FireBlock : SourceBlock {
  private int range = 2;
  Vector2[] changeMatrix = {
      Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right, Vector2.Up + Vector2.Left, Vector2.Up + Vector2.Right,
      Vector2.Down + Vector2.Right, Vector2.Down + Vector2.Left, Vector2.Up * 2, Vector2.Down * 2, Vector2.Right * 2,
      Vector2.Left * 2
    };

  public FireBlock(TileMap tileMap, Vector2 cell) : base(tileMap, cell)
  {
  }

  public override void spread() {
    //Diamond spread implementation
    //TODO move outside of spread
    Vector2[] spreadBlocks = new Vector2[12];
    bool[] spreadObstruct = checkSpreadObstruct(changeMatrix);
    foreach (bool b in spreadObstruct) {
      // GD.Print(b);
    }
    // foreach (Vector2 vec in changeMatrix) {
    for (int i = 0; i < changeMatrix.Length; i++) {
      if (!spreadObstruct[i]) {
        // GD.Print("burning cell: " + (changeMatrix[i] + cell));
        tileMap.SetCellv((changeMatrix[i] + cell), tileSet.FindTileByName("fire_spread"));

      }

    }
    }
  }

public class GrowthBlock : SourceBlock {
  public GrowthBlock(TileMap tileMap, Vector2 cell) : base(tileMap, cell)
  {
  }

  public override void spread() {
    //Recursive spread implementation
    // Vector2 spreadingTile = cell;
    // Vector2[] changeMatrix = { Vector2.Up, Vector2.Down };
  }
}


// public void burnBlock() {
//   // if (range)
// }



/********************************************************************************************************************/

/*

 */
