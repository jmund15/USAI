using Godot;
using System;
using System.Collections.Generic;

public partial class MidStatus : Node2D
{
    private Label chironLabel;

	private List<string> chironFirstPart;
    private List<string> chironSecondPart;

    private List<string> playerNames;

    private string chiron="";

    private void generateChirons()
    {
        List<int> usedIDs = new List<int>();

        Random random = new Random();
        for (int i = 0; i < 4; i++) {
            int id;
            do
            {
                id = (random.Next(0, chironFirstPart.Count));
            } while (usedIDs.Contains(id));


           int playerID = (random.Next(0, playerNames.Count));

            //Re-Do
            if (playerNames[playerID].Equals("disabled")){
                i--;
                continue;
            }

            string player = (playerNames[playerID]);
            chiron = chiron + string.Format("{0}{1}{2}", chironFirstPart[id], player, chironSecondPart[id]);
            usedIDs.Add(id);
        }

        //GD.Print(chiron);

        chironLabel.Text = chiron;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		chironFirstPart = new List<string> {
            "",
			"",
			"Who da\' man? ",
			"",
            "",
            "\"The Texas-American war MUST END!\", declares ",
            "\"Pokemon Go-to-the-polls!\", declares ",
            "",
            "\"Hot Dogs are Sandwiches\" Exclaims "
        };
        chironSecondPart = new List<string> { 
            " promises to make opium illegal (population outraged). ",
			" has Invoked a Cerified Hashbrown Moment. ",
            " da\' man (or woman :o)! ",
            " promises to make school lunches worse. ",
            " Suggest a border wall across Missouri. ",
            " (BRING OUR TROOPS HOME) ",
            ", despite the populous having no impact on the current election. ",
            " poll numbers skyrocket after USA.I. discovered their name on Einstein\'s list. ",
            " (Millions Outraged) "
            };

        //TODO:GRAB PLAYER NAMES
        playerNames = new List<string> { "DEFAULT" , "disabled", "disabled", "disabled" };

        chironLabel = GetNode<Label>("TheNews/Chiron");

        generateChirons();

        PlayerManager playerManager = GetNode<PlayerManager>("/root/MainScene/PlayerManager");

        //playerManager.GetNames += GetNames;

    }

    private void GetNames(string p1, string p2, string p3, string p4)
    {
        playerNames[0] = p1;
        playerNames[1] = p2;
        playerNames[2] = p3;
        playerNames[3] = p4;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        chironLabel.Position += new Vector2(-150,0) * (float)delta;
	}
}
