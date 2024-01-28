using Godot;
using System;
using System.Collections.Generic;

public partial class MidStatus : Node2D
{
    private Label chironLabel;

	private List<string> chironFirstPart;
    private List<string> chironSecondPart;

    private List<string> playerNames=new List<string> {"DEFAULT"};

    private string chiron="";

    private void generateChirons()
    {
        Random random = new Random();
        for (int i = 0; i < 3; i++) {
            int id = (random.Next(0, chironFirstPart.Count));
            int playerID = (random.Next(0, playerNames.Count));
            string player = (playerNames[playerID]);
            chiron += string.Format("{0}{1}{2}", chironFirstPart[id], player, chironSecondPart[id]);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		chironFirstPart = new List<string> {
            "",
			"",
			"Who da’ man? ",
			"",
            "",
            "\"The Texan-American war MUST END!\", declares ",
            "\"Pokemon Go-to-the-polls!\", declares ",
            "",
            "\"Hot Dogs are Sandwiches\" Exclaims "
        };
        chironSecondPart = new List<string> { 
            " promises to make opium illegal (population outraged). ",
			" has Invoked a Cerified Hashbrown Moment. ",
            " da’ man (or women :o)! ",
            " promises to make school lunches worse. ",
            " Suggest a border wall across Missouri. ",
            " (BRING OUR TROOPS HOME) ",
            ", despite the populous having no impact on the current election. ",
            " poll numbers skyrocket after USA.I. discovered their name on Einstein’s list. ",
            " (Millions Outraged) "
            };

        //TODO:GRAB PLAYER NAMES

        generateChirons();

        chironLabel = GetNode<Label>("TheNews/Chiron");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
