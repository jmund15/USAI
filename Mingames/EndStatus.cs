using Godot;
using System;
using System.Collections.Generic;

public partial class EndStatus : Node2D
{
	private Global _global;

	private List<string> playerNames;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");

        var candidates = _global.Players;
        playerNames = new List<string> { "disabled", "disabled", "disabled", "disabled" };

        for (int i = 0; i < candidates.Count; i++)
        {
            playerNames[i] = candidates[i].PlayerName;
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            Label resLabel = GetNode<Label>((string.Format("Results/P{0}Result", candidates[i].PlayerNum)));
            resLabel.Text = string.Format("{0} has {1} Points!", candidates[i].PlayerName, candidates[i].TotalScore);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
