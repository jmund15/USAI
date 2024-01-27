using Godot;
using System;
public partial class Events : Node
{
    //[Signal]
    //public delegate void LoadMapEventHandler(string mapPath);
    

    [Signal]
    public delegate void EnemyDetectPlayerEventHandler(TopDownCharacter body);
    
	[Signal]
	public delegate void ProjectileFireEventHandler(ProjectileType projectileType, Vector2 startingPosition, Vector2 direction);
	[Signal]
	public delegate void HomingProjectileFireEventHandler(ProjectileType projectileType, Vector2 startingPosition, Node2D target);

	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
	}
}
