using Godot;
using System;
using System.Collections.Generic;

public partial class EndScene : Node2D
{
    private Global _global;

    private List<string> playerNames;
    public Sprite2D _winnerHead;
    public Sprite2D _loserHead;

	public Label WinnerLabel;

	public string WinnerText;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");

        _winnerHead = GetNode<Sprite2D>("Pile/WinnerHead");
        _loserHead = GetNode<Sprite2D>("Pile/LastPlaceHead");

		WinnerLabel = GetNode<Label>("WinnerLabel");
		WinnerLabel.Text = WinnerText;

        var candidates = _global.Players;
        playerNames = new List<string> { "disabled", "disabled", "disabled", "disabled" };

        for (int i = 0; i < candidates.Count; i++)
        {
            playerNames[i] = candidates[i].PlayerName;
        }

        int maxPlayerID = -1;

        for (int i = 0; i < candidates.Count; i++)
        {
            Label resLabel = GetNode<Label>((string.Format("Results/P{0}Result", candidates[i].PlayerNum)));
            resLabel.Text = string.Format("{0} had {1}%!", candidates[i].PlayerName, candidates[i].TotalScore);
            if (maxPlayerID == -1)
            {
                maxPlayerID = i;
            }
            else if (candidates[i].TotalScore > candidates[maxPlayerID].TotalScore)
            {
                maxPlayerID = i;
            }
        }

        Label winLabel = GetNode<Label>((string.Format("Results/Winner")));

        winLabel.Text = string.Format("{0} WINS!", candidates[maxPlayerID].PlayerName);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
