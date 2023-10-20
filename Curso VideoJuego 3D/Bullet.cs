using Godot;
using System;

public partial class Bullet : Area3D
{

	[Export]
	public float speed = 45f;
	[Export]
	public float damage = 5f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= Transform.Basis.Z * speed * (float)delta;
	}

	private void OnBodyEntered()
	{
		GD.Print("He impactado con algo!!");
		Destroy();
	}

	private void OnTimerTimeout()
	{
		Destroy();
	}

	public void Destroy()
	{
		QueueFree();
	}
}
