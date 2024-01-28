using Godot;
using System;
using System.Collections.Generic;

public enum PlayableChar
{
	NotBiden,
	NotObama,
	NotTrump,
	NotHillary
}
public enum Minigame
{
	Select,
    Cutscene,
    Debate,
    Rig,
    BabyKisser,
    Debt
}

[GlobalClass]
public partial class Global : Node
{
	private Events _signalBus;

	public const float PixelTopOfScreen = -300; // DON'T KNOW
	public const float LowestPixelSize = 0.01f;

	public Minigame CurrentMinigame { get; set; }

	public List<Candidate> Players { get; set; } = new List<Candidate>();
	public bool PlayersEnabled { get; set; } = false;

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
