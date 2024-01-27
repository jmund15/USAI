using Godot;
using System;
public partial class Events : Node
{
    //[Signal]
    //public delegate void LoadMapEventHandler(string mapPath);
    

    [Signal]
    public delegate void LaunchMinigameEventHandler(Minigame minigame);

	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
	}
}
