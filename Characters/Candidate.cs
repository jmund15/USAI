using Godot;
using System;
using static Events;

[GlobalClass]
public partial class Candidate : CharacterBody2D
{
    #region CLASS_VARIABLES
    public int PlayerNum { get; set; }
    public string PlayerName { get; set; }
    public PlayableChar CharSelected { get; set; }
    public string CharSelectedName { get; set; }
    public Color SuitColor { get; set; }

    private string _leftInAction;
    private string _rightInAction;
    private string _upInAction;
    private string _downInAction;
    private string _aInAction;
    private string _bInAction;

    public Vector2 CharacterSize { get; protected set; }

    protected readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());

    private Global _global { get; set; }
    private Events _signalBus { get; set; }
    private Sprite2D _charSprite { get; set; }
    private Sprite2D _suitSprite { get; set; }
    private Area2D _hitboxArea { get; set; }
    private AnimationPlayer _animPlayer { get; set; }

    private Node2D _playerDebtGame { get; set; }
    private AnimationPlayer _debtAnimPlayer { get; set; }
    private Timer _debtStunTimer { get; set; }
    public bool IsRaisingDebt { get; private set; } = false;
    public bool IsDebtStunned { get; private set; } = false;

    private Node2D _playerRigGame;
    private AnimationPlayer _rigAnimPlayer;
    private Area2D _rigHitBoxArea;
    private bool _hitBallotRight = true;

    private Node2D _playerBabyGame;
    private AnimationPlayer _babyAnimPlayer;
    public Sprite2D BabySelectUI;
    public bool PerparingCatch = false;
    public bool HoldingBaby = false;
    public bool OutBabyGame = false;

    public Vector2 Direction { get; set; }
    public float WalkSpeed { get; set; } = 7000f;
    public float RunSpeed { get; set; } = 10000f;
    public float JumpVelocity { get; set; } = -400.0f;
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public float TotalScore { get; set; } = 0;
    public int DebateScore { get; set; } = 0;
    public float DebtScore { get; set; } = 0;
    public float RigScore { get; set; } = 0;   
    public float BabyScore { get; set; } = 0;
    public bool IsAlive { get; set; } = true;

    #endregion

    #region BASE_GODOT_OVERRIDEN_FUNCTIONS
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("PLAYER INIT,\nNUM: " + PlayerNum + "\nNAME: " + PlayerName);
        AddToGroup("Candidate");
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        _signalBus.MinigameStart += OnMinigameStart;
        _signalBus.MinigameOver += OnMinigameOver;

        _charSprite = GetNode<Sprite2D>("Char");
        _suitSprite = GetNode<Sprite2D>("Suit");
        _hitboxArea = GetNode<Area2D>("HitBoxArea");
        _hitboxArea.AreaEntered += OnAreaEnteredHitBoxArea;

        //CharacterSize = new Vector2(Sprite.Texture.GetHeight() / Sprite.Hframes, Sprite.Texture.GetWidth() / Sprite.Vframes);

        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animPlayer.AnimationFinished += OnAnimationFinished;

        _playerDebtGame = GetNode<Node2D>("PlayerDebtGame");
        _debtAnimPlayer = _playerDebtGame.GetNode<AnimationPlayer>("AnimationPlayer");
        _debtStunTimer = _playerDebtGame.GetNode<Timer>("StunTimer");
        _debtStunTimer.Timeout += OnDebtStunTimeout;
        _debtAnimPlayer.AnimationFinished += OnAnimationFinished;
        _playerDebtGame.Hide();

        _playerRigGame = GetNode<Node2D>("PlayerRigGame");
        _rigAnimPlayer = _playerRigGame.GetNode<AnimationPlayer>("AnimationPlayer");
        _rigAnimPlayer.AnimationFinished += OnAnimationFinished;
        _rigHitBoxArea = _playerRigGame.GetNode<Area2D>("HandHitBox");
        _rigHitBoxArea.Monitoring = false;
        _rigHitBoxArea.Monitorable = false;
        _rigHitBoxArea.AreaEntered += OnAreaEnteredHitBoxArea;
        _playerRigGame.Hide();

        _playerBabyGame = GetNode<Node2D>("PlayerBabyGame");
        _babyAnimPlayer = _playerBabyGame.GetNode<AnimationPlayer>("AnimationPlayer");
        BabySelectUI = _playerBabyGame.GetNode<Sprite2D>("Select");
        _playerBabyGame.Hide();

        //_signalBus.DebtPlayerStunned += OnDebtPlayerStunned;

        _suitSprite.Modulate = new Color(255,255,255,1);
        _suitSprite.Modulate = SuitColor;
        _playerDebtGame.GetNode<Sprite2D>("Suit").Modulate = SuitColor;
        _playerRigGame.GetNode<Sprite2D>("Arm").Modulate = SuitColor;
        _playerBabyGame.GetNode<Sprite2D>("Suit").Modulate = SuitColor;

        switch (CharSelected)
        {
            case PlayableChar.NotBiden:
                _charSprite.FrameCoords = new Vector2I(0, 1);
                _playerDebtGame.GetNode<Sprite2D>("Char").FrameCoords = new Vector2I(0, 2);
                CharSelectedName = "NotBiden";
                break;
            case PlayableChar.NotObama:
                _charSprite.FrameCoords = new Vector2I(0, 2);
                _playerDebtGame.GetNode<Sprite2D>("Char").FrameCoords = new Vector2I(0, 3);
                CharSelectedName = "NotObama";
                break;
            case PlayableChar.NotTrump:
                _charSprite.FrameCoords = new Vector2I(0, 3);
                _playerDebtGame.GetNode<Sprite2D>("Char").FrameCoords = new Vector2I(0, 4);
                CharSelectedName = "NotTrump";
                break;
            case PlayableChar.NotHillary:
                _charSprite.FrameCoords = new Vector2I(0, 4);
                _playerDebtGame.GetNode<Sprite2D>("Char").FrameCoords = new Vector2I(0, 5);
                CharSelectedName = "NotHillary";
                break;
        }
    }

    public override void _Process(double delta)
    {
        switch (_global.CurrentMinigame)
        {
            case Minigame.Cutscene:
                break;
            case Minigame.Debate:
                if (IsAlive)
                {
                    // nothing?
                }
                break;
            case Minigame.Debt:
                if (!IsRaisingDebt)
                {
                    if (IsDebtStunned)
                    {
                        DebtScore -= 8 * (float)delta;
                    }
                    else
                    {
                        DebtScore -= 3 * (float)delta;
                    }
                    DebtScore = Mathf.Max(0, DebtScore);
                }
                break;
            case Minigame.BabyKisser:
                if (HoldingBaby)
                {
                    BabyScore += 100 * (float)delta;

                    if (_babyAnimPlayer.CurrentAnimationPosition >= 0.2f && _babyAnimPlayer.CurrentAnimation.Contains("throw"))
                    {
                        _signalBus.EmitSignal(nameof(Events.ThrowBaby));
                        HoldingBaby = false;
                    }
                    if (Input.IsActionJustPressed(_leftInAction))
                    {
                        _signalBus.EmitSignal(nameof(Events.ThrowSelectChange), false);
                    }
                    else if (Input.IsActionJustPressed(_rightInAction))
                    {
                        _signalBus.EmitSignal(nameof(Events.ThrowSelectChange), true);
                    }
                }
                break;
            default: break; 
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        HumanMovementController(delta);
        HumanAnimationController();
    }
#endregion

    #region MOVEMENT_AND_ANIMATION_CONTROLLERS
    private void HumanMovementController(double delta)
    {
        Vector2 velocity = Velocity;
        switch (_global.CurrentMinigame)
        {
            case Minigame.Cutscene:
                return;
            case Minigame.Debate:
                Direction = Input.GetVector(_leftInAction, _rightInAction, _upInAction, _downInAction).Normalized();
                velocity.X = Direction.X * RunSpeed * (float)delta;

                // Add the gravity.
                if (!IsOnFloor())
                {
                    velocity.Y += gravity * (float)delta;
                }

                // Handle Jump.
                if (Input.IsActionJustPressed(_aInAction) && IsOnFloor())
                {
                    velocity.Y = JumpVelocity;
                }
                break;
            case Minigame.Debt:
                velocity = Vector2.Zero;
                break; // NO MOVEMENT
            case Minigame.BabyKisser:
                velocity = Vector2.Zero;
                break;
            case Minigame.Rig:
                velocity = Vector2.Zero;
                break;
            default: break;
        }
        Velocity = velocity;
        MoveAndSlide();
    }
    private void HumanAnimationController()
    {
        switch (_global.CurrentMinigame)
        {
            case Minigame.Cutscene:
                return;
            case Minigame.Debate:
                if (Velocity.Length() == 0)
                {
                    _animPlayer.Play("idleRight" + CharSelectedName); // change to based on curr direction!
                }
                if (Velocity.X < 0)
                {
                    _animPlayer.Play("runRight" + CharSelectedName); // CHANGE LATER
                }
                else if (Velocity.X > 0)
                {
                    _animPlayer.Play("runRight" + CharSelectedName);
                }
                break;
            case Minigame.Debt:
                if (Input.IsActionJustPressed(_aInAction) && !IsDebtStunned)
                {
                    _debtAnimPlayer.Play("press" + CharSelectedName);
                    IsRaisingDebt = true;
                }
                break;
            case Minigame.Rig:
                if (!_rigAnimPlayer.IsPlaying() && Input.IsActionJustPressed(_rightInAction))
                {
                    _rigAnimPlayer.Play("swatRight" + CharSelectedName);
                    _rigHitBoxArea.Monitoring = true;
                    _rigHitBoxArea.Monitorable = true;
                    _hitBallotRight = true;
                }
                else if (!_rigAnimPlayer.IsPlaying() && Input.IsActionJustPressed(_leftInAction))
                {
                    _rigAnimPlayer.Play("swatLeft" + CharSelectedName);
                    _rigHitBoxArea.Monitoring = true;
                    _rigHitBoxArea.Monitorable = true;
                    _hitBallotRight = false;
                }
                break;
            case Minigame.BabyKisser:
                if (Input.IsActionJustPressed(_aInAction) && !PerparingCatch && !OutBabyGame)
                {
                    _babyAnimPlayer.Play("throw" + CharSelectedName);
                }
                break;
            default: break;
        }
    }
    #endregion

    #region SIGNAL_LISTENERS
    private void OnMinigameStart(Minigame minigame)
    {
        switch (minigame)
        {
            case Minigame.Cutscene:
                break;
            case Minigame.Debate:
                _charSprite.Show();
                _suitSprite.Show();
                _hitboxArea.Monitorable = true;
                _hitboxArea.Monitoring = true;
                _playerDebtGame.Hide();
                _playerRigGame.Hide();
                _playerBabyGame.Hide();
                break;
            case Minigame.Debt:
                _charSprite.Hide();
                _suitSprite.Hide();
                _hitboxArea.Monitorable = false;
                _hitboxArea.Monitoring = false;
                _playerDebtGame.Show();
                _playerRigGame.Hide();
                _playerBabyGame.Hide();
                break;
            case Minigame.Rig:
                _charSprite.Hide();
                _suitSprite.Hide();
                _hitboxArea.Monitorable = false;
                _hitboxArea.Monitoring = false;
                _playerDebtGame.Hide();
                _playerRigGame.Show();
                _playerBabyGame.Hide();
                break;
            case Minigame.BabyKisser:
                _charSprite.Hide();
                _suitSprite.Hide();
                _hitboxArea.Monitorable = false;
                _hitboxArea.Monitoring = false;
                _playerDebtGame.Hide();
                _playerRigGame.Hide();
                _playerBabyGame.Show();
                break;
            default:
                break;
        }
    }
    private void OnMinigameOver(Minigame minigame)
    {
        throw new NotImplementedException();
    }
    public void OnDebtPlayerStunned()
    {
        IsDebtStunned = true;
        _playerDebtGame.GetNode<Sprite2D>("Podium").Modulate = new Color("f926ff");
        _debtStunTimer.Start();
    }
    private void OnDebtStunTimeout()
    {
        IsDebtStunned = false;
        _playerDebtGame.GetNode<Sprite2D>("Podium").Modulate = new Color("ffffff");
    }
    private void OnAreaEnteredHitBoxArea(Area2D area)
    {
        if (area.CollisionLayer == 16) // DEBATE QUESTION
        {
            Hide();
            ProcessMode = ProcessModeEnum.Disabled;
            IsAlive = false;
            // "death anim"
            GD.Print("PLAYER HIT QUESTION");
            area.GetParent().QueueFree(); // animation later?
            _signalBus.EmitSignal(nameof(Events.DebatePlayerOut), PlayerNum);
        }
        else if (area.CollisionLayer == 32) // BALLOT
        {
            Ballot ballot = area.GetParent() as Ballot;
            ballot.BallotHit(_hitBallotRight);
        }
    }
    private void OnAnimationFinished(StringName animName)
    {
        if (animName.ToString().Contains("press"))
        {
            IsRaisingDebt = false;
            DebtScore += 2;
        }
        if (animName.ToString().Contains("swat"))
        {
            _rigHitBoxArea.Monitoring = false;
            _rigHitBoxArea.Monitorable = false;
        }
    }

    #endregion
    #region HELPER_FUNCITONS

    public void SetInputActions(string left, string right, string up, string down, string a, string b)
    {
        _leftInAction = left;
        _rightInAction = right;
        _upInAction = up;
        _downInAction = down;
        _aInAction = a;
        _bInAction = b;
    }
    public void PrepareCatchBaby()
    {
        _babyAnimPlayer.Play("hold" + CharSelectedName);
        PerparingCatch = true;
    }
    public void CatchBaby()
    {
        _babyAnimPlayer.Active = true;
        PerparingCatch = false;
        HoldingBaby = true;
    }
    private Vector2 GetRandomDirection()
    {
        var direction = new Vector2();
        var randNeg = _rnd.Next(0, 4);
        switch (randNeg)
        {
            case 0:
                direction = new Vector2(_rnd.NextSingle(), _rnd.NextSingle()); //(MovementDirection)Rnd.Next(0, 4);
                break;
            case 1:
                direction = new Vector2(-_rnd.NextSingle(), _rnd.NextSingle());
                break;
            case 2:
                direction = new Vector2(_rnd.NextSingle(), -_rnd.NextSingle());
                break;
            case 3:
                direction = new Vector2(-_rnd.NextSingle(), -_rnd.NextSingle());
                break;
            default:
                break;
        }
        return direction;
    }
    #endregion
}
