using Godot;
using System;
using System.IO;

public partial class SoundLib : ResourcePreloader
{
	private AudioStreamOggVorbis openMusic;

	//Locations of Audio Here

	private AudioStreamOggVorbis getFileBuffer(string path)
	{
		return GD.Load<AudioStreamOggVorbis>(string.Format("res://", path));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Preload here


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
