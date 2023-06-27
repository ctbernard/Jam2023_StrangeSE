using Godot;
using System;

public class Selector : Area2D {

  [Export]
  public NodePath HoverObject;

  public override void _Ready()
  {
    this.Connect("body_entered", this, nameof(OnDetectionAreaEntered));
  }

  private void OnDetectionAreaEntered(Node body) {
    HoverObject = body.GetPath();
  }
  private void OnDetectionAreaExited(Node body) {
    HoverObject = null;
  }
}
