using Godot;
using System;

public partial class ProjectileEnergyBall : Projectile
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("instatiated energy ball projectile");
		ProjSpeed = 150;
		ProjPower = 25;
		base._Ready();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
