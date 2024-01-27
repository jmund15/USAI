using Godot;
using System;
using System.Diagnostics.Tracing;
using static Godot.Camera3D;
using static Godot.TextServer;

public enum ProjectileType
{
	NULL = -1,
	ENERGY_BALL
}
public partial class ProjectileManager : Node2D
{
	private Events _signalBus;
	private PackedScene _projectileEnergy = ResourceLoader.Load("res://Objects/Projectiles/projectile_energyball.tscn") as PackedScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _signalBus = GetNode<Events>("/root/Events");
		_signalBus.ProjectileFire += OnProjectileFire;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnProjectileFire(ProjectileType projectileType, Vector2 startingPosition, Vector2 direction)
	{
		switch (projectileType)
		{
			case ProjectileType.ENERGY_BALL:
				var spawnedProj = _projectileEnergy.Instantiate() as Projectile;
				spawnedProj.StartingPos = startingPosition;
				spawnedProj.ProjDir = direction;
				AddChild(spawnedProj);
				break;
			default:
				GD.Print("invalid spawned proj type: " + projectileType);
				break;
		}
	}
}
