using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BabyMinigame : Node2D
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }

    private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

    private PackedScene _babySelectUI;

    private Sprite2D _babySprite;
	private AnimationPlayer _babyAnimPlayer;

	private Sprite2D _audienceSprite;
	private AnimationPlayer _audienceAnimPlayer;

	private Sprite2D _paparazziSprite;
	private AnimationPlayer _paparazziAnimPlayer;

    private Sprite2D _explodySprite;
    private AnimationPlayer _explodyAnimPlayer;

    private Timer _babyCryTimer;
	private int _currentHolderPlayerNum;
    private int _selectThrowPlayerNum;

    private List<Candidate> _playersInGame = new List<Candidate>();

    private bool _babyCrying = false;
    private bool _babyInFlight = false;

    private Vector2 _cryTimeInterval = new Vector2(10f, 20f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        _signalBus.ThrowBaby += OnThrowBaby;
        _signalBus.ThrowSelectChange += OnThrowSelectChange;

        _babySelectUI = GD.Load<PackedScene>("res://Mingames/BabyMinigame/baby_select.tscn");

        _babySprite = GetNode<Sprite2D>("Baby");
		_babyAnimPlayer = GetNode<AnimationPlayer>("BabyAnim");

        _audienceSprite = GetNode<Sprite2D>("Audience");
        _audienceAnimPlayer = GetNode<AnimationPlayer>("AudienceAnim");

        _paparazziSprite = GetNode<Sprite2D>("Paparazzi");
        _paparazziAnimPlayer = GetNode<AnimationPlayer>("PaparazziAnim");

        _explodySprite = GetNode<Sprite2D>("Explosion");
        _explodyAnimPlayer = GetNode<AnimationPlayer>("ExplosionAnim");

        _babyCryTimer = GetNode<Timer>("BabyCryTimer");
		_babyCryTimer.Timeout += OnBabyCry;

        var cryDub = Rnd.NextDouble();
        _babyCryTimer.WaitTime = cryDub * (_cryTimeInterval.Y - _cryTimeInterval.X) + _cryTimeInterval.X; //Generates double within a range
        _babyCryTimer.Start();

        foreach (var player in _global.Players)
        {
            _playersInGame.Add(player);
        }

        _currentHolderPlayerNum = Rnd.Next(0, _playersInGame.Count - 1);
        _selectThrowPlayerNum = _currentHolderPlayerNum ++;
        if (_selectThrowPlayerNum >= _playersInGame.Count)
        {
            _selectThrowPlayerNum = 0;
        }
        foreach (var player in _playersInGame)
        {
            player.BabySelectUI.Frame = 6;

            var playerLabel = new Label { Text = player.PlayerName + "'s Votes: 0" };
            playerLabel.ZIndex = 3;
            playerLabel.HorizontalAlignment = HorizontalAlignment.Center;
            playerLabel.SetAnchorsPreset(Control.LayoutPreset.BottomLeft);
            //playerLabel.Modulate = Colors.White;
            playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
            playerLabel.VerticalAlignment = VerticalAlignment.Bottom;
            playerLabel.Position = new Vector2(player.Position.X, -250);
            playerLabel.Name = "playerLabel" + player.PlayerNum;
            playerLabel.LabelSettings = new LabelSettings() { FontSize = 32, FontColor = Colors.White };

            AddChild(playerLabel);
        }
        var newHolder = _playersInGame[_currentHolderPlayerNum];
        newHolder.HoldingBaby = true;
        newHolder.BabySelectUI.Hide();
        _babySprite.Position = new Vector2(newHolder.Position.X, newHolder.Position.Y - 75);
        _paparazziSprite.Position = new Vector2(newHolder.Position.X, newHolder.Position.Y + 75);
        _paparazziAnimPlayer.Play("paparazzi");

        _playersInGame[_selectThrowPlayerNum].BabySelectUI.Frame = 7;
       
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

    private void OnBabyCaught()
    {
        _babyInFlight = false;
        if (_babyCrying)
        {
            OnResetGame(_playersInGame[_currentHolderPlayerNum]);

            foreach (var player in _playersInGame)
            {
                player.HoldingBaby = false;
            }
        }
        else
        {
            _playersInGame[_selectThrowPlayerNum].BabySelectUI.Frame = 7;
            _paparazziSprite.Show();
            _paparazziAnimPlayer.Play("paparazzi");
        }
    }
    private void OnBabyCry()
    {
        _babyCrying = true;
        _babyAnimPlayer.Play("babyCry");

        if (_babyInFlight)
        {
            return;
        }
        OnResetGame(_playersInGame[_currentHolderPlayerNum]);

        foreach (var player in _playersInGame)
        {
            player.HoldingBaby = false;
        }

    }

    //public void BabyDoneCrying()
    //{
    //    _babyAnimPlayer.Stop();

    //}

    private async void OnResetGame(Candidate playerOut)
    {
        _audienceAnimPlayer.Play("audienceCelebrate");

        _explodySprite.Position = new Vector2(playerOut.Position.X, playerOut.Position.Y);
        _explodyAnimPlayer.Play("explosion");

        await Task.Delay(TimeSpan.FromSeconds(2f));

        _audienceAnimPlayer.Play("RESET");

        _global.CurtainAnim.Play("closeCurtain");

        await Task.Delay(TimeSpan.FromSeconds(2f));

        playerOut.OutBabyGame = true;
        playerOut.HoldingBaby = false;
        playerOut.Hide();
        _playersInGame.Remove(playerOut);

        if (_playersInGame.Count == 0) {
            EndMinigame();
        }

        _babyAnimPlayer.Play("RESET");

        _currentHolderPlayerNum = Rnd.Next(0, _playersInGame.Count - 1);
        _selectThrowPlayerNum = _currentHolderPlayerNum++;
        if (_selectThrowPlayerNum >= _playersInGame.Count)
        {
            _selectThrowPlayerNum = 0;
        }

        foreach (var player in _playersInGame)
        {
            player.BabySelectUI.Frame = 6;
        }
        var newHolder = _playersInGame[_currentHolderPlayerNum];
        newHolder.HoldingBaby = true;
        newHolder.BabySelectUI.Hide();
        _babySprite.Position = new Vector2(newHolder.Position.X, newHolder.Position.Y - 75);
        _paparazziSprite.Position = new Vector2(newHolder.Position.X, newHolder.Position.Y + 75);
        _paparazziAnimPlayer.Play("paparazzi");

        _playersInGame[_selectThrowPlayerNum].BabySelectUI.Frame = 7;

        _global.CurtainAnim.Play("openCurtain");

        var cryDub = Rnd.NextDouble();
        _babyCryTimer.WaitTime = cryDub * (_cryTimeInterval.Y - _cryTimeInterval.X) + _cryTimeInterval.X; //Generates double within a range
        _babyCryTimer.Start();

        _babyCrying = false;
    }


    private void OnThrowSelectChange(bool right)
    {
        _playersInGame[_selectThrowPlayerNum].BabySelectUI.Frame = 6;

        if (right) 
        { _selectThrowPlayerNum++;
            if (_selectThrowPlayerNum == _currentHolderPlayerNum) { _selectThrowPlayerNum++; }
        } 
        else { _selectThrowPlayerNum--;
            if (_selectThrowPlayerNum == _currentHolderPlayerNum) { _selectThrowPlayerNum--; }
        }

        if (_selectThrowPlayerNum <= -1) { _selectThrowPlayerNum = _playersInGame.Count -1; }
        if (_selectThrowPlayerNum >= _playersInGame.Count) { _selectThrowPlayerNum = 0; }

        if (_selectThrowPlayerNum == _currentHolderPlayerNum) { if (right) { _selectThrowPlayerNum++; } else { _selectThrowPlayerNum--; } }

        _playersInGame[_selectThrowPlayerNum].BabySelectUI.Frame = 7;
    }

    private void OnThrowBaby()
    {
        _paparazziSprite.Hide();
        _paparazziAnimPlayer.Stop();

        Vector2 babyDestination = Vector2.Zero;
        Candidate oldBabyHolder = _playersInGame[_currentHolderPlayerNum];
        oldBabyHolder.BabySelectUI.Show();

        Candidate newBabyHolder = _playersInGame[_selectThrowPlayerNum];
        newBabyHolder.BabySelectUI.Hide();
        newBabyHolder.BabySelectUI.Frame = 6;
        _paparazziSprite.Position = new Vector2(newBabyHolder.Position.X, newBabyHolder.Position.Y + 75);
        babyDestination = new Vector2(newBabyHolder.Position.X, newBabyHolder.Position.Y - 75);
       
        var timeDub = Rnd.NextDouble();
        var throwTime = timeDub * (3 - 1.5f) + 1.5f; //Generates double within a range

        var numRotations = Rnd.Next(1, 10);

        var heightDub = Rnd.NextDouble();
        var throwHeight = heightDub * (-200 + 100) - 100; //Generates double within a range
        var halfwayDest = new Vector2(_babySprite.Position.X + (babyDestination.X - _babySprite.Position.X) / 2f, _babySprite.Position.Y + (float)throwHeight);

        var rotTween = GetTree().CreateTween();
        rotTween.TweenProperty(_babySprite, "rotation", numRotations * 2 * Math.PI, throwTime).SetEase(Tween.EaseType.InOut);
        rotTween.TweenProperty(_babySprite, "rotation", 0, 0);

        var throwTween = GetTree().CreateTween();
        throwTween.TweenProperty(_babySprite, "position", halfwayDest, throwTime / 2).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
        throwTween.TweenProperty(_babySprite, "position", babyDestination, throwTime / 2).SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.In);
        throwTween.TweenCallback(Callable.From(newBabyHolder.CatchBaby));
        throwTween.TweenCallback(Callable.From(OnBabyCaught));

        _currentHolderPlayerNum = _selectThrowPlayerNum;
        _selectThrowPlayerNum++; if (_selectThrowPlayerNum == _currentHolderPlayerNum) { _selectThrowPlayerNum++; }
        if (_selectThrowPlayerNum == 0) { _selectThrowPlayerNum = _playersInGame.Count - 1; }
        if (_selectThrowPlayerNum >= _playersInGame.Count) { _selectThrowPlayerNum = 0; }

        newBabyHolder.PrepareCatchBaby();
        _babyInFlight = true;
    }

    private void EndMinigame()
    {
        float totalPoints = 0;
        foreach (var player in _global.Players)
        {
            totalPoints += player.BabyScore;
        }
        foreach (var player in _global.Players)
        {
            player.TotalScore = (player.BabyScore / totalPoints) * 100;
        }

        QueueFree();
        _signalBus.EmitSignal(nameof(Events.MinigameOver), Variant.From(Minigame.BabyKisser));
    }
}
