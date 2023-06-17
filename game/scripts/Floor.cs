using Godot;
using System;
using Godot.Collections;

public class Floor : TileMap {
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  private Array<Vector2> activationLocations = new Array<Vector2>();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    activationLocations.Add(new Vector2(0, 0));
    foreach (Vector2 location in activationLocations) {
      placeActivationTile(location);
    }
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

  public bool spriteOnTile(Sprite sprite, Vector2 vec) {
    /*
     *
     */

    if (WorldToMap(sprite.Position) == WorldToMap(vec)) {
      // GD.Print(sprite.Name + " and " + vec.ToString() + "have collided");
      return true;
    }

    return false;
  }

}

/********************************************************************************************************************/

/*

 */
