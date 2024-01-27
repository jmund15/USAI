using Godot;
using System;

public partial class HUD : Control
{
    private Global _global;
    private Events _signalBus;
    public Button PauseButton { get; private set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        PauseButton = GetNode<Button>("PauseButton");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
