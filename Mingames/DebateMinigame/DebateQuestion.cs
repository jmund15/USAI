using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class DebateQuestion : Node2D
{
	private string _labelText;
	private int _baseFallSpeed = 15;
	private int _fallSpeed;
	private Vector2 test = new Vector2(0,0);
	 
	private Label _questionLabel;
	private Theme _labelTheme;
	private Area2D _labelArea;
	private CollisionShape2D _labelCollisionShape;
	private RectangleShape2D _labelRectangleShape;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_questionLabel = GetNode<Label>("QuestionText");
        _labelArea = GetNode<Area2D>("Area2D");
		_labelCollisionShape = _labelArea.GetNode<CollisionShape2D>("CollisionShape2D");
		_labelRectangleShape = _labelCollisionShape.Shape as RectangleShape2D;

        _questionLabel.Text = _labelText;
		GD.Print(_labelText + " - LABEL SIZE: " + _questionLabel.Size);
		_labelRectangleShape.SetDeferred("size", _questionLabel.Size);
        //_labelRectangleShape.Size = _questionLabel.Size;
        //GD.Print("NEW COL SIZE: " + _labelRectangleShape.Size);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (_labelRectangleShape.Size != _questionLabel.Size)
		{
            _labelRectangleShape.Size = _questionLabel.Size;
        }
		if (Position.Y > 300)
		{
			QueueFree();
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		Position += new Vector2(0, _fallSpeed * 2 * (float)delta);
    }

    #region HELPER_FUNCTIONS

    public void SetQuestionText(string text)
	{
        _labelText = text;
		_fallSpeed = _baseFallSpeed + text.Length;
	}

    #endregion
}
