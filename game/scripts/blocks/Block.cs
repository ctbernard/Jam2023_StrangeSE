using Godot;

public class Block : StaticBody2D {
  // TODO: preload the character and any other elements (blocks?) in the scene
  // Base block scene cannot be inherited, but if I were to try it would be by abstracting necessary fields and
  // most blocks will be added to the game scene by code rather than added directly, so add
  // can still build blocks similarly
  private static int blockCount;
  private Vector2 originalPosition;
  private Character character;
  /* Notes for base block
   * Moveable object that is controlled in some way by the character or user
   *
   *
   */
  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  //Preloads go here
  // private Texture texture = ResourceLoader.Load("") as Texture;

  //Constuctors

  public Block(Vector2 originalPosition) => this.originalPosition = originalPosition;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() => character = GetNode<Character>("../Character");

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta) {
    //Check condition for moving and allow movement
  }

  public void blockMove() {
    //some blocks might require an animation to move from spot to spot,
    //but expectation is that the block goes 4 cardinal directions w/o animation change

    //if animation changes or disappear, break animation > change location > remake animation (Water?)

    //moving block

  }

  public void blockOnTile() {

  }
}

  /********************************************************************************************************************/

  /* Notes for block objects inheriting this block
   Blocks currently include:
    hole, standard, immovable, water, fire, bush,

   All blocks are interactable with the player and require code except for the immoveable black (can just be a tile?)



   */
