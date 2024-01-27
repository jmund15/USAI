using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Godot.Projection;

public partial class MainScene : Node
{
    private Global _global;
    private Events _signalBus;

    private HUD _hud;
    private MainMenu _mainMenu;
    private PauseMenu _pauseMenu;
    //private GameManager _gameManager;

    public List<Candidate> players { get; private set; }

    private PackedScene _selectScreen;

    private PackedScene _debateMinigame;
    public Node CurrentScene { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        //_mainMenu = GetNode<MainMenu>("UI/MainMenu");
        //_hud = GetNode<HUD>("UI/HUD");
        //_pauseMenu = GetNode<PauseMenu>("UI/PauseMenu");
        //_gameManager = GetNode<GameManager>("GameManager");

        _selectScreen = GD.Load<PackedScene>("res://Mingames/SelectScreen/select_screen.tscn");
        _debateMinigame = GD.Load<PackedScene>("res://Mingames/DebateMinigame/debate_minigame.tscn");


        CurrentScene = _selectScreen.Instantiate();
        AddChild(CurrentScene);

        players = _global.Players;

        //LOAD SELECT SCREEN
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (players != _global.Players)
        {
            players = _global.Players;
        }
    }

    public async Task OnStartGame()
    {
        //INTRO
    }

    public void LaunchMinigame(Minigame game)
    {
        switch (game)
        {
            case Minigame.Debate:
                CurrentScene = _debateMinigame.Instantiate();
                var debate = CurrentScene as DebateMinigame;
                AddChild(debate);
                _global.CurrentMinigame = Minigame.Debate;

                //PLACE PLAYERS IN STAGE

                foreach (var player in players)
                {
                    player.IsAlive = true;
                }
                break;
            default: break;
        }
    }
}
