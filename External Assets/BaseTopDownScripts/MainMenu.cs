using Godot;
using System;

public partial class MainMenu : Control
{
    private Global _global;
    private Events _signalBus;
    
	public Button StartButton { get; private set; }
	private string START_BUTTON_MAP_PATH = "";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        StartButton = GetNode<Button>("StartButton");
        StartButton.Pressed += OnStartButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnStartButtonPressed()
	{
		//animation??? music??? etc.
		//_signalBus.EmitSignal(nameof(Events.LoadMapEventHandler), START_BUTTON_MAP_PATH);
	}
}
