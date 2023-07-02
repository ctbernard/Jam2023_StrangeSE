using System;
using Godot;

public class Game : Node2D {
  /* TODO
   * build character scene
   * build hole (colliable)
   * build base block
   * build sample block
   * build immovable block
   * Flesh out system for block placing
   * Flesh out system for moving blocks
   * Implement room scene and room generation
   * Might be best to abstractify blocks to easily implement base mechanics
   * Check GoDot inheritance to see how easy scene inheritance can be?
   */

  /* TODO notes
   * Scenes setup and use:
   * Room?>>Background+TileMap+
   * Character>>
   * Block>>Block Sprite+Surrounding Effects+
   *
   *
   */

  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  private Vector2 startTile;
  private Vector2 endTile = new Vector2(200, 200);
  private Vector2 charPosMapped;
  private KinematicBody2D character;
  private Floor tiles;
  private Vector2 roomDimensions; // private var roomLength; private var roomHeight;
  // private Character user = new();

  //Preloads go here
  // TODO: preload all blocks and Room assets (load 1 room at a time, seperate room scenes? -sounds too verbose)


  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    character = GetNode<KinematicBody2D>("Character");
    // Character character = GetNode<Character>("Character.tscn"); //pull node as scene?
    tiles = GetNode<Floor>("Floor");
    charPosMapped = tiles.WorldToMap(character.Position);

    GD.Print("tile: " + tiles.GetCellv(charPosMapped) + " Character Position: " + character.Position);
  }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    // checkEnd(tiles, character);
  }

  public override void _PhysicsProcess(float delta) {
    int moveMultiplier = 1;
    KinematicCollision2D collide;
#if DEBUG
    moveMultiplier = 3;
#endif
    //Movement > Physics Checks >
    //simple temporary wasd controls, can we move this into character? - would have to make a KinematicBody
    if (Input.IsActionPressed("ui_right") || Input.IsPhysicalKeyPressed((int)KeyList.D)) {
      GD.Print("Character Position: " + character.Position);
      character.MoveAndCollide(Vector2.Right * moveMultiplier);
    }

    if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed((int)KeyList.A)) {
      GD.Print("Character Position: " + character.Position);
      character.MoveAndCollide(Vector2.Left * moveMultiplier);
    }

    if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed((int)KeyList.W)) {
      GD.Print("Character Position: " + character.Position);
      character.MoveAndCollide(Vector2.Up * moveMultiplier);
    }

    if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed((int)KeyList.S)) {
      GD.Print("Character Position: " + character.Position);
      character.MoveAndCollide(Vector2.Down * moveMultiplier);
    }

    if (tiles.spriteOnTile(character, endTile)) {
      onRoomSuccess();
    }
  }

  public void createBlock() {

  }

  public void onRoomSuccess() {
    /* Successfully finished the room
     * TODO Go through next door> Close current room scene> To black> Start to open next room scene
     */

  }

  /********************************************************************************************************************/


/* Notes for Game object
 Load world in:
  [Room to room walk through]
  Same room each time?
  Each world predetermined?

  Load floor [Grid]

  Load background
    same background each time?

  Load blocks
    Block can be set in godot if predetermined or

  Load character


 */
}
