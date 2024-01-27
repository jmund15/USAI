using Godot;
using System;
using System.Reflection.PortableExecutable;

public sealed partial class Player : TopDownCharacter
{
    #region CLASS_VARIABLES
    private const float _humanForce = 500;

    #endregion

    #region BASE_GODOT_OVERRIDEN_FUNCTIONS

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("init player");
        base._Ready();
        WalkSpeed = 5000;
        RunSpeed = 8000;

        AddToGroup("Player");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        FaceDirection = (MovementDirection)Sprite.FrameCoords.X;
    }
    public override void _PhysicsProcess(double delta)
    {
        HumanMovementController(delta);
        HumanAnimationController();
    }
    #endregion

    #region MOVEMENT_AND_ANIMATION_CONTROLLERS
    private void HumanMovementController(double delta)
    {
        Direction = Input.GetVector("left", "right", "up", "down").Normalized();
        Velocity = Direction * WalkSpeed * (float)delta;
        MoveAndSlide();
    }
    private void HumanAnimationController()
    {
        if (Velocity.Length() == 0)
        {
            //TODO: IN THE FUTURE MAKE THE NEXT STATEMENTS ELSE IF AND ENABLE SOME FACE ANIMATIONS BASED ON "FACEDIRECTION" (i.e. "faceLeft")
            AnimPlayer.Stop();
        }
        if (Velocity.X < 0 && (Math.Abs(Direction.X) - Math.Abs(Direction.Y)) >= 0)
        {
            AnimPlayer.Play("walkLeft");
        }
        else if (Velocity.X > 0 && (Math.Abs(Direction.X) - Math.Abs(Direction.Y)) >= 0)
        {
            AnimPlayer.Play("walkRight");
        }
        else if (Velocity.Y < 0)
        {
            AnimPlayer.Play("walkUp");
        }
        else
        {
            AnimPlayer.Play("walkDown");
        }
    }
    #endregion

    #region SIGNAL_LISTENERS
    
    private void OnGrabRangeBodyEntered(Node2D body)
    {
        if (body == this)
        {
            return; // don't use self
        }
        if (body is TileMap)
        {
            return; //This seems good, probably anything tilemapped isn't interactable
        }
        PhysicsBody2D physicsBody = body as PhysicsBody2D; if (physicsBody == null) { GD.PrintErr(nameof(OnGrabRangeBodyEntered) + " ERROR || " + "body isn't tilemap or phsyics body????");  return; } // should be impossible

        else if (physicsBody.CollisionLayer == 4 && physicsBody is Interactable)
        {
            var interactable = physicsBody as Interactable;
            GD.Print("prompt emit");
        }
    }
    private void OnGrabRangeBodyExited(Node2D body)
    {
        if (body == this)
        {
            return; // don't use self
        }
        if (body is TileMap)
        {
            return; //This seems good, probably anything tilemapped isn't interactable
        }
        PhysicsBody2D physicsBody = body as PhysicsBody2D; if (physicsBody == null) { GD.PrintErr(nameof(OnGrabRangeBodyExited) + " ERROR || " + "body isn't tilemap or phsyics body????"); return; } // should be impossible
    }
 
    #endregion
    #region HELPER_FUNCITONS

    private Vector2 GetRandomDirection()
    {
        var direction = new Vector2();
        var randNeg = Rnd.Next(0, 4);
        switch (randNeg)
        {
            case 0:
                direction = new Vector2(Rnd.NextSingle(), Rnd.NextSingle()); //(MovementDirection)Rnd.Next(0, 4);
                break;
            case 1:
                direction = new Vector2(-Rnd.NextSingle(), Rnd.NextSingle());
                break;
            case 2:
                direction = new Vector2(Rnd.NextSingle(), -Rnd.NextSingle());
                break;
            case 3:
                direction = new Vector2(-Rnd.NextSingle(), -Rnd.NextSingle());
                break;
            default:
                break;
        }
        return direction;
    }
    #endregion

}
