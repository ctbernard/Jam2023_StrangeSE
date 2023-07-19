using Godot;
using System;

public class SoundManager : Node {

  private AudioStreamPlayer BackgroundAudio = new AudioStreamPlayer();

  //SoundEffect
  [Export] public AudioStream Click;
  [Export] public AudioStream Walk;

  //Background
  [Export] public AudioStream TitleScreen;
  [Export] public AudioStream Victory;
  [Export] public AudioStream GameOver;
  [Export] public AudioStream Level1;

    public override void _Ready()
    {

    }

    public void ChangeBackground(E_BackGroundMusic backGroundMusic) {
      if (BackgroundAudio.Playing)
        BackgroundAudio.Stop();

      switch (backGroundMusic)
      {
        case E_BackGroundMusic.Titlescreen:
          BackgroundAudio.Stream = TitleScreen;
          break;
        case E_BackGroundMusic.Victory:
          BackgroundAudio.Stream = Victory;
          break;
        case E_BackGroundMusic.Game_Over:
          BackgroundAudio.Stream = GameOver;
          break;
        case E_BackGroundMusic.Level_1:
          BackgroundAudio.Stream = Level1;
          break;
      }

      BackgroundAudio.Play();
    }

}


public enum E_BackGroundMusic {
  Titlescreen,
  Victory,
  Game_Over,
  Level_1,
}
public enum E_SoundEffect {
  Click,
  Walk,
}
