using Godot;
using System;
using System.Collections.Generic;

public enum MovementDirection
{
	DOWN = 0,
	UP,
	LEFT,
	RIGHT
}

[GlobalClass]
public partial class TopDownCharacter : CharacterBody2D
{
	public MovementDirection FaceDirection { get; protected set; }
    public Vector2 CharacterSize { get; protected set; }

    protected readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

	protected Global Global { get; private set; }
	protected Events SignalBus { get; private set; }
	protected Sprite2D Sprite { get; set; }
	protected AnimationPlayer AnimPlayer { get; set; }

	protected Vector2 Direction { get; set; }
	protected float WalkSpeed { get; set; }
	protected float RunSpeed { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Character");
		SignalBus = GetNode<Events>("/root/Events");
		Global = GetNode<Global>("/root/Global");

        Sprite = GetNode<Sprite2D>("Sprite2D"); //Make sure all inherited classes' sprites & anim players are titled the same
        CharacterSize = new Vector2(Sprite.Texture.GetHeight() / Sprite.Hframes, Sprite.Texture.GetWidth() / Sprite.Vframes);

        AnimPlayer = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
    }
}
