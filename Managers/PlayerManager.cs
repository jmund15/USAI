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

        //_signalBus.DebatePlayerOut += OnDebatePlayerOut;

        _candidate = GD.Load<PackedScene>("res://Characters/candidate.tscn");

        AddPlayer(1, "JIZZLE", PlayableChar.NotObama, Colors.Black);
        AddPlayer(2, "JOEL", PlayableChar.NotHillary, Colors.Blue);
        AddPlayer(3, "JESWY", PlayableChar.NotTrump, Colors.Red);
        AddPlayer(4, "L-STAR", PlayableChar.NotBiden, Colors.HotPink);
    }

    public override void _Process(double delta)
    {
    }
    #endregion

    #region SIGNAL_LISTENERS
    private void OnDebatePlayerOut(int playerNum)
    {
        foreach (var player in _global.Players)
        {

        }
    }
    #endregion

    #region HELPER_FUNCITONS
    public void AddPlayer(int num, string name, PlayableChar character, Color suitColor)
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
        newPlayer.CharSelected = character;
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
