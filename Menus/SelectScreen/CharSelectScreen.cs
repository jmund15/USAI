using Godot;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public partial class CharSelectScreen : Node2D
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }
    private PlayerManager _playerManager;

	private P1Select _p1Select;
	private P2Select _p2Select;
	private P3Select _p3Select;
	private P4Select _p4Select;

	private AnimationPlayer _player1Anim;
	private AnimationPlayer _player2Anim;
	private AnimationPlayer _player3Anim;
	private AnimationPlayer _player4Anim;

    private List<bool> _controllersConnected;
	private int _numConnected = 0;

	private Timer _startTimer;
	private Label _startLabel;

	private int _playersReady = 0;

    [Signal]
    public delegate void StartGameEventHandler();
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");
        _playerManager = GetNode<PlayerManager>("/root/MainScene/PlayerManager");
        
		_p1Select = GetNode<P1Select>("CanvasLayer/P1Select");
		_p2Select = GetNode<P2Select>("CanvasLayer/P2Select");
		_p3Select = GetNode<P3Select>("CanvasLayer/P3Select");
		_p4Select = GetNode<P4Select>("CanvasLayer/P4Select");

		_player1Anim = GetNode<AnimationPlayer>("P1SelectVis/AnimationPlayer");
		_player2Anim = GetNode<AnimationPlayer>("P2SelectVis/AnimationPlayer");
		_player3Anim = GetNode<AnimationPlayer>("P3SelectVis/AnimationPlayer");
		_player4Anim = GetNode<AnimationPlayer>("P4SelectVis/AnimationPlayer");

		_player1Anim.Play("idleNotBiden");
		_player2Anim.Play("idleNotObama");
		_player3Anim.Play("idleNotTrump");
		_player4Anim.Play("idleNotHillary");

		_startTimer = GetNode<Timer>("StartTimer");
		_startLabel = GetNode<Label>("StartTime");

        _startTimer.Timeout += OnStartTimeout;

        InputManager InputMan = GetNode<InputManager>("/root/MainScene/InputManager");

        _controllersConnected = new List<bool> { false, false, false, false };

        InputMan.ControllersConnected += OnControllersConnected;
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        _playersReady = 0;
        if (_controllersConnected[0] && _p1Select.charState)
        {
            _playersReady++;
        }
        if (_controllersConnected[1] && _p2Select.charState)
        {
            _playersReady++;
        }
        if (_controllersConnected[2] && _p3Select.charState)
        {
            _playersReady++;
        }
        if (_controllersConnected[3] && _p4Select.charState)
        {
            _playersReady++;
        }

        if (_controllersConnected[0])
		{
			((Node2D)_player1Anim.GetParent()).Show();
			_player1Anim.Play("idle" + getAnimName(_p1Select.head));

            GetNode<Sprite2D>("P1SelectVis/PlayerSuit").Modulate = new Color(_p1Select.red, _p1Select.green, _p1Select.blue);
        }
		else { ((Node2D)_player1Anim.GetParent()).Hide(); }

        if (_controllersConnected[1])
        {
            ((Node2D)_player2Anim.GetParent()).Show();
            _player2Anim.Play("idle" + getAnimName(_p2Select.head));

            GetNode<Sprite2D>("P2SelectVis/PlayerSuit").Modulate = new Color(_p2Select.red, _p2Select.green, _p2Select.blue);
        }
        else { ((Node2D)_player2Anim.GetParent()).Hide(); }

        if (_controllersConnected[2])
        {
            ((Node2D)_player3Anim.GetParent()).Show();
            _player3Anim.Play("idle" + getAnimName(_p3Select.head));

            GetNode<Sprite2D>("P3SelectVis/PlayerSuit").Modulate = new Color(_p3Select.red, _p3Select.green, _p3Select.blue);
        }
        else { ((Node2D)_player3Anim.GetParent()).Hide(); }

        if (_controllersConnected[3])
        {
            ((Node2D)_player4Anim.GetParent()).Show();
            _player4Anim.Play("idle" + getAnimName(_p4Select.head));

            GetNode<Sprite2D>("P4SelectVis/PlayerSuit").Modulate = new Color(_p4Select.red, _p4Select.green, _p4Select.blue);
        }
        else { ((Node2D)_player4Anim.GetParent()).Hide(); }

        if (_numConnected > 1 && _playersReady > 1 && _numConnected == _playersReady)
		{
			if (_startTimer.TimeLeft == 0) // timer not started yetcheck
            {
				_startTimer.Start();
			} 
		}
		else if (_startTimer.TimeLeft != 0)
		{
			_startTimer.Stop();
		}

        if (_startTimer.TimeLeft != 0)
		{
			_startLabel.Show();
			_startLabel.Text = "Election Starts in:\n" + (int)_startTimer.TimeLeft + " Seconds!";
        }
		else
		{
			_startLabel.Hide();
		}
    }
    private async void OnStartTimeout()
    {
		int playerNum = 1;
        if (_controllersConnected[0] && _p1Select.charState)
		{
			Color suitCol = new Color(_p1Select.red, _p1Select.green, _p1Select.blue);
			var inputs = new List<string>()
			{
				"left" + playerNum, "right" + playerNum, "up" + playerNum, "down" + playerNum,
				"A" + playerNum, "B" + playerNum
			};
			_playerManager.AddPlayer(playerNum, _p1Select.PlayerName, (PlayableChar)_p1Select.head, suitCol, inputs);

			playerNum++;
        }
        if (_controllersConnected[1] && _p2Select.charState)
        {
            Color suitCol = new Color(_p2Select.red, _p2Select.green, _p2Select.blue);
            var inputs = new List<string>()
            {
                "left" + playerNum, "right" + playerNum, "up" + playerNum, "down" + playerNum,
                "A" + playerNum, "B" + playerNum
            };
            _playerManager.AddPlayer(playerNum, _p2Select.PlayerName, (PlayableChar)_p2Select.head, suitCol, inputs);

            playerNum++;
        }
        if (_controllersConnected[2] && _p3Select.charState)
        {
            Color suitCol = new Color(_p3Select.red, _p3Select.green, _p3Select.blue);
            var inputs = new List<string>()
            {
                "left" + playerNum, "right" + playerNum, "up" + playerNum, "down" + playerNum,
                "A" + playerNum, "B" + playerNum
            };
            _playerManager.AddPlayer(playerNum, _p3Select.PlayerName, (PlayableChar)_p3Select.head, suitCol, inputs);

            playerNum++;
        }
        if (_controllersConnected[3] && _p4Select.charState)
        {
            Color suitCol = new Color(_p4Select.red, _p4Select.green, _p4Select.blue);
            var inputs = new List<string>()
            {
                "left" + playerNum, "right" + playerNum, "up" + playerNum, "down" + playerNum,
                "A" + playerNum, "B" + playerNum
            };
            _playerManager.AddPlayer(playerNum, _p4Select.PlayerName, (PlayableChar)_p4Select.head, suitCol, inputs);

            playerNum++;
        }

        GetNode<CanvasLayer>("CanvasLayer").Hide();

		_global.CurtainAnim.Play("closeCurtain");
        await Task.Delay(TimeSpan.FromSeconds(1.5f));
        EmitSignal(SignalName.StartGame);
		QueueFree();
    }
    private void OnControllersConnected(bool p1, bool p2, bool p3, bool p4)
    {

        _controllersConnected[0] = p1;
        _controllersConnected[1] = p2;
        _controllersConnected[2] = p3;
        _controllersConnected[3] = p4;

		_numConnected = 0;
		foreach (var controller in _controllersConnected)
		{
			if (controller) { _numConnected++; }
		}
    }

	private string getAnimName(int head)
	{
		switch (head)
		{
			case 0:
				return "NotBiden";
			case 1:
				return "NotObama";
			case 2:
				return "NotTrump";
			case 3:
				return "NotHillary";
			default:
				GD.PrintErr("head doesn't match player options!!");
				return null;
		}
	}
}
