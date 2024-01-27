using Godot;
using System;
using System.Net.Http.Headers;

public partial class MainScene : Node
{
	public enum GameState
	{
		MAIN_MENU,
		IN_GAME,
		PAUSE_MENU
	}
    [Signal]
    public delegate void GameStateChangeEventHandler(GameState state);

    private Global _global;
	private Events _signalBus;

	private HUD _hud;
	private MainMenu _mainMenu;
	private PauseMenu _pauseMenu;
	private MapManager _mapManager;

	private Node _currScene;
	private string _currScenePath;

	private GameState _gameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        _mainMenu = GetNode<MainMenu>("CanvasLayer/MainMenu");
        _hud = GetNode<HUD>("CanvasLayer/HUD");
        _pauseMenu = GetNode<PauseMenu>("CanvasLayer/PauseMenu");
        _mapManager = GetNode<MapManager>("MapManager");

		GameStateChange += OnGameStateChange;

		_mainMenu.StartButton.Pressed += () => OnGameStateChange(GameState.IN_GAME);
        _hud.PauseButton.Pressed += () => OnGameStateChange(GameState.PAUSE_MENU);
        _pauseMenu.ReturnMainMenuButton.Pressed += () => OnGameStateChange(GameState.MAIN_MENU);

        EmitSignal(SignalName.GameStateChange, Variant.From(GameState.MAIN_MENU));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	private void OnGameStateChange(GameState newState)
	{
		switch (newState)
		{
			case GameState.MAIN_MENU:
                _mainMenu.Visible = true;
				_hud.Visible = false;
                break;
			case GameState.IN_GAME:
				_mainMenu.Visible = false;
				_hud.Visible = true;
				break;
			case GameState.PAUSE_MENU:
                _mainMenu.Visible = false;
				_pauseMenu.Visible = true;
				break; 
			default:
				GD.PrintErr("ERROR || Undefined 'GameState': " +  newState);
				break;
		}
        _gameState = newState; // This way, "_gameState" preserves its value until after the switch statement.
							   // This means within the switch statements we can check what the previous state was and use logic based on that.
    }
}
