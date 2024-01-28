using Godot;
using System;

public partial class EndScene : Node2D
{
	public Sprite2D _winnerHead;
    public Sprite2D _loserHead;

	public Label WinnerLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_winnerHead = GetNode<Sprite2D>("Pile/WinnerHead");
        _loserHead = GetNode<Sprite2D>("Pile/LastPlaceHead");

		WinnerLabel = GetNode<Label>("WinnerLabel");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
