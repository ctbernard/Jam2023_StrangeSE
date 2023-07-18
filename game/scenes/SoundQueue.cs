using Godot;
using System;
using System.Collections.Generic;

public class SoundQueue : Node {
  private int Next = 0;
  private List<AudioStreamPlayer> AudioStreamPlayerList = new List<AudioStreamPlayer>();

  [Export] public int Count { get; set; } = 1;

  // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      if (GetChildCount() == 0) {
        GD.Print("No AudioStream Player child found");
        return;
      }

      var child = GetChild(0);
      if (child is AudioStreamPlayer audioStreamPlayer) {
        AudioStreamPlayerList.Add(audioStreamPlayer);

        for (int i = 0; i < Count; i++) {
          AudioStreamPlayer duplicate = audioStreamPlayer.Duplicate() as AudioStreamPlayer;
          AddChild(duplicate);
          AudioStreamPlayerList.Add(duplicate);
        }
      }
    }

    public override string _GetConfigurationWarning() {
      if (GetChildCount() == 0) {
        return "No children found. Expected one AudioStreamPlayer child.";
      }

      if (GetChild(0) is not AudioStreamPlayer) {
        return "Expected first child to be an AudioStreamPlayer.";
      }

      return base._GetConfigurationWarning();
    }


}
