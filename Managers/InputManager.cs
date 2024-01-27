using Godot;
using System;
using System.Collections.Generic;

public partial class InputManager : Node
{

	private List<Controller> Controllers;

	private InputEventJoypadButton buttonReturn(int device, JoyButton buttonID)
	{
		InputEventJoypadButton button= new InputEventJoypadButton();
		button.Device = device;
		button.ButtonIndex = buttonID;
		return button; 
	}

	//TODO: ABOVE BUT WITH JOY AXIS

    //InputMap.AddAction(newAction, deadzone);
    //InputMap.ActionAddEvent(newAction, newEvent);
	//Does mapping for top of controller list. I hope this doesn't bonk as a race condition.
	private void initInputMap()
	{
		//Add everything to input map here, this is static AF. Quick and Dirty Unfortunately
		InputMap.AddAction(String.Format("up{0}",Controllers.Count), 0.25f);
        InputMap.ActionAddEvent(String.Format("up{0}", Controllers.Count), buttonReturn(Controllers[Controllers.Count-1].device, JoyButton.DpadUp));
        InputMap.AddAction(String.Format("left{0}", Controllers.Count), 0.25f);
        InputMap.ActionAddEvent(String.Format("left{0}", Controllers.Count), buttonReturn(Controllers[Controllers.Count - 1].device, JoyButton.DpadLeft));
        InputMap.AddAction(String.Format("down{0}", Controllers.Count), 0.25f);
        InputMap.ActionAddEvent(String.Format("down{0}", Controllers.Count), buttonReturn(Controllers[Controllers.Count - 1].device, JoyButton.DpadDown));
        InputMap.AddAction(String.Format("right{0}", Controllers.Count), 0.25f);
        InputMap.ActionAddEvent(String.Format("right{0}", Controllers.Count), buttonReturn(Controllers[Controllers.Count - 1].device, JoyButton.DpadRight));
        InputMap.AddAction(String.Format("A{0}", Controllers.Count));
        InputMap.ActionAddEvent(String.Format("A{0}", Controllers.Count), buttonReturn(Controllers[Controllers.Count - 1].device, JoyButton.A));
        InputMap.AddAction(String.Format("B{0}", Controllers.Count));
        InputMap.ActionAddEvent(String.Format("B{0}", Controllers.Count), buttonReturn(Controllers[Controllers.Count - 1].device, JoyButton.B));
    }

    private void destroyInputMap()
	{
		//Init in reverse, also all static
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Controllers = new List<Controller>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	

    public override void _UnhandledInput(InputEvent @event)
    {
		//Fail out if we already have max players
		if (Controllers.Count >= 4) {
			return;
		}

		//WARNING, SCOPED TO JOYPAD ONLY, NO KEYBOARDS ALLOWED
		if(@event is InputEventJoypadButton eventJoypadButton) {

			//Fail if already mapped failsafe
			foreach (Controller cont in Controllers) {
				if (cont.device == @event.Device)
				{
					return;
				}
			}

			//DO JOYSTICK INPUT MAPPING

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

	public int device;

    public Controller(int deviceID) {
	//Init to no Input
	inputState= new List<bool> { false, false, false, false, false, false};

	device=deviceID;
	}

	
}