using Godot;
using System;

public class Menu : Control
{
    [Export]
    public NodePath FirstFocus;

    [Export]
    public NodePath MenuPath;
    [Export]
    public NodePath HowToPlay;
    [Export]
    public NodePath HTPExitButton;
    [Export]
    public NodePath LevelContainer;
    [Export]
    public NodePath LevelOne;
    [Export]
    public NodePath Play;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      GetNode<Control>(FirstFocus).GrabFocus();
    }

    public void On_Start_Button_Pressed() {
      GetNode<Control>(MenuPath).Visible = false;
      GetNode<Control>(LevelContainer).Visible = true;
      GetNode<Control>(LevelOne).GrabFocus();
    }

    public void On_Option_Button_Pressed() {
      GetNode<Control>(MenuPath).Visible = false;
      GetNode<Control>(HowToPlay).Visible = true;
      GetNode<Control>(HTPExitButton).GrabFocus();
    }

    public void On_Exit_Button_Pressed() {
      GetTree().Quit();
    }

    public void On_HTP_Exit_Button_pressed() {
      GetNode<Control>(HowToPlay).Visible = false;
      GetNode<Control>(MenuPath).Visible = true;
      GetNode<Control>(FirstFocus).GrabFocus();
    }

    public void On_Level_Back_pressed() {
      GetNode<Control>(MenuPath).Visible = true;
      GetNode<Control>(LevelContainer).Visible = false;
      GetNode<Control>(Play).GrabFocus();
    }

    public void LoadLevel(string sceneName) {
      GetTree().ChangeScene("res://game/scenes/" + sceneName +".tscn");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
