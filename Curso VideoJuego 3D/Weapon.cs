using Godot;
using System;

public partial class Weapon : Node3D
{

	private bool isFiring = false;

	public Vector3 lookAt;

	public void StartFiring()
	{
		isFiring = true;
	}

	public void StopFiring()
	{
		isFiring = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LookAt(lookAt);
	}
}
