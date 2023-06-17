using System;
using Godot;

// TODO replace Sprite with KinematicBody2D
public class Character : Sprite { //: KinematicBody2D {
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";
  // private Vector2 startingPosition;



  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
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

  public bool activationAvailable(TileMap tiles) {

    return false;
  }
}

/********************************************************************************************************************/

/*

 */
