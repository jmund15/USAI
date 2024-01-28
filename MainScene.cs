using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Godot.Projection;

public partial class MainScene : Node
{
    private bool debug = true;

    private Global _global;
    private Events _signalBus;

    private HUD _hud;
    private MainMenu _mainMenu;
    private PauseMenu _pauseMenu;
    private PlayerManager _playerManager;
    //private GameManager _gameManager;

    public List<Candidate> players { get; private set; }

    private PackedScene _selectScreen;

    private PackedScene _introScene;
    private PackedScene _debateMinigame;
    private PackedScene _debtMinigame;
    private PackedScene _rigMinigame;
    private PackedScene _babyMinigame;
    private PackedScene _midStatus;
    private PackedScene _endScene;

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
        _introScene = GD.Load<PackedScene>("res://Mingames/intro_scene.tscn");
        _debateMinigame = GD.Load<PackedScene>("res://Mingames/DebateMinigame/debate_minigame.tscn");
        _debtMinigame = GD.Load<PackedScene>("res://Mingames/DebtMinigame/debt_minigame.tscn");
        _rigMinigame = GD.Load<PackedScene>("res://Mingames/RigMinigame/rig_minigame.tscn");
        _babyMinigame = GD.Load<PackedScene>("res://Mingames/BabyMinigame/baby_minigame.tscn");
        _midStatus = GD.Load<PackedScene>("res://Mingames/MidStatus.tscn");
        _endScene = GD.Load<PackedScene>("res://Backgrounds/MidStatus/end_scene.tscn");

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
        if (debug)
        {
            players[0].TotalScore = 1000;
            _signalBus.EmitSignal(nameof(Events.MinigameOver), Variant.From(Minigame.BabyKisser));
            return;
        }
        _global.CurtainAnim.Play("openCurtain");

        _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Cutscene));
        _global.CurrentMinigame = Minigame.Cutscene;

        var introScene = _introScene.Instantiate() as IntroScene;
        AddChild(introScene);

        await Task.Delay(TimeSpan.FromSeconds(1.5f));

        introScene.IntroLabel.Text = "The year is 2032...";
        await Task.Delay(TimeSpan.FromSeconds(3f));
        var picTween = GetTree().CreateTween();
        picTween.TweenProperty(introScene._mrica, "modulate:a", 1, 1f);
        introScene.IntroLabel.Text = "America is fed up with having to choose a President every 4 years. To solve this, America developed an AI to preform the election for them.";
        await Task.Delay(TimeSpan.FromSeconds(5f));
        var pic2Tween = GetTree().CreateTween();
        pic2Tween.TweenProperty(introScene._robot, "modulate:a", 1, 1f);
        await Task.Delay(TimeSpan.FromSeconds(3f));

        introScene.IntroLabel.Text = "That AI was titled: USAI!!!";
        var pic3Tween = GetTree().CreateTween();
        pic3Tween.TweenProperty(introScene.Usai, "modulate:a", 1, 1f);
        await Task.Delay(TimeSpan.FromSeconds(3f));

        introScene.IntroLabel.Text = "The top candidates will now preform the final debate based on USAI's top categories!";
        await Task.Delay(TimeSpan.FromSeconds(3f));


        _global.CurtainAnim.Play("closeCurtain");
        await Task.Delay(TimeSpan.FromSeconds(2f));
        introScene.QueueFree();

        _global.CurtainAnim.Play("openCurtain");

        var midStatus = _midStatus.Instantiate() as MidStatus;
        AddChild(midStatus);

        //PLACE PLAYERS IN STAGE
        int cutscenetartX = 100;
        foreach (var player in players)
        {
            player.Position = new Vector2(cutscenetartX, 430);
            player.IsAlive = true;
            cutscenetartX += 150;
            player.ZIndex += 1;
        }
        _playerManager.EnablePlayers();

        await Task.Delay(TimeSpan.FromSeconds(1.5f));

        midStatus.UsaiLabel.Text = "WELCOME TO THE PRESIDENTIAL ELECTION OF 2032!!!";
        await Task.Delay(TimeSpan.FromSeconds(4f));
        midStatus.UsaiLabel.Text = "How's everybody doing?\n";
        await Task.Delay(TimeSpan.FromSeconds(3f));
        midStatus.UsaiLabel.Text = "Today we will be ranking our top candidates off of 4 main categories.";
        await Task.Delay(TimeSpan.FromSeconds(4f));
        midStatus.UsaiLabel.Text = "The first of which is... (pause)";
        await Task.Delay(TimeSpan.FromSeconds(3f));
        midStatus.UsaiLabel.Text = "DODGING QUESTIONS!!";
        await Task.Delay(TimeSpan.FromSeconds(2f));
        _global.CurtainAnim.Play("closeCurtain");
        await Task.Delay(TimeSpan.FromSeconds(1f));
        midStatus.QueueFree();
        //INTROs
        //foreach (var player in _global.Players)
        //{
        //    GD.Print(player.PlayerNum);
        //}
        //_signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debate));
        //_signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debt));

        foreach (var player in players)
        {
            player.ZIndex -= 1;
        }
        _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debate));
    }

    public async void OnMinigameStart(Minigame game)
    {

        if (game == Minigame.Cutscene)
        {
            return;
        }

        _global.CurtainAnim.Play("openCurtain");
        await Task.Delay(TimeSpan.FromSeconds(1f));


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
        _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Cutscene));
        _global.CurrentMinigame = Minigame.Cutscene;

        //PLACE PLAYERS IN STAGE
        int cutscenetartX = 100;
        foreach (var player in players)
        {
            player.Position = new Vector2(cutscenetartX, 410);
            player.IsAlive = true;
            cutscenetartX += 150;
        }
        _playerManager.EnablePlayers();


        var midStatus = _midStatus.Instantiate() as MidStatus;
        AddChild(midStatus);
        switch (minigame)
        {
            case Minigame.Debate:
                _global.CurtainAnim.Play("openCurtain");
                await Task.Delay(TimeSpan.FromSeconds(1f));
                midStatus.UsaiLabel.Text = "WOWIE!!";
                await Task.Delay(TimeSpan.FromSeconds(2f));
                midStatus.UsaiLabel.Text = "That was impressive! I haven't seen questions dodge that well since Steve Job's gerbil incident.";
                await Task.Delay(TimeSpan.FromSeconds(5f)); 
                midStatus.UsaiLabel.Text = "The next game will encompass the true spirit of a presidential candidate!";
                await Task.Delay(TimeSpan.FromSeconds(4f)); 
                midStatus.UsaiLabel.Text = "RAISING THE NATIONAL DEBT!!!!!";
                await Task.Delay(TimeSpan.FromSeconds(3f)); 
                //APPLAAUSE
                midStatus.UsaiLabel.Text = "But there's a catch. My stats show that presidents should be underhanded, deceitful, and scummy.";
                await Task.Delay(TimeSpan.FromSeconds(5f)); 
                midStatus.UsaiLabel.Text = "That is why you'll have to raise the debt behind my back!";
                await Task.Delay(TimeSpan.FromSeconds(3f));
                midStatus.UsaiLabel.Text = "Don't let me catch you ;)";
                await Task.Delay(TimeSpan.FromSeconds(3f));
                _global.CurtainAnim.Play("closeCurtain");

                break;
            case Minigame.Debt:
                _global.CurtainAnim.Play("openCurtain");
                await Task.Delay(TimeSpan.FromSeconds(1f));

                midStatus.UsaiLabel.Text = "Congratulations for making it this far!";
                await Task.Delay(TimeSpan.FromSeconds(3f));
                midStatus.UsaiLabel.Text = "Gerbils Steve Stevie.";
                await Task.Delay(TimeSpan.FromSeconds(2f));
                midStatus.UsaiLabel.Text = "Finally, the candidates will vote for which president to elect.";
                await Task.Delay(TimeSpan.FromSeconds(3f));
                midStatus.UsaiLabel.Text = "Or like... I will. But I'll drop the votes all at once so it feels  like it's kinda... organic?";
                await Task.Delay(TimeSpan.FromSeconds(4f));
                midStatus.UsaiLabel.Text = "ON TO THE VOTE!!!\n";
                await Task.Delay(TimeSpan.FromSeconds(3f));
                _global.CurtainAnim.Play("closeCurtain");

                break;
            case Minigame.Rig:
                _global.CurtainAnim.Play("openCurtain");
                await Task.Delay(TimeSpan.FromSeconds(1f));

                midStatus.UsaiLabel.Text = "GREAT GOOGAGLY MOOGAGLY!!!";
                await Task.Delay(TimeSpan.FromSeconds(2f));
                midStatus.UsaiLabel.Text = "I haven't seen the national debt raised that high since Steve Job's... Well, you know.";
                await Task.Delay(TimeSpan.FromSeconds(5f));
                midStatus.UsaiLabel.Text = "Moving on.";
                await Task.Delay(TimeSpan.FromSeconds(1f));
                midStatus.UsaiLabel.Text = "In all my research there is one constant between all U.S. presidents.";
                await Task.Delay(TimeSpan.FromSeconds(3f));
                //APPLAAUSE
                midStatus.UsaiLabel.Text = "KISSING BABIES!!!";
                await Task.Delay(TimeSpan.FromSeconds(4f));
                midStatus.UsaiLabel.Text = "Be careful though. He's a rowdy one. If you're caught in a picture with it crying it could mean the end for your career.\n";
                await Task.Delay(TimeSpan.FromSeconds(5f));
                _global.CurtainAnim.Play("closeCurtain");

                break;
            case Minigame.BabyKisser:
                break;
        }        


        await Task.Delay(TimeSpan.FromSeconds(1f));
        midStatus.QueueFree();

        switch (minigame)
        {
            case Minigame.Debate:
                _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Debt));
                break;
            case Minigame.Debt:
                _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.Rig));
                break;
            case Minigame.Rig:
                _signalBus.EmitSignal(nameof(Events.MinigameStart), Variant.From(Minigame.BabyKisser));
                break;
            case Minigame.BabyKisser:
                _playerManager.DisablePlayers();
                _global.CurtainAnim.Play("openCurtain");
                var endScene = _endScene.Instantiate() as EndScene;
                AddChild(endScene);
                float topScore = 0;
                Candidate loserPlayer = players[0];
                Candidate topPlayer = players[0];
                foreach (var player in players)
                {
                    if (player.TotalScore > topScore)
                    {
                        topScore = player.TotalScore;
                        topPlayer = player;
                    }
                    else
                    {
                        loserPlayer = player;
                    }
                }
                endScene.WinnerText = "Congratulations to the 2032 Presidental Elect:\n" + topPlayer.PlayerName + "!";
                switch (topPlayer.CharSelected)
                {
                    case PlayableChar.NotBiden:
                        endScene._winnerHead.FrameCoords = new Vector2I(1, endScene._winnerHead.FrameCoords.Y);
                        break;
                        case PlayableChar.NotObama:
                        endScene._winnerHead.FrameCoords = new Vector2I(2, endScene._winnerHead.FrameCoords.Y);

                        break;
                        case PlayableChar.NotTrump:
                        endScene._winnerHead.FrameCoords = new Vector2I(3, endScene._winnerHead.FrameCoords.Y);

                        break;
                        case PlayableChar.NotHillary:
                        endScene._winnerHead.FrameCoords = new Vector2I(4, endScene._winnerHead.FrameCoords.Y);
                        break;
                }
                switch (loserPlayer.CharSelected)
                {
                    case PlayableChar.NotBiden:
                        endScene._loserHead.FrameCoords = new Vector2I(1, 3);
                        break;
                    case PlayableChar.NotObama:
                        endScene._loserHead.FrameCoords = new Vector2I(0, 3);

                        break;
                    case PlayableChar.NotTrump:
                        endScene._loserHead.FrameCoords = new Vector2I(3, 3);

                        break;
                    case PlayableChar.NotHillary:
                        endScene._loserHead.FrameCoords = new Vector2I(4, 3);
                        break;
                }

                await Task.Delay(TimeSpan.FromSeconds(4f));

                break;
        }

    }
}
