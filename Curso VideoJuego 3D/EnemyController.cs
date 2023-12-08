using Godot;

public partial class EnemyController : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	private Vector3 direction;

	private NavigationAgent3D navigationAgent3D;

	[Export]
	public Node3D target;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		navigationAgent3D = GetNode<NavigationAgent3D>("NavigationAgent3D");
		navigationAgent3D.TargetPosition = target.GlobalPosition;
	}

	public override void _Process(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (!(navigationAgent3D.IsNavigationFinished() && navigationAgent3D.IsTargetReached()))
		{
			LookAt(new Vector3(target.GlobalPosition.X, Position.Y, target.GlobalPosition.Z));
			direction = navigationAgent3D.GetNextPathPosition() - GlobalPosition;
			direction = direction.Normalized();
		}
		else
		{
			direction = Vector3.Zero;
		}

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
