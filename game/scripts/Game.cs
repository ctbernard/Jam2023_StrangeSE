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

  [Export] public SelectionMode Mode = SelectionMode.Player;
  [Export] public NodePath PlayerNode;
  [Export] public NodePath SelectorNode;
  [Export] public NodePath ObjectNode;

  private Vector2 startTile;
  private Vector2 endTile = new Vector2(200, 200);
  private Vector2 charPosMapped;
  private KinematicBody2D character;
  private Floor tiles;
  private Vector2 roomDimensions; // private var roomLength; private var roomHeight;
  private NodePath hoverObjectPath;

  // private Character user = new();

  //Preloads go here
  // TODO: preload all blocks and Room assets (load 1 room at a time, seperate room scenes? -sounds too verbose)

  PackedScene normBlockScene = ResourceLoader.Load("res://game/scenes/block_scenes/NormBlock.tscn") as PackedScene;
  PackedScene boxBlockScene = ResourceLoader.Load("res://game/scenes/block_scenes/BoxBlock.tscn") as PackedScene;
  PackedScene waterBlockScene = ResourceLoader.Load("res://game/scenes/block_scenes/WaterBlock.tscn") as PackedScene;
  PackedScene fireBlockScene = ResourceLoader.Load("res://game/scenes/block_scenes/FireBlock.tscn") as PackedScene;
  PackedScene growthBlockScene = ResourceLoader.Load("res://game/scenes/block_scenes/GrowthBlock.tscn") as PackedScene;


  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    GD.Print(PlayerNode);
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
    // if (Input.IsActionPressed("ui_right") || Input.IsPhysicalKeyPressed((int)KeyList.D)) {
    //   GD.Print("Character Position: " + character.Position);
    //   character.MoveAndCollide(Vector2.Right * moveMultiplier);
    // }
    //
    // if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed((int)KeyList.A)) {
    //   GD.Print("Character Position: " + character.Position);
    //   character.MoveAndCollide(Vector2.Left * moveMultiplier);
    // }
    //
    // if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed((int)KeyList.W)) {
    //   GD.Print("Character Position: " + character.Position);
    //   character.MoveAndCollide(Vector2.Up * moveMultiplier);
    // }
    //
    // if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed((int)KeyList.S)) {
    //   GD.Print("Character Position: " + character.Position);
    //   character.MoveAndCollide(Vector2.Down * moveMultiplier);
    // }
    //
    if (tiles.spriteOnTile(character, endTile)) {
      onRoomSuccess();
    }
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventKey eventKey) {
      if (eventKey.Pressed) {
        string KeyPressed = OS.GetScancodeString(eventKey.Scancode);
        if (KeyPressed == "Right" || KeyPressed == "Left" || KeyPressed == "Up" || KeyPressed == "Down") {
          ActionMovement(KeyPressed);
        }
        else if (KeyPressed == "Z") {
          ActionChangeMode(this.Mode);
        }
        else if (KeyPressed == "X" && Mode == SelectionMode.Selector) {
          ActionSetObjectNode();
        }
      }
    }
  }

  public void ActionMovement(string KeyPressed) {
    NodePath SelectedNodePath = "";

    switch (Mode) {
      case SelectionMode.Player:
        SelectedNodePath = this.PlayerNode;
        break;
      case SelectionMode.Selector:
        SelectedNodePath = this.SelectorNode;
        break;
      case SelectionMode.Object:
        SelectedNodePath = this.ObjectNode;
        break;
    }

    Node2D SelectedEntity = GetNode<Node2D>(SelectedNodePath);
    float[] EntityPostion = new float[] { SelectedEntity.Position.x, SelectedEntity.Position.y };

    if (KeyPressed == "Right")
      // SelectedEntity.MoveAndCollide(Vector2.Right * 32);
    EntityPostion[0] += 32;
    if (KeyPressed == "Left")
      // SelectedEntity.MoveAndCollide(Vector2.Left * 32);
    EntityPostion[0] -= 32;
    if (KeyPressed == "Up")
      // SelectedEntity.MoveAndCollide(Vector2.Up * 32);
    EntityPostion[1] -= 32;
    if (KeyPressed == "Down")
      // SelectedEntity.MoveAndCollide(Vector2.Down * 32);
    EntityPostion[1] += 32;

    // SelectedEntity.Position =
    //   new Vector2((int)Math.Round(SelectedEntity.Position.x), (int)Math.Round(SelectedEntity.Position.y));
    GD.Print("Character Position: " + character.Position);

    SelectedEntity.Position = new Vector2(EntityPostion[0], EntityPostion[1]);
  }

  public void ActionChangeMode(SelectionMode mode) {
    switch (Mode) {
      case SelectionMode.Player:
        Mode = SelectionMode.Selector;
        break;
      case SelectionMode.Selector:
        Mode = SelectionMode.Player;
        break;
      case SelectionMode.Object:
        Mode = SelectionMode.Selector;
        break;
    }
  }

  public void ActionSetObjectNode() {
    // hoverObjectPath = GetNode<Selector>(this.SelectorNode).HoverObject;
    // NodePath hoverObjectPath = GetNode<Selector>(this.SelectorNode).HoverObject;
    if (hoverObjectPath != null) {
      this.ObjectNode = hoverObjectPath;
      Mode = SelectionMode.Object;
      GD.Print("Selected");
    }
    else {
      GD.Print("Nothing Found");
    }
  }

  public void createBlock(Vector2 position, int blockID) {
    switch (blockID) {
      case 0:
        Node2D newNormalBlock = normBlockScene.Instance() as Node2D;
        AddChildBelowNode(tiles, newNormalBlock);
        newNormalBlock.Position = position;
        GD.Print("New normal block at " + position);
        hoverObjectPath = new NodePath(newNormalBlock.GetPath());
        ActionSetObjectNode();
        break;
      case 1:
        Node2D newBoxBlock = boxBlockScene.Instance() as Node2D;
        AddChildBelowNode(tiles, newBoxBlock);
        newBoxBlock.Position = position;
        GD.Print("New box block at " + position);
        hoverObjectPath = new NodePath(newBoxBlock.GetPath());
        ActionSetObjectNode();
        break;
      case 2:
        Node2D newWaterBlock = waterBlockScene.Instance() as Node2D;
        AddChildBelowNode(tiles, newWaterBlock);
        newWaterBlock.Position = position;
        GD.Print("New water block at " + position);
        break;
      case 3:
        Node2D newFireBlock = fireBlockScene.Instance() as Node2D;
        AddChildBelowNode(tiles, newFireBlock);
        newFireBlock.Position = position;
        GD.Print("New fire block at " + position);

        break;

      case 4:
        Node2D newGrowthBlock = growthBlockScene.Instance() as Node2D;
        AddChildBelowNode(tiles, newGrowthBlock);
        newGrowthBlock.Position = position;
        GD.Print("New growth block at " + position);

        break;


        GD.Print("New fire block at " + position);

        break;
      default:
        break;
    }
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
