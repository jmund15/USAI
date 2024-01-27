using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    #region CLASS_VARIABLES
    public Dictionary<string, Robber> Players { get; private set; } = new Dictionary<string, Robber>();

    public Dictionary<string, string> key1InMap { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> key2InMap { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> contr1InMap { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> contr2InMap { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> contr3InMap { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> contr4InMap { get; private set; } = new Dictionary<string, string>();

    public bool IsMaxPlayers { get; private set; } = false;

    private PackedScene robberScene;

    #endregion

    #region BASE_GODOT_OVERRIDEN_FUNCTIONS
    public override void _Ready()
    {
        SetProcessUnhandledInput(true);
        robberScene = GD.Load<PackedScene>("res://Characters/prototype_robber.tscn");

        //var inputActions = InputMap.GetActions();
        //foreach (var action in inputActions)
        //{
        //    if (action == "key1Left")
        //    {

        //    }
        //    else if (action == "key1Right") {

        //    }
        //    else if (action == "key1Up") {

        //    }
        //    else if (action == "key1Down") {

        //    }
        //    else if (action == "key1SwingBag") {

        //    }

        //    else if (action == "key2Left")
        //    {

        //    }
        //    else if (action == "key2Right")
        //    {

        //    }
        //    else if (action == "key2Up")
        //    {

        //    }
        //    else if (action == "key2Down")
        //    {

        //    }
        //    else if (action == "key2SwingBag")
        //    {

        //    }

        //    else if (action == "contr1Left")
        //    {

        //    }
        //    else if (action == "contr1Right")
        //    {

        //    }
        //    else if (action == "contr1Up")
        //    {

        //    }
        //    else if (action == "contr1Down")
        //    {

        //    }
        //    else if (action == "contr1SwingBag")
        //    {

        //    }
        //}
    }

    public override void _Process(double delta)
    {
    }
    #endregion

    #region SIGNAL_LISTENERS

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!IsMaxPlayers && @event.IsAction("playerJoin"))
        {
            GD.Print("player joining");
            if (!Players.ContainsKey("Player1"))
            {
                var newPlayer = robberScene.Instantiate<Robber>();

                if (@event.IsAction("leftBoardplayerJoin"))
                {
                    newPlayer.SetInputStrings(1, "leftBoardleft", "leftBoardright", "leftBoardup", "leftBoarddown", "leftBoardswingBag");
                }
                else if (@event.IsAction("rightBoardplayerJoin"))
                {
                    newPlayer.SetInputStrings(1, "rightBoardleft", "rightBoardright", "rightBoardup", "rightBoarddown", "rightBoardswingBag");
                }
                AddChild(newPlayer);
            }
            else if (!Players.ContainsKey("Player2"))
            {
                var newPlayer = robberScene.Instantiate<Robber>();

                if (@event.IsAction("leftBoardplayerJoin"))
                {
                    newPlayer.SetInputStrings(2, "leftBoardleft", "leftBoardright", "leftBoardup", "leftBoarddown", "leftBoardswingBag");
                }
                else if (@event.IsAction("rightBoardplayerJoin"))
                {
                    newPlayer.SetInputStrings(2, "rightBoardleft", "rightBoardright", "rightBoardup", "rightBoarddown", "rightBoardswingBag");
                }
                AddChild(newPlayer);
            }
            else if (!Players.ContainsKey("Player3"))
            {

            }
            else // then player 4
            {
                IsMaxPlayers = true;
            }
            InputMap.ActionEraseEvent("playerJoin", @event);
        }
    }

    #endregion

    #region HELPER_FUNCITONS
    #endregion
}
