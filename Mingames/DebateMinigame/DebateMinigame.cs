using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public partial class DebateMinigame : Node2D
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }

    private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

    private PackedScene _debateQuestion;

    private Timer _minigameTimer;
    private Label _minigameTime;

    private Timer _spawnTimer;
    private Timer _scoreTimer;

    private Sprite2D _audienceSprite;
    private AnimationPlayer _audienceAnimPlayer;

    private Vector2 _spawnTimeInterval = new Vector2(2.5f, 4.5f);
    //private Vector2 _spawnAmtInterval = new Vector2(1, 2);
    private Vector2 _spawnPosInterval = new Vector2(150, 950);
    private float _funnyQuestionOdds = 0.1f;
    private float _speedMult = 500f;


    private Dictionary<int, string> _debateQuestions = new Dictionary<int, string>();
    private Dictionary<int, string> _funnyQuestions = new Dictionary<int, string>();

    private bool _halfTimeLeft = false;
    private bool _fifteenLeft = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        _signalBus.DebatePlayerOut += OnPlayerOut;

        _debateQuestion = GD.Load<PackedScene>("res://Mingames/DebateMinigame/debate_question.tscn");

        _audienceSprite = GetNode<Sprite2D>("Audience");
        _audienceAnimPlayer = GetNode<AnimationPlayer>("AudienceAnim");
        _audienceAnimPlayer.Play("audienceCelebrate");

        _debateQuestions.Add(1, "Foreign Policy?");
        _debateQuestions.Add(2, "National Debt??");
        _debateQuestions.Add(3, "Taxes???");
        _debateQuestions.Add(4, "What is your plan with the\nTexan-American War?");
        _debateQuestions.Add(5, "Immigration?");
        _debateQuestions.Add(6, "How Should the Government\nHelp Mental Health?");
        _debateQuestions.Add(7, "You've Been Reported to have\nReceived Donations from...");
        _debateQuestions.Add(8, "Education?");

        _funnyQuestions.Add(1, "Why does Florida Look Similar to Italy??");
        _funnyQuestions.Add(2, "Should New California Citizen's be Given Rights?");
        _funnyQuestions.Add(3, "How to Deal with Nikocado Avocado?????");
        for (int i = 0; i < _global.Players.Count; i++)
        {
            if (i == 0)
            {
                _funnyQuestions.Add(4, "Does " + _global.Players[i].PlayerName + "\nstill pee their bed?");
            }
            if (i == 1)
            {
                _funnyQuestions.Add(5, "How does " + _global.Players[i].PlayerName + "\nfeel about the rising popularity of\nbody pillows of them being sold?");
            }
            if (i == 2)
            {
                _funnyQuestions.Add(6, "At what point will " + _global.Players[i].PlayerName + "\nfinally admit to their undying love for Monopoly?");
            }
            else
            {
                _funnyQuestions.Add(7, "Did " + _global.Players[i].PlayerName + "\nreally encourage public nudity\nat their last pep rally?");
            }
        }

        _minigameTime = GetNode<Label>("GameTime");

        _minigameTimer = GetNode<Timer>("GameTimer");
        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _scoreTimer = GetNode<Timer>("ScoreTimer");

        _minigameTimer.Timeout += OnMinigameTimerTimeout;
        _spawnTimer.Timeout += QuestionSpawner;
        _scoreTimer.Timeout += OnDebateScoreInc;


        foreach (var player in _global.Players)
        {
            var playerLabel = new Label { Text = player.PlayerName + "'s Debate Performance: 0"};
            playerLabel.Modulate = player.SuitColor;

            switch (player.PlayerNum)
            {
                case 1:
                    playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    playerLabel.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case 2:
                    playerLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    playerLabel.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case 3:
                    playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    playerLabel.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case 4:
                    playerLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    playerLabel.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    //error msg
                    break;
            }
            playerLabel.Name = "playerLabel" + player.PlayerNum;
            AddChild(playerLabel);
        }



        QuestionSpawner();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _minigameTime.Text = "Debate Ends in:\n" + (int)_minigameTimer.TimeLeft + " Seconds!";

        if (!_halfTimeLeft && _minigameTimer.TimeLeft != 0 && _minigameTimer.TimeLeft < _minigameTimer.WaitTime/2)
        {
            GD.Print("half time reached: " + _minigameTimer.TimeLeft);
            _halfTimeLeft = true;
            _spawnTimeInterval = new Vector2(1.5f, 3f);
            _funnyQuestionOdds = 0.3f;
            _speedMult = 750f;
        }
        if (!_fifteenLeft && _minigameTimer.TimeLeft != 0 && _minigameTimer.TimeLeft < 15)
        {
            GD.Print("15 time reached: " + _minigameTimer.TimeLeft);

            _fifteenLeft = true;
            _spawnTimeInterval = new Vector2(0.75f, 2f);
            _funnyQuestionOdds = 0.5f;
            _speedMult = 1000f;
        }
    }

    #region SIGNAL_LISTENERS
    private void OnMinigameTimerTimeout()
    {
        EndMinigame();
    }

    private void OnPlayerOut(int playerNum)
    {
        int alivePlayers = 0;
        foreach (var player in _global.Players)
        {
            if (player.IsAlive)
            {
                alivePlayers++;
            }
        }
        if (alivePlayers <= 1)
        {
            EndMinigame();
        }
    }

    private void OnDebateScoreInc()
    {
        foreach (var player in _global.Players)
        {
            if (player.IsAlive)
            {
                player.DebateScore += 1;
                GetNode<Label>("playerLabel" + player.PlayerNum).Text = player.PlayerName + "'s Debate Performance: " + player.DebateScore;
            }
        }
    }

    #endregion
    #region HELPER_FUNCTIONS
    public void QuestionSpawner()
    {
        GD.Print("spawn question! Timer wait time = " + _spawnTimer.WaitTime);
        int questionId;
        string questionText;
        var funnyDub = Rnd.NextDouble();
        if (funnyDub <= _funnyQuestionOdds)
        {
            questionId = Rnd.Next(1, _funnyQuestions.Count);
            questionText = _funnyQuestions[questionId];
        }
        else
        {
            questionId = Rnd.Next(1, _debateQuestions.Count);
            questionText = _debateQuestions[questionId];
        }
        var pos = Rnd.Next((int)_spawnPosInterval.X, (int)_spawnPosInterval.Y + 1);

        SpawnQuestion(questionText, pos);

        var timeDub = Rnd.NextDouble();
        _spawnTimer.WaitTime = timeDub * (_spawnTimeInterval.Y - _spawnTimeInterval.X) + _spawnTimeInterval.X; //Generates double within a range
        _spawnTimer.Start();
    }
    public void SpawnQuestion(string text, int x)
    {
        DebateQuestion debateQuestion = _debateQuestion.Instantiate() as DebateQuestion;
        debateQuestion.SetQuestionAttr(_speedMult, text);
        AddChild(debateQuestion); //put in tree
        debateQuestion.Position = new Vector2(x, 0);
    }

    private void EndMinigame()
    {
        //EXIT ANIM
        QueueFree(); 
        _signalBus.EmitSignal(nameof(Events.MinigameOver), Variant.From(Minigame.Debate));
    }

    #endregion


}
