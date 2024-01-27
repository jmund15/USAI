using Godot;
using System;

public enum InteractableType
{
    COLLECTABLE,
    SWITCH // EXAMPLES, KEEP ADDING ON
}

[GlobalClass]
public partial class Interactable : RigidBody2D
{
    [Export]
    public InteractableType interactableType { get; protected set; }

    protected Sprite2D Sprite;
    protected Vector2 ObjectSize;
    protected Player Player;
    protected Sprite2D InteractIcon;
    protected CollisionShape2D CollisionShape;

    protected bool JustInteracted = false;

    private int _contactsReported = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        AddToGroup("Interactable");

        Sprite = GetNode<Sprite2D>("Sprite2D");
        ObjectSize = new Vector2(Sprite.Texture.GetHeight() / Sprite.Hframes, Sprite.Texture.GetWidth() / Sprite.Vframes);

		InteractIcon = GetNode<Sprite2D>("grabIcon");
		CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        MaxContactsReported = _contactsReported;
		FreezeMode = FreezeModeEnum.Static; // should be kinematic but having issues!!!
        CanSleep = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

  //  public void OnPromptPlayerInteract(bool showPrompt)
  //  {
		//InteractIcon.Visible = showPrompt;
  //  }
	//public void OnPlayerGrabObject(Player player)
	//{
	//	Freeze = true;
 //       JustInteracted = true;
 //       GD.Print("interacted interactable");
 //   }
}
