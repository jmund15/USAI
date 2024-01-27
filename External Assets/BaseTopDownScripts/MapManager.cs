using Godot;
using System;
using System.Collections.Generic;

public partial class MapManager : Node
{
    private Global _global;
    private Events _signalBus;

    private HUD _hud;
    private MainMenu _mainMenu;
    private PauseMenu _pauseMenu;
    public List<Node> MapsLoaded { get; private set; }

    // Called when the node enters the map tree for the first time.
    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _signalBus = GetNode<Events>("/root/Events");

        var rootNodePath = "//root/MainScene/";

        _mainMenu = GetNode<MainMenu>(rootNodePath + "CanvasLayer/MainMenu");
        _hud = GetNode<HUD>(rootNodePath + "CanvasLayer/HUD");
        _pauseMenu = GetNode<PauseMenu>(rootNodePath + "CanvasLayer/PauseMenu");

        var initialScenePath = "res://Levels/level.tscn";
        _mainMenu.StartButton.Pressed += () => LoadMap(initialScenePath);
        //_signalBus.LoadMap += OnLoadMap;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void LoadMap(string mapPath)
    {
        //var loadingMap = ResourceLoader.LoadThreadedRequest(mapName, useSubThreads: true, cacheMode: ResourceLoader.CacheMode.Reuse);
        //await loadingMap;

        var map = ResourceLoader.Load<PackedScene>(mapPath);
        GD.Print(map.ResourceName);
        var loadedMap = map.Instantiate() as Map;
        if (loadedMap == null) { GD.PrintErr("ERROR || loaded map is not of type 'Map'! Object name: " + loadedMap.Name); return; }
        
        AddChild(loadedMap);
        //MapsLoaded.Add(loadedMap);
    }
}
