using Godot;
using System;

public partial class Projectile : RigidBody2D
{
    protected Global Global { get; private set; }
    protected Events SignalBus { get; private set; }

    public float ProjSpeed { get; protected set; }
    public float ProjPower { get; protected set; }

	public Vector2 StartingPos { get; set; }
	public Vector2 ProjDir { get; set; }

	protected bool StopMoving { get; set; } = false;

	protected Sprite2D Sprite { get; set; }
    protected AnimationPlayer AnimPlayer { get; set; }
    protected CollisionShape2D CollisionShape { get; set; }
    protected Timer TimeoutTimer { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        SignalBus = GetNode<Events>("/root/Events");
        Global = GetNode<Global>("/root/Global");

		Sprite = GetNode<Sprite2D>("ProjectileSprite");
        AnimPlayer = GetNode<AnimationPlayer>("ProjectileSprite/AnimationPlayer");
		CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		TimeoutTimer = GetNode<Timer>("ProjTimeout");

		GlobalPosition = StartingPos;
		LookAt(ProjDir);

		AnimPlayer.Play("projectileAnim");
		TimeoutTimer.Timeout += OnTimeout;

		BodyEntered += OnProjectileHit;

		ApplyCentralImpulse(ProjDir * ProjSpeed);
	}
	public override void _Process(double delta)
	{
	}
	public override void _PhysicsProcess(double delta)
	{
		if (!Freeze)
		{
			ProjectileMovement(delta);
		}
	}
	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (StopMoving)
		{
			StopMoving = false;
			LinearVelocity = Vector2.Zero;
			AngularVelocity = 0;
			CollisionShape.SetDeferred("disabled", true);
			SetDeferred("freeze", true);
		}
	}

	protected virtual void ProjectileMovement(double delta)
	{
		//add tracking stuff here later
	}

	protected virtual void OnProjectileHit(Node body)
	{
		if (body.IsInGroup("Robber")) // maybe use "is Player" later?
		{
			//if (!Global.PlayerIsMonster)
			//{

                StopMoving = true;
                DestroyProj();
            //}
		}
	}
	protected virtual void OnTimeout()
	{
		StopMoving = true;
		DestroyProj();
	}

	protected virtual void DestroyProj()
	{
		AnimPlayer.Stop();
		Tween destoryTween = CreateTween();
		destoryTween.TweenCallback(Callable.From(() => AnimPlayer.Stop())); 
		destoryTween.TweenProperty(this, "scale", Vector2.Zero, 0.2).SetEase(Tween.EaseType.In);
		destoryTween.TweenCallback(Callable.From(QueueFree));	
	}
}
