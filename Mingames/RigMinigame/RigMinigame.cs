using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class RigMinigame : Node2D
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }

    private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

    private PackedScene _votingScene;
    private PackedScene _ballot;

    private List<string> _ballotNames = new List<string>();

    private Timer _minigameTimer;
    private Label _minigameTime;

    private int _currentPlayerToSpawnBallot = 1;
    private Vector2 _spawnTimeInterval = new Vector2(3f, 9f);
    private Vector2 _ballotSpeedInterval = new Vector2(100f, 250f);

    int _xBasePlacement = -75;
    int _xPlacementInterval = 275;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        GD.Print("start rig minigame");
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");
        
        _votingScene = GD.Load<PackedScene>("res://Mingames/RigMinigame/voting_scene.tscn");
        _ballot = GD.Load<PackedScene>("res://Mingames/RigMinigame/ballot.tscn");

        _minigameTimer = GetNode<Timer>("GameTimer");
        _minigameTimer.Timeout += OnMinigameTimeout;
        _minigameTime = GetNode<Label>("GameTime");

        int xPlacement;
        foreach (var player in _global.Players)
        {
            xPlacement = _xBasePlacement + _xPlacementInterval * player.PlayerNum;

            var votingScene = _votingScene.Instantiate() as Node2D;
            votingScene.Name = "VotingScene" + player.PlayerNum;
            votingScene.Position = new Vector2(xPlacement, 0);
            votingScene.GetNode<Area2D>("VotingBoxHitBox").AreaEntered += (area2D) => OnBallotReachBox(area2D, player.PlayerNum);
            AddChild(votingScene);

            var playerLabel = new Label { Text = player.PlayerName + "'s Votes: 0" };
            playerLabel.ZIndex = 3;
            playerLabel.HorizontalAlignment = HorizontalAlignment.Center;
            playerLabel.SetAnchorsPreset(Control.LayoutPreset.BottomLeft);
            //playerLabel.Modulate = Colors.White;
            playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
            playerLabel.VerticalAlignment = VerticalAlignment.Bottom;
            playerLabel.Position = new Vector2(xPlacement - _xPlacementInterval / 2f, 0);
            playerLabel.Name = "playerLabel" + player.PlayerNum;
            playerLabel.LabelSettings = new LabelSettings() { FontSize = 32, FontColor = Colors.White };
           
            AddChild(playerLabel);

            var styleBox = new StyleBoxFlat();
            styleBox.BgColor = new Color("19191996");
            playerLabel.AddThemeStyleboxOverride("normal", styleBox);

            _ballotNames.Add(player.PlayerName);
        }
        for (int i = 0; i < 2; i++)
        {
            var spawnBallotTimer = new Timer();
            spawnBallotTimer.Name = "SpawnBallotTimer" + i;
            spawnBallotTimer.Timeout += () => OnBallotSpawnTimeout(spawnBallotTimer);
            AddChild(spawnBallotTimer);
            var timeDub = Rnd.NextDouble();
            spawnBallotTimer.WaitTime = timeDub * (_spawnTimeInterval.Y - _spawnTimeInterval.X) + _spawnTimeInterval.X; //Generates double within a range
            spawnBallotTimer.Start();
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        _minigameTime.Text = "Voting Closes in:\n" + (int)_minigameTimer.TimeLeft + " Seconds!";
    }

    private async void OnMinigameTimeout()
    {
        float totalPoints = 0;
        foreach (var player in _global.Players)
        {
            totalPoints += player.RigScore;
        }
        foreach (var player in _global.Players)
        {
            player.TotalScore = (player.RigScore / totalPoints) * 100;
        }

        _global.CurtainAnim.Play("closeCurtain");
        await Task.Delay(TimeSpan.FromSeconds(1.5f));
        _signalBus.EmitSignal(nameof(Events.MinigameOver), Variant.From(Minigame.Rig));
        QueueFree();
    }

    private void OnBallotSpawnTimeout(Timer timer)
    {
        var ballotNameId = Rnd.Next(0, _ballotNames.Count);

        var speedDub = Rnd.NextDouble();
        var ballotSpeed = speedDub * (_ballotSpeedInterval.Y - _ballotSpeedInterval.X) + _ballotSpeedInterval.X; //Generates double within a range

        SpawnBallot(_currentPlayerToSpawnBallot, _ballotNames[ballotNameId], (float)ballotSpeed);

        var timeDub = Rnd.NextDouble();
        timer.WaitTime = timeDub * (_spawnTimeInterval.Y - _spawnTimeInterval.X) + _spawnTimeInterval.X; //Generates double within a range
        timer.Start();

        _currentPlayerToSpawnBallot = Rnd.Next(1, _global.Players.Count + 1);
    }

    private void OnBallotReachBox(Area2D area, int playerNum)
    {
        Ballot ballot = area.GetParent() as Ballot;

        foreach (var player in  _global.Players)
        {
            if (player.PlayerNum == ballot.PlayerNumPoint)
            {
                if (playerNum == ballot.PlayerNumPoint)
                {
                    player.RigScore += 250;
                }
                else
                {
                    player.RigScore += 100;
                }
                GetNode<Label>("playerLabel" + player.PlayerNum).Text = player.PlayerName + "'s Votes:\n" + player.RigScore + "K";
            }
        }

        ballot.QueueFree();
    }

    private void SpawnBallot(int playerNum, string ballotName, float ballotSpeed)
    {
        var votingSceneClaw = GetNode<Node2D>("VotingScene" + playerNum).GetNode<Node2D>("FullClaw");
        var clawAnimPlayer = GetNode<Node2D>("VotingScene" + playerNum).GetNode<AnimationPlayer>("AnimationPlayer");

        var clawTween = GetTree().CreateTween();
        clawTween.TweenProperty(votingSceneClaw, "position:y", 144, 1f);
        clawTween.TweenCallback(Callable.From(() => ClawAnimation(votingSceneClaw, clawAnimPlayer)));

        Ballot ballot = _ballot.Instantiate() as Ballot;
        ballot.BallotName = ballotName;
        ballot.BallotSpeed = ballotSpeed;

        foreach (var player in _global.Players)
        {
            if (player.PlayerName == ballotName) {
                ballot.PlayerNumPoint = player.PlayerNum;
            }
        }

        AddChild(ballot); //put in tree
        ballot.Position = new Vector2(_xBasePlacement + _xPlacementInterval * playerNum, 0);
    }

    private void ClawAnimation(Node2D claw, AnimationPlayer clawAnimPlayer)
    {
        clawAnimPlayer.Play("clawOpen");

        var clawTween = GetTree().CreateTween();
        clawTween.TweenProperty(claw, "position:y", 0, 1f);
        clawTween.TweenProperty(claw.GetNode<Node2D>("Claw"), "frame", 0, 0);
        //clawTween.TweenCallback(Callable.From(() => ClawClose(clawAnimPlayer)));
    }
    private void ClawClose(AnimationPlayer clawAnimPlayer)
    {
        clawAnimPlayer.Play("clawClose");
    }
}
