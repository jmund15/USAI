using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Map : Node2D
{
	public List<string> ConnectedMaps { get; protected set; }

    protected Global Global { get; private set; }
    protected Events SignalBus { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup("Map");
        SignalBus = GetNode<Events>("/root/Events");
        Global = GetNode<Global>("/root/Global");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
