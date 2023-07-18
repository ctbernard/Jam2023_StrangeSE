using Godot;
using System;using System.Collections.Generic;

public class GameManager : Node2D {
  [Export] public Dictionary<string, string> TileSetDictionary;
  [Export] public NodePath FloorMapNode;
  [Export] public NodePath EntityMapNode;
  [Export] public PackedScene PlayerPreload;
  [Export] public PackedScene WoodPreload;

  public SelectionMode Mode = SelectionMode.Player;
  public Node2D SelectedEntity;
  public TileMap tileMapFloor;
  public TileMap tileMapEntity;

  public NodePath PlayerNodePath;
  public Node2D PlayerNode;
  public Node2D SelectionRangeNode;
  public Node2D SelectorNode;
  public Node2D ObjectNode;

  public override void _Ready() {
    tileMapFloor = GetNode<TileMap>(FloorMapNode);
    tileMapEntity = GetNode<TileMap>(EntityMapNode);
    CreateEntityMap(tileMapEntity);
    SelectedEntity = GetNode<Node2D>(this.PlayerNodePath);

    PlayerNode = GetNode<Node2D>(this.PlayerNodePath);
    SelectionRangeNode = PlayerNode.GetNode<Node2D>("SelectionRange");
    SelectorNode = PlayerNode.GetNode<Node2D>("Selector");
  }

  public void CreateEntityMap(TileMap tileMap) {
    for (int x = 0;x < 32; x++) {
      for (int y = 0;y < 19; y++) {
        Vector2 WorldPos = new Vector2(x * 32, y * 32);
        Vector2 CellPos = tileMap.WorldToMap(WorldPos);
        int CellTitleID = tileMap.GetCellv(CellPos);
        string TileName = tileMap.TileSet.TileGetName(CellTitleID);

        // if (TileName != "Empty") {
        //   GD.Print(CellPos);
        //   GD.Print(TileName);
        // }

        if (TileName == "Player") {
          Node2D playerPreload = PlayerPreload.Instance() as Node2D;
          playerPreload.Position = new Vector2(x * 32, y * 32);
          AddChild(playerPreload);
          PlayerNodePath = playerPreload.GetPath();
        }
        if (TileName == "Wood") {
          Node2D woodPreload = WoodPreload.Instance() as Node2D;
          woodPreload.Position = new Vector2(x * 32, y * 32);
          AddChild(woodPreload);
        }
      }
    }

    tileMap.Visible = false;
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

    Vector2 coordinate = GetSelectedEntityCoordinate(null);

    if (KeyPressed == "Right") {
      coordinate.x += 1;
    }
    if (KeyPressed == "Left") {
      coordinate.x -= 1;
    }
    if (KeyPressed == "Up") {
      coordinate.y -= 1;
    }
    if (KeyPressed == "Down") {
      coordinate.y += 1;
    }

    if (CanMove(coordinate,Mode)) {
      SelectedEntity.Position = new Vector2(coordinate.x * 32, coordinate.y * 32);

      if (Mode == SelectionMode.Object) {
        ActionChangeMode(SelectionMode.Object);
      }
    }
  }

  public void ActionChangeMode(SelectionMode mode) {
    switch (Mode) {
      case SelectionMode.Player:
        Mode = SelectionMode.Selector;
        this.SelectedEntity = this.SelectorNode;
        SelectionRangeNode.Visible = true;
        SelectorNode.Visible = true;
        ShowSelectionRange();
        break;
      case SelectionMode.Selector:
        Mode = SelectionMode.Player;
        this.SelectedEntity = this.PlayerNode;
        SelectionRangeNode.Visible = false;
        SelectorNode.Visible = false;
        ActionSetObjectNode();
        break;
      case SelectionMode.Object:
        Mode = SelectionMode.Player;
        this.SelectedEntity = this.PlayerNode;
        SelectionRangeNode.Visible = false;
        SelectorNode.Visible = false;
        break;
    }
  }

  public void ActionSetObjectNode() {
    NodePath hoverObjectPath = GetNode<Selector>(this.SelectorNode.GetPath()).HoverObject;
    GD.Print(hoverObjectPath);
    if(hoverObjectPath != null && !hoverObjectPath.ToString().Contains("Player")) {
      this.ObjectNode = GetNode<Node2D>(hoverObjectPath);
      Mode = SelectionMode.Object;
      this.SelectedEntity = this.ObjectNode;
      SelectionRangeNode.Visible = true;
      GD.Print("Selected");
    }
    else {
      GD.Print("Nothing Found");
    }
  }

  public bool CanMove(Vector2 vector2, SelectionMode mode) {
    string tileName = GetTileName(vector2);
    string entityName = GetEntityName(vector2);
    GD.Print(entityName, mode);
    switch (Mode) {
      case SelectionMode.Player:
        if (tileName == "Floor" && entityName == "Nothing")
          return true;
        else
          return false;
        break;
      case SelectionMode.Selector:
        Node2D upNode = SelectionRangeNode.GetNode<Node2D>("Up");
        Node2D downNode = SelectionRangeNode.GetNode<Node2D>("Down");
        Node2D leftNode = SelectionRangeNode.GetNode<Node2D>("Left");
        Node2D rightNode = SelectionRangeNode.GetNode<Node2D>("Right");
        if ((vector2 == new Vector2(0, -1) && upNode.IsVisibleInTree()) ||
            (vector2 == new Vector2(0, 1) && downNode.IsVisibleInTree()) ||
            (vector2 == new Vector2(-1, 0) && leftNode.IsVisibleInTree()) ||
            (vector2 == new Vector2(1, 0) && rightNode.IsVisibleInTree()) ||
            (vector2 == new Vector2(0, 0))) {
          return true;
        }
        else
          return false;
        break;
      case SelectionMode.Object:
        if (tileName == "Floor" && new Vector2(vector2.x * 32, vector2.y * 32) != PlayerNode.Position)
          return true;
          else
          return false;
        break;
      default:
        return false;
        break;
    }
  }

  public string GetTileName(Vector2 vector2) {
    Vector2 WorldPos = new Vector2(vector2.x * 32, vector2.y * 32);
    Vector2 CellPos = tileMapFloor.WorldToMap(WorldPos);
    int CellTitleID = tileMapFloor.GetCellv(CellPos);
    return tileMapFloor.TileSet.TileGetName(CellTitleID);
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

  public Vector2 GetSelectedEntityCoordinate(SelectionMode? mode) {
    Node2D originalSelected = SelectedEntity;
    if (mode != null) {
      switch (Mode) {
        case SelectionMode.Player:
          this.SelectedEntity = this.PlayerNode;
          break;
        case SelectionMode.Selector:
          this.SelectedEntity = this.SelectorNode;
          break;
        case SelectionMode.Object:
          this.SelectedEntity = this.ObjectNode;
          break;
      }
    }

    Vector2 coordinate = new Vector2(this.SelectedEntity.Position.x / 32, this.SelectedEntity.Position.y /32 );

    if (mode != null) {
      this.SelectedEntity = originalSelected;
    }
    return coordinate;
  }

  public void ShowSelectionRange() {
    SelectorNode.Position = new Vector2(0, 0);
    Node2D upNode = SelectionRangeNode.GetNode<Node2D>("Up");
    Node2D downNode = SelectionRangeNode.GetNode<Node2D>("Down");
    Node2D leftNode = SelectionRangeNode.GetNode<Node2D>("Left");
    Node2D rightNode = SelectionRangeNode.GetNode<Node2D>("Right");

    Vector2 upVector = new Vector2((PlayerNode.Position.x / 32),(PlayerNode.Position.y / 32) - 1 );
    Vector2 downVector = new Vector2((PlayerNode.Position.x / 32),(PlayerNode.Position.y / 32) + 1 );
    Vector2 leftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32));
    Vector2 rightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32));

    upNode.Visible = GetTileName(upVector) == "Wood" || GetTileName(upVector) == "Floor";
    downNode.Visible = GetTileName(downVector) == "Wood" || GetTileName(downVector) == "Floor";
    leftNode.Visible = GetTileName(leftVector) == "Wood" || GetTileName(leftVector) == "Floor";
    rightNode.Visible = GetTileName(rightVector) == "Wood" || GetTileName(rightVector) == "Floor";
  }
}

public enum SelectionMode {
  Player, Selector, Object
}
