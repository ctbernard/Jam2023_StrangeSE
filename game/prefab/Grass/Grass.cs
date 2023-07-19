using Godot;
using System;

public class Grass : KinematicBody2D
{
    [Export] public string GrowthDirection = "";
    [Export] public PackedScene GrassGrowth;
    [Export] public NodePath Before;
    [Export] public NodePath After;
    [Export] public TileMap TileMapReference;

    public Node2D GrowGrass() {

      if (!string.IsNullOrWhiteSpace(GrowthDirection)) {
        Node2D GrowGroup = this.GetNode<Node2D>("Grow");

        if (GrowGroup.GetChildCount() > 0) {
          Vector2 lastChildPosition = GrowGroup.GetChild<Node2D>(GrowGroup.GetChildCount() - 1).Position;

          if (GrowthDirection == "Up") {
            return CreateGrowth(new Vector2(lastChildPosition.x, lastChildPosition.y - 32), GrowGroup);
          }
          else if (GrowthDirection == "Left") {
            return CreateGrowth(new Vector2(lastChildPosition.x - 32, lastChildPosition.y), GrowGroup);
          }
          else if (GrowthDirection == "Right") {
            return CreateGrowth(new Vector2(lastChildPosition.x + 32, lastChildPosition.y), GrowGroup);
          }
          else if(GrowthDirection == "Down") {
            return CreateGrowth(new Vector2(lastChildPosition.x, lastChildPosition.y + 32), GrowGroup);
          }
        }
        else {
          if (GrowthDirection == "Up") {
            return CreateGrowth(new Vector2(16, -16), GrowGroup);
          }
          else if (GrowthDirection == "Left") {
            return CreateGrowth(new Vector2(-48, 16), GrowGroup);
          }
          else if (GrowthDirection == "Right") {
            return CreateGrowth(new Vector2(48, 16), GrowGroup);
          }
          else if(GrowthDirection == "Down") {
            return CreateGrowth(new Vector2(16, 48), GrowGroup);
          }
        }
      }

      return null;
    }

    public Node2D CreateGrowth(Vector2 vector2, Node2D growGroup) {

      Vector2 worldPos = new Vector2((this.Position.x / 32) + (vector2.x/32),(this.Position.y / 32) + (vector2.y/32) );
      string tileName = GetTileName(worldPos);
      string entityName = GetEntityName(worldPos);
      Node2D grassPreload = null;
      if (tileName == "Empty" && entityName == "Nothing") {

        grassPreload = GrassGrowth.Instance() as Node2D;
        grassPreload.Position = vector2;
        growGroup.AddChild(grassPreload);
        return growGroup;
      }
      else {
        return null;
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
}
