using Godot;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

public partial class InputManager : Node
{
    [Signal]
    public delegate void ControllersConnectedEventHandler(bool p1, bool p2, bool p3, bool p4);

	private List<Controller> Controllers;

	private List<bool> mappedSlots;

	private InputEventJoypadButton buttonReturn(int device, JoyButton buttonID)
	{
		InputEventJoypadButton button= new InputEventJoypadButton();
		button.Device = device;
		button.ButtonIndex = buttonID;
		return button; 
	}

    private InputEventJoypadMotion axisReturn(int device, JoyAxis axisID)
    {
        InputEventJoypadMotion axis = new InputEventJoypadMotion();
        axis.Device = device;
        axis.Axis = axisID;
        return axis;
    }

    //InputMap.AddAction(newAction, deadzone);
    //InputMap.ActionAddEvent(newAction, newEvent);
    //Does mapping for associated controller to player num.
    private void initInputMap(int playerNum)
	{
		int initPlayer = -1;
        int controllerID = -1;

		for(int i=0; i<Controllers.Count; i++)
		{
			//If the controller is the device for the specified player number, map the actions
			if (Controllers[i].playerNum==playerNum)
			{
				initPlayer = Controllers[i].device;
                controllerID = i;
			}
		}

		//Fail if player not found
		if(initPlayer == -1)
		{
			//Log Failure Here
			return;
		}

		//Add everything to input map here, this is static AF. Quick and Dirty Unfortunately 
		//WARNING USING DPAD ONLY!!!
		InputMap.AddAction(String.Format("up{0}", playerNum), 0.25f);
        InputMap.ActionAddEvent(String.Format("up{0}", playerNum), buttonReturn(Controllers[controllerID].device, JoyButton.DpadUp));
        InputMap.AddAction(String.Format("left{0}", playerNum), 0.25f);
        InputMap.ActionAddEvent(String.Format("left{0}", playerNum), buttonReturn(Controllers[controllerID].device, JoyButton.DpadLeft));
        InputMap.AddAction(String.Format("down{0}", playerNum), 0.25f);
        InputMap.ActionAddEvent(String.Format("down{0}", playerNum), buttonReturn(Controllers[controllerID].device, JoyButton.DpadDown));
        InputMap.AddAction(String.Format("right{0}", playerNum), 0.25f);
        InputMap.ActionAddEvent(String.Format("right{0}", playerNum), buttonReturn(Controllers[controllerID].device, JoyButton.DpadRight));

		/*
        InputMap.AddAction(String.Format("upAng{0}", initPlayer), 0.25f);
        InputMap.ActionAddEvent(String.Format("upAng{0}", initPlayer), axisReturn(Controllers[initPlayer].device, JoyAxis.LeftY));
        InputMap.AddAction(String.Format("leftAng{0}", initPlayer), -0.25f);
        InputMap.ActionAddEvent(String.Format("leftAng{0}", initPlayer), axisReturn(Controllers[initPlayer].device, JoyAxis.LeftX));
        InputMap.AddAction(String.Format("downAng{0}", initPlayer), -0.25f);
        InputMap.ActionAddEvent(String.Format("downAng{0}", initPlayer), axisReturn(Controllers[initPlayer].device, JoyAxis.LeftY));
        InputMap.AddAction(String.Format("rightAng{0}", initPlayer), 0.25f);
        InputMap.ActionAddEvent(String.Format("rightAng{0}", initPlayer), axisReturn(Controllers[initPlayer].device, JoyAxis.LeftX));
		*/

        InputMap.AddAction(String.Format("A{0}", playerNum));
        InputMap.ActionAddEvent(String.Format("A{0}", playerNum), buttonReturn(Controllers[controllerID].device, JoyButton.A));
        InputMap.AddAction(String.Format("B{0}", playerNum));
        InputMap.ActionAddEvent(String.Format("B{0}", playerNum), buttonReturn(Controllers[controllerID].device, JoyButton.B));
    }

    private void destroyInputMap(int playerID)
	{
        //Init in reverse, also all static
        InputMap.EraseAction(string.Format("up{0}", playerID));
        InputMap.EraseAction(string.Format("left{0}", playerID));
        InputMap.EraseAction(string.Format("down{0}", playerID));
        InputMap.EraseAction(string.Format("right{0}", playerID));
        InputMap.EraseAction(string.Format("A{0}", playerID));
        InputMap.EraseAction(string.Format("B{0}", playerID));
    }

    public void destroyController(int playerID)
    {
        int controllerID = -1;

        for(int i=0; i<Controllers.Count; i++)
        {
            if (Controllers[i].playerNum == playerID)
            {
                controllerID = i;
                break;
            }
        }

        //Destroy Input Map
        destroyInputMap(Controllers[controllerID].playerNum);

        foreach (bool mPlay in mappedSlots)
        {
            //GD.Print("\n", mPlay);
        }

        //Free the slot
        mappedSlots[playerID - 1] = false;

        //GD.Print("\n", "new");

        foreach (bool mPlay in mappedSlots)
        {
           // GD.Print("\n", mPlay);
        }

        //GD.Print(string.Format("{0} Disconnected as Player {1}", Controllers[controllerID].device, playerID));

        //Remove from the List
        Controllers.RemoveAt(controllerID);

        
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Init resources
		Controllers = new List<Controller>();
		mappedSlots =new List<bool>{false, false, false, false};
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Godot.Collections.Array<int> deviceIDs = Input.GetConnectedJoypads();

        //Make sure each controller is connected
        foreach (Controller cont in Controllers)
        {
            //Look and see the controller is in the list of connected controllers
            int mappedDevice = -1;
            foreach (int devices in deviceIDs)
            {

                if (devices == cont.device)
                {
                    mappedDevice = devices;
                }
            }
            if (mappedDevice == -1)
            {
                destroyController(cont.playerNum);
                break;
            }

            for (int i=0; i < Controllers.Count; i++)
            {
                if ((Controllers[i].device == cont.device) && (Controllers[i].playerNum != cont.playerNum)) {
                    destroyController(Controllers[i].playerNum);
                    GD.Print("\nFunny Dupe Error Occured, Fixed");
                }
            }

        }

    EmitSignal(SignalName.ControllersConnected, mappedSlots[0], mappedSlots[1], mappedSlots[2], mappedSlots[3]);

}

/*public override void _Input(InputEvent @event)
{
    int playerID=-1;
    int controllerID = -1;

    //See if this is a mapped device, get player num
    for (int i = 0; i<Controllers.Count; i++)
    {
        if (@event.Device == Controllers[i].device)
        {
            playerID = Controllers[i].playerNum;
            controllerID = i; 
            break;

        }
    }

    if (playerID == -1)
    {
        return;
    }

    //Get input for device here
    if (@event is InputEventJoypadButton joyEvent)
    {
     //Up
        if(InputMap.EventIsAction(@event, string.Format("up{0}", playerID)))
        {
            if (joyEvent.IsActionPressed(string.Format("up{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(0, true);
            }
            else if (joyEvent.IsActionReleased(string.Format("up{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(0, false);
            }
        }
        //Left
        if (InputMap.EventIsAction(@event, string.Format("left{0}", playerID)))
        {
            if (joyEvent.IsActionPressed(string.Format("left{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(1, true);
            }
            else if (joyEvent.IsActionReleased(string.Format("left{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(1, false);
            }
        }
        //Down
        if (InputMap.EventIsAction(@event, string.Format("down{0}", playerID)))
        {
            if (joyEvent.IsActionPressed(string.Format("down{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(2, true);
            }
            else if (joyEvent.IsActionReleased(string.Format("down{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(2, false);
            }
        }
        //Right
        if (InputMap.EventIsAction(@event, string.Format("right{0}", playerID)))
        {
            if (joyEvent.IsActionPressed(string.Format("right{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(3, true);
            }
            else if (joyEvent.IsActionReleased(string.Format("right{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(3, false);
            }
        }
        //A
        if (InputMap.EventIsAction(@event, string.Format("A{0}", playerID)))
        {
            if (joyEvent.IsActionPressed(string.Format("A{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(4, true);
            }
            else if (joyEvent.IsActionReleased(string.Format("A{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(4, false);
            }
        }
        //B
        if (InputMap.EventIsAction(@event, string.Format("B{0}", playerID)))
        {
            if (joyEvent.IsActionPressed(string.Format("B{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(5, true);
            }
            else if (joyEvent.IsActionReleased(string.Format("B{0}", playerID)))
            {
                Controllers[controllerID].setInputStateBit(5, false);
            }
        }

    }
}*/

public override void _Input(InputEvent @event)
    {
        var _coreActions = InputMap.GetActions();
        foreach (var coreAction in _coreActions)
        {
            if (@event.IsAction(coreAction.ToString()))
            {
                //GD.Print("\n", coreAction.ToString());
            }
        }
     }

    public override void _UnhandledInput(InputEvent @event)
    {
        //Fail if already mapped failsafe
        foreach (Controller cont in Controllers)
        {
            if (cont.device == @event.Device)
            {
                return;
            }
        }

        //GD.Print("\n", mappedSlots.ToString());

        //WARNING, SCOPED TO JOYPAD ONLY, NO KEYBOARDS ALLOWED
        if (@event is InputEventJoypadButton eventJoypadButton) {
            int mapPlayer = -1;

            //Fail out if we already have max players
            for (int i = 0; i < mappedSlots.Count; i++)
            {
                if (mappedSlots[i] == false)
                {
                    mapPlayer = i + 1;
                    break;
                }
            }

            //GD.Print("\n Old Mapped Players");

            foreach (bool mPlay in mappedSlots)
            {
                //GD.Print("\n", mPlay);
            }

            //Fails if all players are mapped
            if (mapPlayer == -1)
            {
                return;
            }

            //GD.Print("\n", mappedSlots.ToString());

           // GD.Print("\n", mapPlayer);

            //Add Controller to InputMan
            Controllers.Add(new Controller(@event.Device, mapPlayer));

            //DO JOYSTICK INPUT MAPPING
            initInputMap(mapPlayer);

            //Add to mapped slots
            mappedSlots[mapPlayer - 1] = true;

            //GD.Print("\n New Mapped Players");

            foreach (bool mPlay in mappedSlots)
            {
               //GD.Print("\n", mPlay);
            }

            //GD.Print(string.Format("{0} Connected as Player {1}\nActions: ", Controllers[Controllers.Count - 1].device, mapPlayer));

            var _coreActions = InputMap.GetActions();
            foreach (var coreAction in _coreActions)
            {
                if (!coreAction.ToString().Contains("ui"))
                {
                   //GD.Print(string.Format("\n{0}", coreAction.ToString()));
                }
            }

                return;
		}
    }

	/*Controller Object
	 * Holds Input State and Device/Player Binding
	*/
	public class Controller
	{
		//Input State is an Array of 6 Bools representing buttons pressed as follows:
		// Up, Left, Down, Right, A, B
		private List<bool> inputState;

		//Device ID
		public int device;

		//Player Number
		public int playerNum;

		public Controller(int deviceID, int playNum)
		{
			//Init to no Input
			inputState = new List<bool> { false, false, false, false, false, false };

			device = deviceID;
			playerNum = playNum;
		}

        public void setInputStateBit(int iter, bool val) { inputState[iter] = val; }
        public List<bool> getInputState() { return inputState; }
	}
}