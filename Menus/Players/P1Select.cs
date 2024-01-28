using Godot;
using System;
using System.Collections.Generic;

public partial class P1Select : Control
{
	private GridContainer p1keyboard;

    private GridContainer p1CharEdit;

	private Label p1Label;
	
    private int playerNum = 1;

    private int head;

    private int maxHeads=4;

	private Button currentButton;

	private Color origColor= new Color(255,255,255,1);
    private Color p1Color = new Color(255, 0, 0, 1);

    private Color Red = new Color(255, 0, 0, 1);
    private Color Green = new Color(0, 255, 0, 1);
    private Color Blue = new Color(0, 0, 255, 1);

    private int red;
    private int green;
    private int blue;
    

    private string name;

	private int selectorX;
	private int selectorY;
    private int selectorEdit;

    private List<List<char>> selections;
    private List<char> selectionsEdit;

    private bool state;
    private bool charState;

	private List<bool> controllersConnected;

    private void changeSliderVal(bool positive, char editor)
    {
        int addTo= positive ? 10 : -10;

        switch (editor)
        {
            case 'R':
                p1CharEdit.GetNode<Slider>("RedSlider").Value = p1CharEdit.GetNode<Slider>("RedSlider").Value+addTo;
                return;
            case 'G':
                p1CharEdit.GetNode<Slider>("GreenSlider").Value = p1CharEdit.GetNode<Slider>("GreenSlider").Value+addTo;
                return;
            case 'B':
                p1CharEdit.GetNode<Slider>("BlueSlider").Value = p1CharEdit.GetNode<Slider>("BlueSlider").Value + addTo;
                return;
        }
    }

    private void changeSliderColor(char editor)
    {
        GD.Print(editor);
        p1CharEdit.GetNode<Button>("HeadChange").Modulate = origColor;
        p1CharEdit.GetNode<Slider>("RedSlider").Modulate = Red;
        p1CharEdit.GetNode<Slider>("GreenSlider").Modulate = Green;
        p1CharEdit.GetNode<Slider>("BlueSlider").Modulate = Blue;

        switch (editor)
        {
            case 'H':
                p1CharEdit.GetNode<Button>("HeadChange").Modulate = p1Color;
                return;
            case 'R':
                p1CharEdit.GetNode<Slider>("RedSlider").Modulate = origColor;
                return;
            case 'G':
                p1CharEdit.GetNode<Slider>("GreenSlider").Modulate = origColor;
                return;
            case 'B':
                p1CharEdit.GetNode<Slider>("BlueSlider").Modulate = origColor;
                return;
        }
    }

	private void chageButtonColor(char selectCase) {
		currentButton.Modulate=origColor;

        switch (selectCase)
        {
            case '@':
                currentButton = p1keyboard.GetNode<Button>("At");
                currentButton.Modulate = p1Color;
                return;
            case '$':
                currentButton = p1keyboard.GetNode<Button>("Cash");
                currentButton.Modulate = p1Color;
                return;
            case '(':
                currentButton = p1keyboard.GetNode<Button>("Clear");
                currentButton.Modulate = p1Color;
                return;
            case ')':
                currentButton = p1keyboard.GetNode<Button>("Enter");
                currentButton.Modulate = p1Color;
                return;
        }

        

        if (selectCase != '@' || selectCase != '$' || selectCase != '(' || selectCase != ')')
		{
			currentButton=p1keyboard.GetNode<Button>(selectCase.ToString());
		}
		
		currentButton.Modulate=p1Color;
		}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		name = "";
        head = 0;
		selectorX = 0;
		selectorY = 0;
        selectorEdit = 0;

        red = 0;
        green = 0;
        blue = 0;

		state = false;
        charState = false;

		selections = new List<List<char>>();
		
		selections.Add(new List<char> {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' });
        selections.Add(new List<char> { 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P' });
        selections.Add(new List<char> { 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X' });
        selections.Add(new List<char> { 'Y', 'Z', '!', '?', '@', '$', '(', ')' });

        p1keyboard = GetNode<GridContainer>(string.Format("P{0}Keyboard", playerNum));

		currentButton = p1keyboard.GetNode<Button>("A");

		p1Label = GetNode<Label>(string.Format("P{0}Label",playerNum));

        //Start By hiding keys
        p1keyboard.Hide();

        selectionsEdit=new List<char> {'H', 'R', 'G', 'B' };

        p1CharEdit = GetNode<GridContainer>(string.Format("P{0}Char", playerNum));

        p1CharEdit.Hide();

        InputManager InputMan = GetNode<InputManager>("/root/MainScene/InputManager");

		controllersConnected = new List<bool> {false, false, false, false};

		InputMan.ControllersConnected += OnControllersConnected;


    }

    private void OnControllersConnected(bool p1, bool p2, bool p3, bool p4)
    {
		controllersConnected[0] = p1;
        controllersConnected[1] = p2;
        controllersConnected[2] = p3;
        controllersConnected[3] = p4;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		//Swap to Idle
		if (!controllersConnected[0]&&state)
		{
			p1keyboard.Hide();
            p1CharEdit.Hide();
            name = "";
			p1Label.Text = "Press the A Button to Join";
			state = false;
		}
		//Init
		else if (controllersConnected[0] && !state)
		{
            p1Label.Text = "Name: ";
			p1keyboard.Show();
            foreach (var charArr in selections)
            {
                foreach (char chara in charArr)
                {
                    chageButtonColor(chara);
                }
            }
            chageButtonColor(selections[selectorY][selectorX]);
            state = true;
        }

	}

	public override void _Input(InputEvent @event)
	{
        if (state)
        {
            if (charState)
            {
                //GD.Print(selectorEdit, " Got to New Code");
                if(@event.IsActionPressed(string.Format("up{0}", playerNum)))
                {
                    if (selectorEdit != 0)
                    {
                       // GD.Print(selectorEdit, " Got to New Up");
                        selectorEdit = selectorEdit-1;
                        changeSliderColor(selectionsEdit[selectorEdit]);
                    }
                }
                if (@event.IsActionPressed(string.Format("down{0}", playerNum)))
                {
                    GD.Print(selectorEdit, " Got to New Down");
                    if (selectorEdit != selectionsEdit.Count)
                    {
                        selectorEdit = selectorEdit+1;
                        changeSliderColor(selectionsEdit[selectorEdit]);
                    }
                }
                if (@event.IsActionPressed(string.Format("left{0}", playerNum)))
                {
                    if (selectorEdit != 0)
                    {
                        changeSliderVal(false, selectionsEdit[selectorEdit]);
                    }
                }
                if (@event.IsActionPressed(string.Format("right{0}", playerNum)))
                {
                    if (selectorEdit != 0)
                    {
                        changeSliderVal(true, selectionsEdit[selectorEdit]);
                    }
                }
                if (@event.IsActionPressed(string.Format("A{0}", playerNum)))
                {
                    if (selectorEdit == 0)
                    {
                        head = (head+1) % maxHeads;
                    }
                }
                if (@event.IsActionPressed(string.Format("B{0}", playerNum)))
                {
                    charState = false;
                    p1CharEdit.Hide();

                    p1keyboard.Show();
                    foreach (var charArr in selections)
                    {
                        foreach (char chara in charArr)
                        {
                            chageButtonColor(chara);
                        }
                    }
                    chageButtonColor(selections[selectorY][selectorX]);
                    state = true;

                }
                return;
            }

            //Move, if keyboard is up
            if (state && @event.IsActionPressed(string.Format("up{0}", playerNum)))
            {
                if (selectorY == 0)
                {
                    selectorY = selections.Count - 1;
                }
                else
                {
                    selectorY = (selectorY - 1);
                }
                chageButtonColor(selections[selectorY][selectorX]);
            }
            if (state && @event.IsActionPressed(string.Format("left{0}", playerNum)))
            {
                if (selectorX == 0)
                {
                    selectorX = selections[0].Count - 1;
                }
                else
                {
                    selectorX = (selectorX - 1);
                }
                chageButtonColor(selections[selectorY][selectorX]);
            }
            if (state && @event.IsActionPressed(string.Format("down{0}", playerNum)))
            {
                if (selectorY == selections.Count - 1)
                {
                    selectorY = 0;
                }
                else
                {
                    selectorY = (selectorY + 1);
                }
                chageButtonColor(selections[selectorY][selectorX]);
            }
            if (state && @event.IsActionPressed(string.Format("right{0}", playerNum)))
            {
                if (selectorX == selections[0].Count - 1)
                {
                    selectorX = 0;
                }
                else
                {
                    selectorX = (selectorX + 1);
                }
                chageButtonColor(selections[selectorY][selectorX]);
            }
            //GD.Print("\n", selectorY, selectorX);
            //Select and Erase
            if (state && @event.IsAction(string.Format("A{0}", playerNum)))
            {
                if (@event.IsActionPressed(string.Format("A{0}", playerNum)))
                {
                    if (selections[selectorY][selectorX] != '(' && selections[selectorY][selectorX] != ')')
                    {
                        if (name.Length < 6)
                        {
                            name += selections[selectorY][selectorX];
                            p1Label.Text = "Name: " + name;
                        }
                    }
                    else if (selections[selectorY][selectorX] == '(')
                    {
                        if (name.Length!=0)
                        {
                            //GD.Print("got to remove");
                            name=name.Remove(name.Length - 1);
                            p1Label.Text = "Name: " + name;
                        }
                    }
                    else if (selections[selectorY][selectorX] == ')')
                    {
                        if (name.Length != 0)
                        {
                            charState = true;
                            p1keyboard.Hide();
                            selectorEdit = 0;
                            p1CharEdit.Show();
                            p1CharEdit.GetNode<Button>("HeadChange").Modulate=p1Color;

                        }
                    }
                }
            }
            if (state && @event.IsAction(string.Format("B{0}", playerNum)))
            {
                if (@event.IsActionPressed(string.Format("B{0}", playerNum)))
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        name=name.Remove(name.Length - 1);
                        p1Label.Text = "Name: " + name;
                    }
                }
            }
        }
    }
}

                