using Godot;
using System;
using System.Threading.Tasks;

public partial class DebtMinigame : Node2D
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }

    //private AudioStreamOggVorbis makeDing;

    private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

    private Sprite2D _usaiSprite;
	private AnimationPlayer _usaiAnimPlayer;
	private Timer _usaiTimer;

	public int DebtGoal { get; private set; } = 100;
	public bool UsaiPeeking { get; private set; } = false;
    private float _peekingGracePeriod = 1.5f;

    private Vector2 _timeIgnoringInterval = new Vector2(2f, 8f);
    private Vector2 _timePeekingInterval = new Vector2(1f, 4f);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("start debt minigame");
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");



        _usaiSprite = GetNode<Sprite2D>("Usai");
        _usaiAnimPlayer = GetNode<AnimationPlayer>("UsaiAnimationPlayer");
        _usaiTimer = GetNode<Timer>("UsaiTimer");

        _usaiTimer.Timeout += OnUsaiTimeout;


        foreach (var player in _global.Players)
        {
            var progBar = new ProgressBar()
            {
                Name = "progBar" + player.PlayerNum,
                MinValue = 0,
                MaxValue = 100,
                ShowPercentage = false,
                FillMode = 3,
                Step = 0.01,
                AllowLesser = false,
                Rounded = false,
                Modulate = Colors.GreenYellow,
                Size = new Vector2(25f, 100f),
                Position = new Vector2(player.Position.X, player.Position.Y - 200)
            };
            AddChild(progBar);
            GD.Print("added prog bar");
        }



        _usaiAnimPlayer.Play("usaiIgnore");
        var timeDub = Rnd.NextDouble();
        _usaiTimer.WaitTime = timeDub * (_timeIgnoringInterval.Y - _timeIgnoringInterval.X) + _timeIgnoringInterval.X; //Generates double within a range
        _usaiTimer.Start();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        foreach (var player in _global.Players)
        {
            if (player.DebtScore > DebtGoal)
            {
                EndMinigame();
            }
            GetNode<ProgressBar>("progBar" + player.PlayerNum).Value = player.DebtScore;
            if (UsaiPeeking && player.IsRaisingDebt)
            {
                player.OnDebtPlayerStunned();
            }
        }
        
	}
    private void OnUsaiTimeout()
    {
        if (UsaiPeeking)
        {
            _usaiAnimPlayer.Play("usaiIgnore");
            var timeDub = Rnd.NextDouble();
            _usaiTimer.WaitTime = timeDub * (_timeIgnoringInterval.Y - _timeIgnoringInterval.X) + _timeIgnoringInterval.X; //Generates double within a range
            UsaiPeeking = false;
            //var gracePeriodTween = GetTree().CreateTween();
            //gracePeriodTween.TweenProperty(this, "UsaiPeeking", false, _peekingGracePeriod / 2f);
        }
        else
        {
            _usaiAnimPlayer.Play("usaiPeek");
            var timeDub = Rnd.NextDouble();
            _usaiTimer.WaitTime = timeDub * (_timePeekingInterval.Y - _timePeekingInterval.X) + _timePeekingInterval.X; //Generates double within a range
            var gracePeriodTween = GetTree().CreateTween();
            gracePeriodTween.TweenProperty(this, "UsaiPeeking", true, _peekingGracePeriod);
        }
        _usaiTimer.Start();
    }

    private async void EndMinigame()
    {
        float totalPoints = 0;
        foreach (var player in _global.Players)
        {
            totalPoints += player.DebtScore;
        }
        foreach (var player in _global.Players)
        {
            player.TotalScore = (player.DebtScore / totalPoints) * 100;
        }

        _global.CurtainAnim.Play("closeCurtain");
        await Task.Delay(TimeSpan.FromSeconds(1.5f));
        _signalBus.EmitSignal(nameof(Events.MinigameOver), Variant.From(Minigame.Debt));
        QueueFree();
    }

}
