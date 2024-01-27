using Godot;
using System;

public partial class Global : Node
{
	private Events _signalBus;

	public const float LowestPixelSize = 0.01f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_signalBus = GetNode<Events>("/root/Events");
	}

	private void OnPlayerTurnHuman()
	{
	}
	private void OnPlayerTurnMonster()
	{
	}
}
