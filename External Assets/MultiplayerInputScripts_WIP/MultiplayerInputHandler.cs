using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

public partial class MultiplayerInputHandler : Node
{
	//## A globally accessible manager for device-specific actions.
	//##
	//## This class automatically duplicates relevant events on all actions for new joypads
	//## when they connect and disconnect.
	//## It also provides a nice API to access all the normal "Input" methods,
	//## but using the device integers and the same action names.
	//## All methods in this class that have a "device" parameter can accept -1
	//## which means the keyboard device.
	//## NOTE: The -1 device will not work on Input methods because it is a specific
	//## concept to this MultiplayerInput class.
	//##
	//## See DeviceInput for an object-oriented way to get input for a single device.
	//## An array of all the non-duplicated action names
	private Godot.Collections.Array<StringName> _coreActions = new Godot.Collections.Array<StringName>();

	//# a dictionary of all action names
	//# the keys are the keyboard/device names/numbersS
	//# the values are a dictionary that maps action name to device action name
	//# for example device_actions[device][action_name] is the device-specific action name
	//# the purpose of this is to cache all the StringNames of all the actions
	//# ... so it doesn't need to generate them every time
	private Dictionary<string, Dictionary<string, string>> _deviceActions = new Dictionary<string, Dictionary<string, string>>();
	public override void _Ready()
	{
		reset();
	}

	private bool IsKeyboardEvent(InputEvent inEvent)
	{
		return (inEvent is InputEventKey);
	}
	private bool IsJoypadEvent(InputEvent inEvent)
	{
		return (inEvent is InputEventJoypadButton) || (inEvent is InputEventJoypadMotion);
	}

	// call this if you change any of the core actions or need to reset everything
	private void reset()
	{
		InputMap.LoadFromProjectSettings();
		_coreActions = InputMap.GetActions();

		// disable joypad events on keyboard actions
		// by setting device id to 8 (out of range, so they'll never trigger)
		// I can't just delete them because they're used as blueprint when a joypad connects
		foreach (var action in _coreActions)
		{
			foreach (var e in InputMap.ActionGetEvents(action))
			{

				if (IsJoypadEvent(e))
				{
					e.Device = 8;
				}
			}
		}

		CreateActionsforKeyboard();

        // create actions for already connected gamepads
        //foreach (var device in Input.GetConnectedJoypads())
        //{
        //	CreateActionsForDevice(device);
        //}

        //// create actions for gamepads that connect in the future
        //// also clean up when gamepads disconnect
        //Input.JoyConnectionChanged += OnJoyConnectionChanged;
    }

	//	private void OnJoyConnectionChanged(long device, bool connected) {
	//		if (connected)
	//		{
	//			CreateActionsForDevice((int)device);

	//		}
	//		else {
	//			DeleteActionsForDevice((int)device);
	//		}
	//	}
	private void CreateActionsforKeyboard()
	{
		foreach (var coreAction in _coreActions)
		{
			if (coreAction.ToString().Contains("ui"))
			{
				continue;
			}
			var leftBoardAction = new string("leftBoard" + coreAction);
			var rightBoardAction = new string("rightBoard" + coreAction);
			var deadzone = InputMap.ActionGetDeadzone(coreAction);

			// get all keyboard events for this action
			var events = InputMap.ActionGetEvents(coreAction).Where(IsKeyboardEvent).ToList();

			// only copy this event if it is relevant to keyboards
			if (events.Count > 0)
			{
				// first add the action with the new name
				InputMap.AddAction(leftBoardAction, deadzone);
				InputMap.AddAction(rightBoardAction, deadzone);

				//if (!_deviceActions.ContainsKey("leftBoard")) {
    //                _deviceActions.Add("leftBoard", )
    //            }
				//_deviceActions["leftBoard"].Add
				//_deviceActions["leftBoard"][coreAction] = leftBoardAction;
				//_deviceActions["rightBoard"][coreAction] = rightBoardAction;

				// without duplicating, all of them have a reference to the same event object
				// which doesn't work because this has to be unique to this device
				InputEvent leftBoardEvent = events[0].Duplicate() as InputEvent;
				InputMap.ActionAddEvent(leftBoardAction, leftBoardEvent);
                InputEvent rightBoardEvent = events[1].Duplicate() as InputEvent;
				InputMap.ActionAddEvent(rightBoardAction, rightBoardEvent);
				// this might be (is) a terrible way to do this
			}
		}
		_coreActions = InputMap.GetActions();
		foreach (var coreAction in _coreActions)
		{
            if (coreAction.ToString().Contains("ui"))
            {
                continue;
            }
            GD.Print("core action: " + coreAction);
			foreach (var ev in InputMap.ActionGetEvents(coreAction))
			{   
                if (ev is InputEventKey)
				{
					var keyEv = ev as InputEventKey;
                    GD.Print("\tkey event in action: " + keyEv.PhysicalKeycode);
                }
				else
				{
                    GD.Print("\tevent in action: " + ev.Device);

                }
            }
		}
	}

		//	private void CreateActionsForDevice(int device)
		//	{
		//		var deviceStr = device.ToString();
		//		foreach (var coreAction in _coreActions)
		//		{
		//			var newAction = new string(deviceStr + coreAction);
		//			var deadzone = InputMap.ActionGetDeadzone(coreAction);

		//			// get all joypad events for this action
		//			var events = InputMap.ActionGetEvents(coreAction).Where(IsJoypadEvent).ToList();

		//			// only copy this event if it is relevant to joypads
		//			if (events.Count > 0) {
		//				// first add the action with the new name
		//				InputMap.AddAction(newAction, deadzone);
		//				_deviceActions[deviceStr][coreAction] = newAction;

		//				// then copy all the events associated with that action
		//				// this only includes events that are relevant to joypads
		//				foreach (var ev in events) {
		//					// without duplicating, all of them have a reference to the same event object
		//					// which doesn't work because this has to be unique to this device
		//					InputEvent newEvent = ev.Duplicate() as InputEvent;
		//					newEvent.Device = device;

		//					// switch the device to be just this joypad
		//					InputMap.ActionAddEvent(newAction, newEvent);
		//				}
		//			}
		//		}
		//	}

		//	private void DeleteActionsForDevice(int device) {
		//		var deviceStr = device.ToString();
		//		_deviceActions.Remove(deviceStr);
		//		var actionsToErase = new List<StringName>();

		//		// figure out which actions should be erased
		//		foreach (var action in InputMap.GetActions()) {
		//			var actionStr = action.ToString();

		//			var maybeDevice = actionStr.Substr(0, deviceStr.Length);
		//			if (maybeDevice == deviceStr) {
		//				actionsToErase.Add(action);
		//			}
		//		}
		//		// now actually erase them
		//		// this is done separately so I'm not erasing from the collection I'm looping on
		//		// not sure if this is necessary but whatever, this is safe
		//		foreach (var action in actionsToErase) {
		//			InputMap.EraseAction(action);
		//		}

		//	}

		//	// use these functions to query the action states just like normal Input functions
		//	private float GetActionRawStrength(int device, StringName action, bool exactMatch = false) {
		//		if (device >= 0) {
		//			action = GetActionName(device, action);
		//		}
		//		return Input.GetActionRawStrength(action, exact_match);
		//	}

		//func get_action_strength(device: int, action: StringName, exact_match: bool = false) -> float:
		//	if device >= 0:
		//		action = get_action_name(device, action)
		//	return Input.get_action_strength(action, exact_match)

		//func get_axis(device: int, negative_action: StringName, positive_action: StringName) -> float:
		//	if device >= 0:
		//		negative_action = get_action_name(device, negative_action)

		//        positive_action = get_action_name(device, positive_action)
		//	return Input.get_axis(negative_action, positive_action)

		//func get_vector(device: int, negative_x: StringName, positive_x: StringName, negative_y: StringName, positive_y: StringName, deadzone: float = -1.0) -> Vector2:
		//	if device >= 0:
		//		negative_x = get_action_name(device, negative_x)

		//        positive_x = get_action_name(device, positive_x)

		//        negative_y = get_action_name(device, negative_y)

		//        positive_y = get_action_name(device, positive_y)
		//	return Input.get_vector(negative_x, positive_x, negative_y, positive_y, deadzone)

		//func is_action_just_pressed(device: int, action: StringName, exact_match: bool = false) -> bool:
		//	if device >= 0:
		//		action = get_action_name(device, action)
		//	return Input.is_action_just_pressed(action, exact_match)

		//func is_action_just_released(device: int, action: StringName, exact_match: bool = false) -> bool:
		//	if device >= 0:
		//		action = get_action_name(device, action)
		//	return Input.is_action_just_released(action, exact_match)

		//func is_action_pressed(device: int, action: StringName, exact_match: bool = false) -> bool:
		//	if device >= 0:
		//		action = get_action_name(device, action)
		//	return Input.is_action_pressed(action, exact_match)

		//// returns the name of a gamepad-specific action
		//func get_action_name(device: int, action: StringName) -> StringName:
		//	if device >= 0:
		//		assert(device_actions.has(device), "Device %s has no actions. Maybe the joypad is disconnected." % device)
		//}
		//        //if it says this dictionary doesn't have the key,
		//// that could mean it's an invalid action name.
		//// or it could mean that action doesn't have a joypad event assigned
		//		return device_actions[device][action]

		//// return the normal action name for the keyboard player
		//	return action
	}


