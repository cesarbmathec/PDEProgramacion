using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;

	private Vector3 lerpDirection;
	private AnimationTree animationTree;
	private Vector2 newDir;
	private bool aiming = false;

	[Export]
	public Node3D weapon;

	[Export]
	public SkeletonIK3D leftIK;
	[Export]
	public SkeletonIK3D rightIK;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D lookAt;

	public override void _Ready()
	{
		lookAt = GetTree().GetNodesInGroup("CameraController")[0].GetNode<Node3D>("LookAt");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationTree.Active = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
			animationTree.Set("parameters/TransitionStrafeJumping/transition_request", "Jumping");
			animationTree.Set("parameters/Jumping/conditions/IsOnFloor", false);
		}
		else
		{
			animationTree.Set("parameters/TransitionStrafeJumping/transition_request", "Strafe");
			animationTree.Set("parameters/Jumping/conditions/IsOnFloor", true);
		}

		if (Input.IsActionJustPressed("aiming"))
		{
			aiming = !aiming;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed * (float)delta * 15f, 0.25f);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed * (float)delta * 15f, 0.25f);

			lerpDirection = lerpDirection.Lerp(
				new(
					lookAt.GlobalPosition.X,
					GlobalPosition.Y,
					lookAt.GlobalPosition.Z
				),
				0.35f
			);

			LookAt(lerpDirection);

			newDir = newDir.Lerp(new Vector2(inputDir.X, -inputDir.Y).Normalized(), 0.25f);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);

			newDir = newDir.Lerp(Vector2.Zero, 0.25f);
		}

		if (aiming)
		{
			animationTree.Set("parameters/TransitionStrafeAiming/transition_request", "aiming");
			animationTree.Set("parameters/StrafeAiming/blend_position", newDir);
			animationTree.Set("parameters/Jumping/conditions/aiming", true);
			leftIK.Start();
			rightIK.Start();
			weapon.Visible = true;
		}
		else
		{
			animationTree.Set("parameters/TransitionStrafeAiming/transition_request", "strafe");
			animationTree.Set("parameters/Strafe/blend_position", newDir);
			animationTree.Set("parameters/Jumping/conditions/aiming", false);
			leftIK.Stop();
			rightIK.Stop();
			weapon.Visible = false;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
