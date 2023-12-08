using Godot;
using System;

[Obsolete]
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

	private Node3D origin;
	private PackedScene bulletScene;
	private float timeElapsed = 0f;
	private Weapon weaponClass;

	[Export]
	public Node3D weapon;

	[Export]
	public SkeletonIK3D leftIK;
	[Export]
	public SkeletonIK3D rightIK;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D lookAt;

	private Control sight;

	public override void _Ready()
	{
		lookAt = GetTree().GetNodesInGroup("CameraController")[0].GetNode<Node3D>("LookAt");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationTree.Active = true;

		weaponClass = GetNode<Weapon>("Weapon");
		origin = GetNode<Node3D>("Weapon/AK-47/Origin");
		bulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/Bullet.tscn");
		sight = GetNode<Control>("Sight");

		sight.Modulate = new Color(1f, 1f, 1f, 0.25f);
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

			newDir = newDir.Lerp(new Vector2(inputDir.X, -inputDir.Y).Normalized(), 0.25f);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);

			newDir = newDir.Lerp(Vector2.Zero, 0.25f);
		}

		lerpDirection = lerpDirection.Lerp(
			new(
				lookAt.GlobalPosition.X,
				GlobalPosition.Y,
				lookAt.GlobalPosition.Z
			),
			0.35f
		);

		LookAt(lerpDirection);

		if (aiming)
		{
			animationTree.Set("parameters/TransitionStrafeAiming/transition_request", "aiming");
			animationTree.Set("parameters/StrafeAiming/blend_position", newDir);
			animationTree.Set("parameters/Jumping/conditions/aiming", true);
			leftIK.Start();
			rightIK.Start();
			weapon.Visible = true;
			sight.Visible = true;
			if (Input.IsActionPressed("shoot") && timeElapsed > 0.1f)
			{
				//timeElapsed += (float)delta;
				Shoot();
				weaponClass.StartFiring();
				timeElapsed = 0f;
			}
			else
			{
				timeElapsed += (float)delta;
				if (timeElapsed > 0.1f)
					weaponClass.StopFiring();
			}
		}
		else
		{
			animationTree.Set("parameters/TransitionStrafeAiming/transition_request", "strafe");
			animationTree.Set("parameters/Strafe/blend_position", newDir);
			animationTree.Set("parameters/Jumping/conditions/aiming", false);
			leftIK.Stop();
			rightIK.Stop();
			weapon.Visible = false;
			sight.Visible = false;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void Shoot()
	{
		Area3D bullet = (Area3D)bulletScene.Instantiate();
		weapon.AddChild(bullet);
		if (origin != null)
		{
			bullet.GlobalTransform = origin.GlobalTransform;
			bullet.Scale = Vector3.One * 0.8f;
		}
	}
}
