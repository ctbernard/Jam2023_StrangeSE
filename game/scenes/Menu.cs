using Godot;
using System;

public class Menu : Control
{
    [Export]
    public NodePath FirstFocus;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      GetNode<Control>(FirstFocus).GrabFocus();
    }

    public void On_Start_Button_Pressed() {

    }

    public void On_Option_Button_Pressed() {

    }

    public void On_Exit_Button_Pressed() {
      GetTree().Quit();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
