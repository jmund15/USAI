using Godot;
using System;

public partial class Ballot : Node2D
{
    private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());
    public string BallotName { get; set; }
	public float BallotSpeed { get; set; }
	public int PlayerNumPoint { get; set; }

	private Label _ballotNameLabel;
	private AnimationPlayer _animPlayer;
	private Area2D _ballotHitBoxArea;

	private Vector2 _flightXInterval = new Vector2(150f, 500f);
    private Vector2 _flightYInterval = new Vector2(100f, 400f);
	private Vector2 _scaleInterval = new Vector2(3f, 7f);
    private Vector2 _flightTimeInterval = new Vector2(0.5f, 2f);

    private bool _isHit = false;
    private float _rotationSpeed;
    private float _fallSpeed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_ballotNameLabel = GetNode<Label>("BallotLabel");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animPlayer.Play("ballotFlap");
        _ballotHitBoxArea = GetNode<Area2D>("Area2D");

        _ballotNameLabel.Text = BallotName;

		_ballotHitBoxArea.AreaEntered += OnBallotAreaEntered;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        if (_isHit)
        {
            Rotation += _rotationSpeed * (float)delta;
            Position += new Vector2(0, _fallSpeed) * (float)delta;
        }
        else
        {
            Position += new Vector2(0, BallotSpeed * (float)delta);
        }
    }

    private void OnBallotAreaEntered(Area2D area)
    {
		//var otherBal = area.GetParent() as Ballot;
  //      if (otherBal != null && Position.Y < 100)
		//{
		//	if (otherBal.Position.Y < Position.Y)
		//	{
		//		otherBal.QueueFree();
		//	}
		//	else
		//	{
		//		QueueFree();
		//	}
		//}
    }

	public void BallotHit(bool hitRight)
	{
        ZIndex += 1; // infront now
        _isHit = true;

        _ballotHitBoxArea.QueueFree(); // no need to hit detection anymore

        var xDub = Rnd.NextDouble();
		double xDiff;
        if (hitRight)
		{
            xDiff = xDub * (_flightXInterval.Y - _flightXInterval.X) + _flightXInterval.X; //Generates double within a range
        }
        else
		{
            xDiff = xDub * (-_flightXInterval.Y + _flightXInterval.X) - _flightXInterval.X; //Generates double within a range
        }

        var yDub = Rnd.NextDouble();
        var yDiff = yDub * (_flightYInterval.Y - _flightYInterval.X) + _flightYInterval.X; //Generates double within a range

        var scaleDub = Rnd.NextDouble();
        var finalScale = scaleDub * (_scaleInterval.Y - _scaleInterval.X) + _scaleInterval.X; //Generates double within a range

        var timeDub = Rnd.NextDouble();
        var flightTime = timeDub * (_flightTimeInterval.Y - _flightTimeInterval.X) + _flightTimeInterval.X; //Generates double within a range

        var tweenPos = GetTree().CreateTween();
        tweenPos.TweenProperty(this, "position", new Vector2(Position.X + (float)xDiff, Position.Y - (float)yDiff), flightTime).SetEase(Tween.EaseType.Out);
        
		var tweenScale = GetTree().CreateTween();
        tweenScale.TweenProperty(this, "scale", new Vector2((float)finalScale, (float)finalScale), flightTime).SetEase(Tween.EaseType.Out);
        
        var tweenAlpha = GetTree().CreateTween();
        tweenAlpha.TweenProperty(this, "modulate:a", 0f, flightTime * 0.75f).SetEase(Tween.EaseType.In).SetDelay(flightTime);
        tweenAlpha.TweenCallback(Callable.From(QueueFree));

        _rotationSpeed = (float)xDiff / 50;
        _fallSpeed = (float)yDiff * 2;
    }
}
