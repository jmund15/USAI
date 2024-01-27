using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerManager : Node
{
    #region CLASS_VARIABLES

    #endregion

    #region BASE_GODOT_OVERRIDEN_FUNCTIONS
    public override void _Ready()
    {
        SetProcessUnhandledInput(true);
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

    

    #endregion

    #region HELPER_FUNCITONS
    #endregion
}
