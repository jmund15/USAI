using Godot;
using System;

[GlobalClass]
public partial class Candidate : CharacterBody2D
{
    #region CLASS_VARIABLES
    public int PlayerNum { get; set; }
    public string PlayerName { get; set; }
    public string CharacterSelected { get; set; }
    public Color SuitColor { get; set; }


    public Vector2 CharacterSize { get; protected set; }

    protected readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());

    private Global _global { get; set; }
    private Events _signalBus { get; set; }
    private Sprite2D _charSprite { get; set; }
    private Sprite2D _suitSprite { get; set; }
    private Area2D _hitboxArea { get; set; }
    private AnimationPlayer _animPlayer { get; set; }

    public Vector2 Direction { get; set; }
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }

    public float TotalScore { get; set; } = 0;
    public float DebateScore { get; set; } = 0;
    private float _debateScoreMultiplyer = 5;

    public bool IsAlive { get; set; } = true;

    private string _leftInput;

    #endregion

    #region BASE_GODOT_OVERRIDEN_FUNCTIONS
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("PLAYER INIT,\nNUM: " + PlayerNum + "\nNAME: " + PlayerName);
        AddToGroup("Candidate");
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        _charSprite = GetNode<Sprite2D>("Char");
        _suitSprite = GetNode<Sprite2D>("Suit");
        _hitboxArea = GetNode<Area2D>("HitBoxArea");
        _hitboxArea.AreaEntered += OnAreaEnteredHitBoxArea;

        //CharacterSize = new Vector2(Sprite.Texture.GetHeight() / Sprite.Hframes, Sprite.Texture.GetWidth() / Sprite.Vframes);

        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animPlayer.AnimationFinished += OnAnimationFinished;

        _suitSprite.Modulate = SuitColor;
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
                    DebateScore += _debateScoreMultiplyer * (float)delta;
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
                Direction = Input.GetVector("left", "right", "up", "down").Normalized();
                velocity.X = Direction.X * RunSpeed * (float)delta;
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
                    _animPlayer.Play("idleRight"); // change to based on curr direction!
                }
                if (Velocity.X < 0)
                {
                    _animPlayer.Play("runLeft");
                }
                else if (Velocity.X > 0)
                {
                    _animPlayer.Play("runRight");
                }
                break;
            default: break;
        }
        
    }
    #endregion

    #region SIGNAL_LISTENERS

    private void OnAreaEnteredHitBoxArea(Area2D area)
    {

        if (area.CollisionLayer == 5) // DEBATE QUESTION
        {
            // "death anim"
            IsAlive = false;
        }
    }
    private void OnAnimationFinished(StringName animName)
    {
        throw new NotImplementedException();
    }

    #endregion
    #region HELPER_FUNCITONS

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
