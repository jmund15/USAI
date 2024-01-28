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
    private PlayerManager _playerManager;
    //private GameManager _gameManager;

    public List<Candidate> players { get; private set; }

    private PackedScene _selectScreen;

    private PackedScene _debateMinigame;
    private PackedScene _debtMinigame;
    private PackedScene _rigMinigame;
    private PackedScene _babyMinigame;
    public Node CurrentScene { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        _signalBus.MinigameStart += OnMinigameStart;
        _signalBus.MinigameOver += OnMinigameOver;


        //_mainMenu = GetNode<MainMenu>("UI/MainMenu");
        //_hud = GetNode<HUD>("UI/HUD");
        //_pauseMenu = GetNode<PauseMenu>("UI/PauseMenu");
        //_gameManager = GetNode<GameManager>("GameManager");

        _playerManager = GetNode<PlayerManager>("PlayerManager");

        _selectScreen = GD.Load<PackedScene>("res://Menus/SelectScreen/select_screen.tscn");
        _debateMinigame = GD.Load<PackedScene>("res://Mingames/DebateMinigame/debate_minigame.tscn");
        _debtMinigame = GD.Load<PackedScene>("res://Mingames/DebtMinigame/debt_minigame.tscn");
        _rigMinigame = GD.Load<PackedScene>("res://Mingames/RigMinigame/rig_minigame.tscn");
        _babyMinigame = GD.Load<PackedScene>("res://Mingames/BabyMinigame/baby_minigame.tscn");

        CurrentScene = _selectScreen.Instantiate();
        var selectScreen = CurrentScene as CharSelectScreen;
        selectScreen.StartGame += OnStartGame;
        AddChild(CurrentScene);

        players = _global.Players;

        _global.CurtainAnim.Play("openCurtain");
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if (players != _global.Players)
        {
            players = _global.Players;
        }
    }

    public async void OnStartGame()
    {
        //INTROs

        //_signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debate));
        //_signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debt));
        _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debate));
    }

    public void OnMinigameStart(Minigame game)
    {
        switch (game)
        {
            case Minigame.Debate:
                _global.CurrentMinigame = Minigame.Debate;

                //PLACE PLAYERS IN STAGE
                int debateStartX = 150;
                foreach (var player in players)
                {
                    player.Position = new Vector2(debateStartX, 400);
                    player.IsAlive = true;
                    debateStartX += 260;
                }

                CurrentScene = _debateMinigame.Instantiate();
                var debate = CurrentScene as DebateMinigame;
                AddChild(debate);

                _playerManager.EnablePlayers();
                _global.PlayersEnabled = true;
                break;
            case Minigame.Debt:
                _global.CurrentMinigame = Minigame.Debt;
                //PLACE PLAYERS IN STAGE
                int debtStartX = 150;
                foreach (var player in players)
                {
                    player.Position = new Vector2(debtStartX, 430);
                    debtStartX += 175;
                }

                CurrentScene = _debtMinigame.Instantiate();
                var debt = CurrentScene as DebtMinigame;
                AddChild(debt);
                _playerManager.EnablePlayers();
                _global.PlayersEnabled = true;
                break;
            case Minigame.Rig:
                _global.CurrentMinigame = Minigame.Rig;
                int rigStartX = 150;
                foreach (var player in players)
                {
                    player.Position = new Vector2(rigStartX, 450);
                    player.IsAlive = true;
                    rigStartX += 260;
                }

                CurrentScene = _rigMinigame.Instantiate();
                var rig = CurrentScene as RigMinigame;
                AddChild(rig);
                _playerManager.EnablePlayers();
                _global.PlayersEnabled = true;
                break;
            case Minigame.BabyKisser:
                _global.CurrentMinigame = Minigame.BabyKisser;
                int babyStartX = 150;
                foreach (var player in players)
                {
                    player.Position = new Vector2(babyStartX, 430);
                    player.IsAlive = true;
                    babyStartX += 260;
                }

                CurrentScene = _babyMinigame.Instantiate();
                var baby = CurrentScene as BabyMinigame;
                AddChild(baby);
                _playerManager.EnablePlayers();
                _global.PlayersEnabled = true;
                break;
            default: break;
        }
    }
    private async void OnMinigameOver(Minigame minigame)
    {
        _global.CurrentMinigame = Minigame.Cutscene;
        _playerManager.DisablePlayers();
    }
}
