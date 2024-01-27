using Godot;
using System;

public partial class SelectScreen : Node2D
{
	private Node2D _player1Select;
	private Node2D _player2Select;
	private Node2D _player3Select;
	private Node2D _player4Select;

	private AnimationPlayer _player1Anim;
	private AnimationPlayer _player2Anim;
	private AnimationPlayer _player3Anim;
	private AnimationPlayer _player4Anim;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player1Select = GetNode<Node2D>("Player1Select");
        _player2Select = GetNode<Node2D>("Player2Select");
        _player3Select = GetNode<Node2D>("Player3Select");
        _player4Select = GetNode<Node2D>("Player4Select");

		_player1Anim = _player1Select.GetNode<AnimationPlayer>("AnimationPlayer");
        _player2Anim = _player2Select.GetNode<AnimationPlayer>("AnimationPlayer");
        _player3Anim = _player3Select.GetNode<AnimationPlayer>("AnimationPlayer");
        _player4Anim = _player4Select.GetNode<AnimationPlayer>("AnimationPlayer");

		_player1Anim.Play("idleNotBiden");
		_player2Anim.Play("idleNotObama");
		_player3Anim.Play("idleNotTrump");
		_player4Anim.Play("idleNotHillary");

		GD.Print("init select screen");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
