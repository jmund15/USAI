using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    #region CLASS_VARIABLES
    private Global _global { get; set; }
    private Events _signalBus { get; set; }


    private PackedScene _candidate;

    #endregion

    #region BASE_GODOT_OVERRIDEN_FUNCTIONS
    public override void _Ready()
    {
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        _candidate = GD.Load<PackedScene>("res://Characters/candidate.tscn");

        AddPlayer(1, "JIZZLE");
    }

    public override void _Process(double delta)
    {
    }
    #endregion

    #region SIGNAL_LISTENERS
    #endregion

    #region HELPER_FUNCITONS
    public void AddPlayer(int num, string name)
    {
        foreach (var player in _global.Players)
        {
            if (player.PlayerNum == num)
            {
                GD.PrintErr("ERROR || Can't add player bc player num desired already exists!");
                return;
            }
        }
        var newPlayer = _candidate.Instantiate() as Candidate;
        newPlayer.PlayerNum = num;
        newPlayer.PlayerName = name;
       
        AddChild(newPlayer);
        if (!_global.PlayersEnabled)
        {
            newPlayer.ProcessMode = ProcessModeEnum.Disabled;
            newPlayer.Hide();
        }
        _global.Players.Add(newPlayer);

    }
    public void RemovePlayer(int playerNum)
    {
        foreach (var player in _global.Players)
        {
            if (player.PlayerNum == playerNum)
            {
                _global.Players.Remove(player);
                player.QueueFree(); // OR JUST REMOVE FROM TREE TO SAVE STATS????
            }
        }
    }

    public void DisablePlayers()
    {
        foreach (var player in _global.Players)
        {
            player.ProcessMode = ProcessModeEnum.Disabled;
            player.Hide();
        }
    }
    public void EnablePlayers()
    {
        foreach (var player in _global.Players)
        {
            player.ProcessMode = ProcessModeEnum.Pausable;
            player.Show();
        }
    }
    #endregion
}
