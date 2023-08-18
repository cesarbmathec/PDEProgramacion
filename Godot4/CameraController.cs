using Godot;
using System;

public partial class CameraController : Node3D
{
	private CharacterBody3D player;

	[Export]
	public float SensitiveHorizontal = 0.01f;
	[Export]
	public float SensitiveVertical = 0.01f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetNodesInGroup("Player")[0] as CharacterBody3D;

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = player.GlobalPosition;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			float aux = Rotation.X - eventMouseMotion.Relative.Y * SensitiveVertical;

			aux = Mathf.Clamp(aux, -0.7f, 0.5f);
			Rotation = new(
				aux,
				Rotation.Y - eventMouseMotion.Relative.X * SensitiveHorizontal,
				Rotation.Z
			);
		}
	}
}
