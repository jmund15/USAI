using Godot;
using System;

public partial class IntroScene : Node2D
{
    private Global _global { get; set; }
    private Events _signalBus { get; set; }


    public Label IntroLabel;

    public Sprite2D NewAmerica;
    public Sprite2D _robot;
    public Sprite2D _mrica;
    public Sprite2D Usai;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _signalBus = GetNode<Events>("/root/Events");
        _global = GetNode<Global>("/root/Global");

        IntroLabel = GetNode<Label>("IntroLabel");

        NewAmerica = GetNode<Sprite2D>("NewAmerica");
        _robot = GetNode<Sprite2D>("Robot");
        _mrica = GetNode<Sprite2D>("Mrica");
        Usai = GetNode<Sprite2D>("Usai");


    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
