using Godot;
using Godot.Collections;
using System;

public partial class DebateMinigame : Node2D
{
    private PackedScene _debateQuestion;

    private Dictionary<int, string> _debateQuestions;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _debateQuestion = GD.Load<PackedScene>("res://Mingames/DebateMinigame/debate_question.tscn");
        SpawnQuestion("Taxes?", -250);
        SpawnQuestion("Immigration?\nBuild the Wall?", 150);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    #region HELPER_FUNCTIONS

    public void SpawnQuestion(string text, int x)
    {
        DebateQuestion debateQuestion = _debateQuestion.Instantiate() as DebateQuestion;
        debateQuestion.SetQuestionText(text);
        AddChild(debateQuestion); //put in tree
        debateQuestion.Position = new Vector2(x, -200);
    }

    #endregion
}
