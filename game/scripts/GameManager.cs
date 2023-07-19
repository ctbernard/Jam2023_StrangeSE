using Godot;
using System;using System.Collections.Generic;

public class GameManager : Node2D {
  [Export] public Dictionary<string, string> TileSetDictionary;
  [Export] public NodePath FloorMapNode;
  [Export] public NodePath EntityMapNode;
  [Export] public PackedScene PlayerPreload;
  [Export] public PackedScene WoodPreload;
  [Export] public PackedScene WaterPreload;
  [Export] public PackedScene GrassPreload;
  [Export] public NodePath MainMenuPreload;
  [Export] public NodePath WaterTimer;

  public SelectionMode Mode = SelectionMode.Player;
  public Node2D SelectedEntity;
  public TileMap tileMapFloor;
  public TileMap tileMapEntity;

  public NodePath PlayerNodePath;
  public Node2D PlayerNode;
  public Node2D SelectionRangeNode;
  public Node2D SelectorNode;
  public Node2D ObjectNode;

  public AudioStreamPlayer2D player_move_sound;
  public AudioStreamPlayer2D object_select_sound;
  public AudioStreamPlayer2D victory_sound;

  public override void _Ready() {
	tileMapFloor = GetNode<TileMap>(FloorMapNode);
	tileMapEntity = GetNode<TileMap>(EntityMapNode);
	CreateEntityMap(tileMapEntity);
	SelectedEntity = GetNode<Node2D>(this.PlayerNodePath);

	PlayerNode = GetNode<Node2D>(this.PlayerNodePath);
	SelectionRangeNode = PlayerNode.GetNode<Node2D>("SelectionRange");
	SelectorNode = PlayerNode.GetNode<Node2D>("Selector");
	GetNode<Timer>(WaterTimer).Start();
	player_move_sound = GetNode<AudioStreamPlayer2D>("player_move");
	object_select_sound = GetNode<AudioStreamPlayer2D>("object_select");
	victory_sound = GetNode<AudioStreamPlayer2D>("victory");
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
		if (TileName.Contains("Water")) {
		  Node2D waterPreload = WaterPreload.Instance() as Node2D;
		  waterPreload.Position = new Vector2(x * 32, y * 32);
		  AddChild(waterPreload);

		  string[] substrings = TileName.Split('_');
		  string lastSubstring = substrings[substrings.Length - 1];
		  if (waterPreload is Water waterScript) {
			waterScript.TileMapReference = tileMapFloor;
			waterScript.WaterDirection = lastSubstring;
			waterScript.WaterTimer = GetNode<Timer>(WaterTimer);
			waterScript.SetTimer();

		  }
		}
		if (TileName.Contains("Grass")) {
		  Node2D grassPreload = GrassPreload.Instance() as Node2D;
		  grassPreload.Position = new Vector2(x * 32, y * 32);
		  AddChild(grassPreload);

		  if (grassPreload is Grass grassScript) {
			grassScript.TileMapReference = tileMapFloor;
		  }
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
		else if (KeyPressed == "R") {
		  GetTree().ReloadCurrentScene();
		}
		else if (KeyPressed == "Escape") {
		  GetTree().ChangeScene("res://game/scenes/Main Menu.tscn");
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

	  if (Mode == SelectionMode.Player){
		player_move_sound.Play();
	  }
	 if (Mode == SelectionMode.Selector){
		object_select_sound.Play();
	  }
	  if (Mode == SelectionMode.Object && IsObjectOutOfRange(coordinate)) {
		GD.Print("out of range");
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

	switch (Mode) {
	  case SelectionMode.Player:
		if (tileName == "Empty" && entityName == "Nothing")
		  return true;
		else if (tileName == "Exit") {
		  Win();
		  return true;
		}
		else
		  return false;
		break;
	  case SelectionMode.Selector:
		Node2D upNode = SelectionRangeNode.GetNode<Node2D>("Up");
		Node2D downNode = SelectionRangeNode.GetNode<Node2D>("Down");
		Node2D leftNode = SelectionRangeNode.GetNode<Node2D>("Left");
		Node2D rightNode = SelectionRangeNode.GetNode<Node2D>("Right");
		Node2D upLeftNode = SelectionRangeNode.GetNode<Node2D>("UpLeft");
		Node2D upRightNode = SelectionRangeNode.GetNode<Node2D>("UpRight");
		Node2D downLeftNode = SelectionRangeNode.GetNode<Node2D>("DownLeft");
		Node2D downRightNode = SelectionRangeNode.GetNode<Node2D>("DownRight");
		if ((vector2 == new Vector2(0, -1) && upNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(0, 1) && downNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(-1, 0) && leftNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(1, 0) && rightNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(-1, -1) && upLeftNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(1, -1) && upRightNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(-1, 1) && downLeftNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(1, 1) && downRightNode.IsVisibleInTree()) ||
			(vector2 == new Vector2(0, 0))) {
		  return true;
		}
		else
		  return false;
		break;
	  case SelectionMode.Object:
		if (tileName == "Empty" && new Vector2(vector2.x * 32, vector2.y * 32) != PlayerNode.Position && entityName == "Nothing")
		  return true;
		else if (entityName.Contains("Water") && entityName.Contains("Down") ||
				 entityName.Contains("Water") && entityName.Contains("Left") ||
				 entityName.Contains("Water") && entityName.Contains("Right")||
				 entityName.Contains("Water") && entityName.Contains("Up")) {
		  RemoveGrowth(vector2);
		  return true;
		}
		else
		  return false;
		break;
	  default:
		return false;
		break;
	}
  }

  public void Win() {
	GetTree().ChangeScene("res://game/scenes/Main Menu.tscn");
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
		if (node2D.Position == new Vector2(vector2.x * 32, vector2.y * 32) || node2D.GlobalTransform.origin == new Vector2(vector2.x * 32, vector2.y * 32))
		{
		  GD.Print(node.Name);
		  return node.Name;
		}
	  }
	}
	return "Nothing";
  }

  public string RemoveGrowth(Vector2 vector2) {
	var checkObjects = GetTree().GetNodesInGroup("Entity");

	foreach (Node node in checkObjects)
	{
	  if (node is Node2D node2D)
	  {
		if (node2D.GlobalTransform.origin == new Vector2(vector2.x * 32, vector2.y * 32))
		{

		  if (node2D.GetParent().GetParent() is Water waterScript) {
			foreach (Node2D growthNode in waterScript.GrowthGrass) {
			  if (growthNode != null && growthNode.GetChildCount() > 0) {
				for (int i = 0; i < growthNode.GetChildCount(); i++)
				{
				  Node child2 = growthNode.GetChild(i);
				  child2.Free();
				}
			  }
			}
			waterScript.GrowthGrass.Clear();
		  }
		  int numChildren = node2D.GetParent().GetChildCount();
		  int startPosition = node2D.GetIndex();

		  for (int i = numChildren - 1; i >= startPosition; i--)
		  {
			Node child = node2D.GetParent().GetChild(i);
			child.Free();
		  }


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
	Node2D upLeftNode = SelectionRangeNode.GetNode<Node2D>("UpLeft");
	Node2D upRightNode = SelectionRangeNode.GetNode<Node2D>("UpRight");
	Node2D downLeftNode = SelectionRangeNode.GetNode<Node2D>("DownLeft");
	Node2D downRightNode = SelectionRangeNode.GetNode<Node2D>("DownRight");

	Vector2 upVector = new Vector2((PlayerNode.Position.x / 32),(PlayerNode.Position.y / 32) - 1 );
	Vector2 downVector = new Vector2((PlayerNode.Position.x / 32),(PlayerNode.Position.y / 32) + 1 );
	Vector2 leftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32));
	Vector2 rightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32));
	Vector2 upLeftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32) - 1 );
	Vector2 upRightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32) - 1 );
	Vector2 downLeftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32) + 1);
	Vector2 downRightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32) + 1);

	upNode.Visible = GetTileName(upVector) == "Wood" || GetTileName(upVector) == "Empty";
	downNode.Visible = GetTileName(downVector) == "Wood" || GetTileName(downVector) == "Empty";
	leftNode.Visible = GetTileName(leftVector) == "Wood" || GetTileName(leftVector) == "Empty";
	rightNode.Visible = GetTileName(rightVector) == "Wood" || GetTileName(rightVector) == "Empty";
	upLeftNode.Visible = GetTileName(upLeftVector) == "Wood" || GetTileName(upLeftVector) == "Empty";
	upRightNode.Visible = GetTileName(upRightVector) == "Wood" || GetTileName(upRightVector) == "Empty";
	downLeftNode.Visible = GetTileName(downLeftVector) == "Wood" || GetTileName(downLeftVector) == "Empty";
	downRightNode.Visible = GetTileName(downRightVector) == "Wood" || GetTileName(downRightVector) == "Empty";
  }

  public bool IsObjectOutOfRange(Vector2 vector2) {
	Node2D upNode = SelectionRangeNode.GetNode<Node2D>("Up");
	Node2D downNode = SelectionRangeNode.GetNode<Node2D>("Down");
	Node2D leftNode = SelectionRangeNode.GetNode<Node2D>("Left");
	Node2D rightNode = SelectionRangeNode.GetNode<Node2D>("Right");
	Node2D upLeftNode = SelectionRangeNode.GetNode<Node2D>("UpLeft");
	Node2D upRightNode = SelectionRangeNode.GetNode<Node2D>("UpRight");
	Node2D downLeftNode = SelectionRangeNode.GetNode<Node2D>("DownLeft");
	Node2D downRightNode = SelectionRangeNode.GetNode<Node2D>("DownRight");

	Vector2 upVector = new Vector2((PlayerNode.Position.x / 32),(PlayerNode.Position.y / 32) - 1 );
	Vector2 downVector = new Vector2((PlayerNode.Position.x / 32),(PlayerNode.Position.y / 32) + 1 );
	Vector2 leftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32));
	Vector2 rightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32));
	Vector2 upLeftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32) - 1 );
	Vector2 upRightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32) - 1 );
	Vector2 downLeftVector = new Vector2((PlayerNode.Position.x / 32) - 1,(PlayerNode.Position.y / 32) + 1);
	Vector2 downRightVector = new Vector2((PlayerNode.Position.x / 32) + 1,(PlayerNode.Position.y / 32) + 1);

	if (vector2 == upVector ||
		vector2 == downVector ||
		vector2 == leftVector ||
		vector2 == rightVector ||
		vector2 == upLeftVector ||
		vector2 == upRightVector ||
		vector2 == downLeftVector ||
		vector2 == downRightVector) {
	  return false;
	}
	else {
	  return true;
	}

  }
}

public enum SelectionMode {
  Player, Selector, Object
}
