using Godot;
using System;
using System.Collections.Generic;
public partial class Events : Node
{
    [Signal]
    public delegate void MinigameStartEventHandler(Minigame minigame);

    [Signal]
    public delegate void MinigameOverEventHandler(Minigame minigame);

    [Signal]
    public delegate void DebatePlayerOutEventHandler(int playerNum);

    //[Signal]
    //public delegate void DebtPlayerStunnedEventHandler(int playerNum);

    public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
	}
}
