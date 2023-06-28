using Godot;
using System;

public class GameManager : Node2D
{
  [Export]
  public SelectionMode Mode = SelectionMode.Player;
  [Export]
  public NodePath PlayerNode;
  [Export]
  public NodePath SelectorNode;
  [Export]
  public NodePath ObjectNode;

  public override void _Ready() {
    GD.Print(PlayerNode);
  }

  public override void _Input(InputEvent @event) {

    if (@event is InputEventKey eventKey)
    {
      if (eventKey.Pressed)
      {
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
      EntityPostion[0] += 64;
    if (KeyPressed == "Left")
      EntityPostion[0] -= 64;
    if (KeyPressed == "Up")
      EntityPostion[1] -= 64;
    if (KeyPressed == "Down")
      EntityPostion[1] += 64;

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
    NodePath hoverObjectPath = GetNode<Selector>(this.SelectorNode).HoverObject;
    if(hoverObjectPath != null) {
      this.ObjectNode = hoverObjectPath;
      Mode = SelectionMode.Object;
      GD.Print("Selected");
    }
    else {
      GD.Print("Nothing Found");
    }
  }
}

public enum SelectionMode {
  Player, Selector, Object
}
