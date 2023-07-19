using Godot;
using System;
using System.Collections.Generic;

public class Water : KinematicBody2D
{
    [Export] public string WaterDirection;
    [Export] public PackedScene WaterLeft;
    [Export] public PackedScene WaterUp;
    [Export] public PackedScene WaterDown;
    [Export] public PackedScene WaterRight;
    [Export] public TileMap TileMapReference;
    [Export] public Timer WaterTimer;
    public List<Node2D?> GrowthGrass = new List<Node2D?>();

    public void SetTimer() {
      WaterTimer.Connect("timeout", this, nameof(OnTimerTimeout));
    }

    private void OnTimerTimeout() {
      Grow();
    }

    public void Grow() {

      Node2D GrowGroup = null;
      if (WaterDirection == "Up") {
        GrowGroup = this.GetNode<Node2D>("Grow Up");
      }
      else if (WaterDirection == "Left") {
        GrowGroup = this.GetNode<Node2D>("Grow Left");
      }
      else if (WaterDirection == "Right") {
        GrowGroup = this.GetNode<Node2D>("Grow Right");
      }
      else if(WaterDirection == "Down") {
        GrowGroup = this.GetNode<Node2D>("Grow Down");
      }

      if (GrowGroup.GetChildCount() > 0) {
        Vector2 lastChildPosition = GrowGroup.GetChild<Node2D>(GrowGroup.GetChildCount() - 1).Position;

        if (WaterDirection == "Up") {
          CreateGrowth(new Vector2(lastChildPosition.x, lastChildPosition.y - 32), GrowGroup);
        }
        else if (WaterDirection == "Left") {
          CreateGrowth(new Vector2(lastChildPosition.x - 32, lastChildPosition.y), GrowGroup);
        }
        else if (WaterDirection == "Right") {
          CreateGrowth(new Vector2(lastChildPosition.x + 32, lastChildPosition.y), GrowGroup);
        }
        else if(WaterDirection == "Down") {
          CreateGrowth(new Vector2(lastChildPosition.x, lastChildPosition.y + 32), GrowGroup);
        }
      }
      else {
        if (WaterDirection == "Up") {
          CreateGrowth(new Vector2(0, -32), GrowGroup);
        }
        else if (WaterDirection == "Left") {
          CreateGrowth(new Vector2(-32, 0), GrowGroup);
        }
        else if (WaterDirection == "Right") {
          CreateGrowth(new Vector2(32, 0), GrowGroup);
        }
        else if(WaterDirection == "Down") {
          CreateGrowth(new Vector2(0, 32), GrowGroup);
        }
      }
    }

    public void CreateGrowth(Vector2 vector2, Node2D growGroup) {
      Vector2 worldPos = new Vector2((this.Position.x / 32) + (vector2.x/32),(this.Position.y / 32) + (vector2.y/32) );
      string tileName = GetTileName(worldPos);
      string entityName = GetEntityName(worldPos);
      Node2D waterPreload = null;
      if (tileName == "Empty" && entityName == "Nothing") {
        if (WaterDirection == "Up") {
          waterPreload = WaterUp.Instance() as Node2D;
        }
        else if (WaterDirection == "Left") {
          waterPreload = WaterLeft.Instance() as Node2D;
        }
        else if (WaterDirection == "Right") {
          waterPreload = WaterRight.Instance() as Node2D;
        }
        else if(WaterDirection == "Down") {
          waterPreload = WaterDown.Instance() as Node2D;
        }
        waterPreload.Position = vector2;
        growGroup.AddChild(waterPreload);
      }
      else if (entityName.Contains("Grass")) {
        SetGrassGrowthDirection(worldPos,WaterDirection);
      }
    }

    public string GetTileName(Vector2 vector2) {
      Vector2 WorldPos = new Vector2(vector2.x * 32, vector2.y * 32);
      Vector2 CellPos = TileMapReference.WorldToMap(WorldPos);
      int CellTitleID = TileMapReference.GetCellv(CellPos);
      return TileMapReference.TileSet.TileGetName(CellTitleID);
    }

    public string GetEntityName(Vector2 vector2) {
      var checkObjects = GetTree().GetNodesInGroup("Entity");

      foreach (Node node in checkObjects)
      {
        if (node is Node2D node2D)
        {
          if (node2D.Position == new Vector2(vector2.x * 32, vector2.y * 32))
          {
            return node.Name;
          }
        }
      }
      return "Nothing";
    }

    public string SetGrassGrowthDirection(Vector2 vector2, string direction) {
      var checkObjects = GetTree().GetNodesInGroup("Entity");

      foreach (Node node in checkObjects)
      {
        if (node is Node2D node2D)
        {
          if (node2D.Position == new Vector2(vector2.x * 32, vector2.y * 32))
          {
            if (node is Grass grassScript) {
              grassScript.GrowthDirection = direction;
              GrowthGrass.Add(grassScript.GrowGrass());
            }
          }
        }
      }
      return "Nothing";
    }
}
