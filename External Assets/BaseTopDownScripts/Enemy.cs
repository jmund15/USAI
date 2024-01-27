using Godot;
using System;

public partial class Enemy : TopDownCharacter
{
    private enum ScientistState
    {
        IDLE,
        FOCUS_TARGET,
        FLEE_TARGET,
    }

    private ScientistState _state = ScientistState.IDLE;

    private Node2D _target;
    private Player _player;

    private Area2D _detectTargetRangeArea;
	private Timer _idleMovementTimer;
	private Timer _shootCooldownTimer;

    private Vector2 _idleDirection;

	private bool _hasTarget = false;
    private float _distanceFromTarget;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();

        WalkSpeed = 3000;
        RunSpeed = 5000;

        AddToGroup("Enemy");

        _detectTargetRangeArea = GetNode<Area2D>("DetectTargetRange");
        _idleMovementTimer = GetNode<Timer>("TimerIdleMovement");
		_shootCooldownTimer = GetNode<Timer>("TimerShotCooldown");

        SignalBus.EnemyDetectPlayer += OnCharacterDetectPlayer;
        _detectTargetRangeArea.BodyEntered += OnDetectTargetRangeBodyEntered;
        _detectTargetRangeArea.BodyExited += OnDetectTargetRangeBodyExited;
        _idleMovementTimer.Timeout += OnIdleMovementTimerTimeout;
        _shootCooldownTimer.Timeout += OnShootCooldownTimerTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		FaceDirection = (MovementDirection)Sprite.FrameCoords.X;
        switch (_state)
        {
            case ScientistState.IDLE:
                break;
            case ScientistState.FOCUS_TARGET:
                _distanceFromTarget = (float)Math.Sqrt( Math.Pow(Position.X - _target.Position.X, 2) + Math.Pow(Position.Y - _target.Position.Y, 2) );
                break;
            case ScientistState.FLEE_TARGET:
                _distanceFromTarget = (float)Math.Sqrt(Math.Pow(Position.X - _target.Position.X, 2) + Math.Pow(Position.Y - _target.Position.Y, 2));
                break;
        }
	}
    public override void _PhysicsProcess(double delta)
    {
        switch (_state)
        {
            case ScientistState.IDLE:
                IdleMovementController(delta);
                IdleAnimationController();
                break;
            case ScientistState.FOCUS_TARGET:
                FocusTargetMovementController(delta);
                FocusTargetAnimationController();
                break;
            case ScientistState.FLEE_TARGET:
                FleeTargetMovementController(delta);
                FleeTargetAnimationController();
                break;
        }
    }
    #region MOVEMENT_AND_ANIMATION_CONTROLLERS
    private void IdleMovementController(double delta)
    {
        //switch (_idleMoveDir)
        //{
        //    case MovementDirection.STATIONARY:
        //        Direction = Vector2.Zero; break;
        //    case MovementDirection.DOWN: 
        //        Direction = Vector2.Down; break;
        //    case MovementDirection.UP: 
        //        Direction = Vector2.Up; break;
        //    case MovementDirection.LEFT: 
        //        Direction = Vector2.Left; break;  
        //    case MovementDirection.RIGHT: 
        //        Direction = Vector2.Right; break;
        //    default:
        //        GD.PrintErr("Scientist Direction: " + _idleMoveDir); break;
        //}
        Velocity = _idleDirection * WalkSpeed * (float)delta;
        MoveAndSlide();
    }
    private void FocusTargetMovementController(double delta)
    {
        Direction = Vector2.Zero;
        Velocity = Direction * WalkSpeed * (float)delta;
        MoveAndSlide();
    }
    private void FleeTargetMovementController(double delta)
    {
        Direction = (Position - _target.Position).Normalized();
        Velocity = Direction * RunSpeed * (float)delta;
        MoveAndSlide();
    }
    private void IdleAnimationController()
    {
        if (Velocity.Length() == 0)
        {
            //TODO: IN THE FUTURE MAKE THE NEXT STATEMENTS ELSE IF AND ENABLE SOME FACE ANIMATIONS BASED ON "FACEDIRECTION" (i.e. "faceLeft")
            AnimPlayer.Stop();
        }
        if (Velocity.X < 0 && (Math.Abs(Direction.X) - Math.Abs(Direction.Y)) >= 0)
        {
            AnimPlayer.Play("walkLeft");
        }
        else if (Velocity.X > 0 && (Math.Abs(Direction.X) - Math.Abs(Direction.Y)) >= 0)
        {
            AnimPlayer.Play("walkRight");
        }
        else if (Velocity.Y < 0)
        {
            AnimPlayer.Play("walkUp");
        }
        else
        {
            AnimPlayer.Play("walkDown");
        }
    }
    private void FocusTargetAnimationController() 
    {
        if (Velocity.Length() == 0)
        {
            //TODO: IN THE FUTURE MAKE THE NEXT STATEMENTS ELSE IF AND ENABLE SOME FACE ANIMATIONS BASED ON "FACEDIRECTION" (i.e. "faceLeft")
            AnimPlayer.Stop();
        }
        float diffX = Position.X - _player.Position.X;
        float diffY = Position.Y - _player.Position.Y;
        if (diffX > 0 && Math.Abs(diffX) > Math.Abs(diffY))
        {
            AnimPlayer.Play("walkLeft");
        }
        else if (diffX < 0 && Math.Abs(diffX) > Math.Abs(diffY))
        {
            AnimPlayer.Play("walkRight");
        }
        else if (diffY > 0)
        {
            AnimPlayer.Play("walkUp");
        }
        else
        {
            AnimPlayer.Play("walkDown");
        }
    }
    private void FleeTargetAnimationController()
    {
        if (Velocity.Length() == 0)
        {
            //TODO: IN THE FUTURE MAKE THE NEXT STATEMENTS ELSE IF AND ENABLE SOME FACE ANIMATIONS BASED ON "FACEDIRECTION" (i.e. "faceLeft")
            AnimPlayer.Stop();
        }
        if (Velocity.X < 0 && (Math.Abs(Direction.X) - Math.Abs(Direction.Y)) >= 0)
        {
            AnimPlayer.Play("walkLeft");
        }
        else if (Velocity.X > 0 && (Math.Abs(Direction.X) - Math.Abs(Direction.Y)) >= 0)
        {
            AnimPlayer.Play("walkRight");
        }
        else if (Velocity.Y < 0)
        {
            AnimPlayer.Play("walkUp");
        }
        else
        {
            AnimPlayer.Play("walkDown");
        }
    }
    #endregion

    #region SIGNAL_LISTENERS
    private void OnCharacterDetectPlayer(TopDownCharacter body)
    {
        if (body == this)
        {

        }
        else
        {

        }
    }
    private void OnDetectTargetRangeBodyEntered(Node2D body)
    {
        if (body == this)
        {
            return; // don't use self
        }
        if (body is TileMap)
        {
            return; //This seems good, probably anything tilemapped isn't interactable
        }
        PhysicsBody2D physicsBody = body as PhysicsBody2D; if (physicsBody == null) { GD.PrintErr(nameof(OnDetectTargetRangeBodyEntered) + " ERROR || " + "body isn't tilemap or phsyics body????"); return; } // should be impossible

        if (physicsBody is Player)
        {
            GD.Print("found Player");
            _target = physicsBody;
            _player = physicsBody as Player;

            if (false) // condition here
            {
                _state = ScientistState.FLEE_TARGET;
            }
            else
            {
                _state = ScientistState.FOCUS_TARGET;
                _shootCooldownTimer.Start();
            }
            SignalBus.EmitSignal(nameof(Events.EnemyDetectPlayer), this);
        }
    }
    private void OnDetectTargetRangeBodyExited(Node2D body)
    {
        if (body == this)
        {
            return;
        }
        if (body is TileMap)
        {
            return; //This seems good, probably anything tilemapped isn't interactable
        }
        PhysicsBody2D physicsBody = body as PhysicsBody2D; if (physicsBody == null) {GD.PrintErr(nameof(OnDetectTargetRangeBodyExited) + " ERROR || " + "body isn't tilemap or phsyics body????"); return; } // should be impossible

        if (physicsBody is Player)
        {
            GD.Print("lost player");
            _state = ScientistState.IDLE;
        }
    }
    private void OnIdleMovementTimerTimeout()
    {
        var rDouble = Rnd.NextDouble();
        float upperB = 1.25f; float lowerB = 0.5f;
        _idleMovementTimer.WaitTime = rDouble * (upperB - lowerB) + lowerB; //Generates double within a range (0.5, 1.25)
        _idleDirection = new Vector2((float)Rnd.NextDouble(), (float)Rnd.NextDouble());
    }
    private void OnShootCooldownTimerTimeout()
    {
        var projDir = (_target.GlobalPosition - GlobalPosition).Normalized();
        var offset = projDir * CharacterSize;
        var projStartingPos = GlobalPosition + offset;

        SignalBus.EmitSignal(nameof(Events.ProjectileFire), Variant.From(ProjectileType.ENERGY_BALL), projStartingPos, projDir);
        GD.Print("Scientist shooting");

        if (_state != ScientistState.FOCUS_TARGET)
        {
            _shootCooldownTimer.Stop();
        }
    }
    #endregion
}
