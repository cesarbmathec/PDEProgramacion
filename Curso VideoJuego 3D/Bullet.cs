using Godot;
using System;

public partial class Bullet : Area3D
{

	[Export]
	public float speed = 45f;
	[Export]
	public float damage = 5f;

	private PackedScene effect;

	public override void _Ready()
	{
		effect = GD.Load<PackedScene>("res://Scenes/Effect.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= Transform.Basis.Z * speed * (float)delta;
	}

	private void OnBodyEntered(Node3D body)
	{
		GpuParticles3D impactEffect = (GpuParticles3D)effect.Instantiate();

		GetTree().Root.AddChild(impactEffect);

		impactEffect.GlobalPosition = GlobalPosition;
		impactEffect.GlobalRotation = GlobalRotation;

		impactEffect.OneShot = true;
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
