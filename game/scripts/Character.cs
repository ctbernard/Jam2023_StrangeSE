using System;
using Godot;



public class Character : KinematicBody2D {
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";
  // private Vector2 startingPosition;
  private Floor tiles;




  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    tiles = GetNode<Floor>("../Floor");


  }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {


  }

  public override void _Input(InputEvent @event) {
    //Other button presses
    // if (@event is InputEventKey keyEvent && keyEvent.Pressed) {
    //   if (keyEvent.) {
    //
    //   }
    // }
  }

  public override void _PhysicsProcess(float delta) {
    checkInteraction("fire_spread");
  }

  public bool activationAvailable(TileMap tiles) {


    return false;
  }

  public void tileInteractions() {
    // if(Position TileMap.Get)
  }

  public void playerDied() {

  }

  public void bump() {

  }

  public void checkInteraction(String tile) {
    //Player - Block interactions

    if (tiles.TileSet.TileGetName(tiles.GetCellv(tiles.WorldToMap(Position))).Equals(tile)) {
      switch (tile) {
        case "fire_spread":
          GD.Print("You are burning!!");
          break;
      }
    }
  }
}

/********************************************************************************************************************/

/*

 */
