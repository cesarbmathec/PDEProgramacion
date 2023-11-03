using Godot;
using System;

public partial class Weapon : Node3D
{

	private bool isFiring = false;

	public Vector3 lookAt;

	private GpuParticles3D gpuParticles3D;

	public override void _Ready()
	{
		gpuParticles3D = GetNode<GpuParticles3D>("Origin/GPUParticles3D");
	}

	public void StartFiring()
	{
		isFiring = true;
		gpuParticles3D.Emitting = true;
	}

	public void StopFiring()
	{
		isFiring = false;
		gpuParticles3D.Emitting = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LookAt(lookAt);
	}
}
