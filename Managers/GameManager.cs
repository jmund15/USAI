using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }

    
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        
        //LaunchMinigame(Minigame.Debate);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        
    }
}
