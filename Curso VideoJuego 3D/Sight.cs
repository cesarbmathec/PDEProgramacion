using Godot;
using System;

public partial class Sight : Control
{

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
	}
}
