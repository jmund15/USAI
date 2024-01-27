using Godot;
using System;

public partial class PauseMenu : Control
{
	public Button ReturnMainMenuButton { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ReturnMainMenuButton = GetNode<Button>("ReturnMainMenuButton");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
