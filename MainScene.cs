using Godot;
using System;

public partial class MainScene : Node
{

    private Global _global;
    private Events _signalBus;

    private HUD _hud;
    private MainMenu _mainMenu;
    private PauseMenu _pauseMenu;
    private MapManager _mapManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        _mainMenu = GetNode<MainMenu>("CanvasLayer/MainMenu");
        _hud = GetNode<HUD>("CanvasLayer/HUD");
        _pauseMenu = GetNode<PauseMenu>("CanvasLayer/PauseMenu");
        _mapManager = GetNode<MapManager>("MapManager");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
