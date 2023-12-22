using Godot;

public partial class EnemyController : CharacterBody3D
{

	// Variables de movimiento y física
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	private Vector3 direction;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Vector3 targetPoint; // Almacena la posición del target actual

	// Variables para navegación
	private NavigationAgent3D navigationAgent3D;

	// Nodos extras
	[Export]
	public Node3D[] targetPoints; // Almacena un arreglo de los target
	private CharacterBody3D player;
	private Node3D eyes;
	private AnimationTree animationTree;

	// Variables para controlar los estados
	public enum EnemyState
	{
		ALERT,
		PATROL,
		PURSUIT,
		ATTACK,
	}
	[Export]
	public EnemyState initialState = EnemyState.PATROL;
	private EnemyState currentState;

	public override void _Ready()
	{
		navigationAgent3D = GetNode<NavigationAgent3D>("NavigationAgent3D");
		player = GetTree().GetNodesInGroup("Player")[0] as CharacterBody3D;
		eyes = GetNode<Node3D>("Eyes");
		animationTree = GetNode<AnimationTree>("AnimationTree");

		animationTree.Active = true;
		// Establecemos el estado incial
		currentState = initialState;
	}

	public override void _Process(double delta)
	{
		switch (currentState)
		{
			case EnemyState.PATROL:
				PatrolState();
				break;
			case EnemyState.ALERT:
				AlertState();
				break;
			case EnemyState.PURSUIT:
				PursuitState();
				break;
			case EnemyState.ATTACK:
				AttackState();
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

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

	// Métodos para controlar los estados del enemigo
	public void PatrolState()
	{
		// Hemos llegado al punto de patrulla?
		if (WeHaveArrived())
		{
			// Generamos un número aleatorio entero para establecer los puntos a patrullar
			uint index = GD.Randi() % (uint)targetPoints.Length;
			// Actualizamos el nuevo punto de patrulla
			targetPoint = targetPoints[index].GlobalPosition;
		}
		// Movemos y rotamos el personaje
		UpdateLookAtAndDirection(targetPoint);
		MoveAndSlideAgent();

		// Establecer las animaciones para este estado
		animationTree.Set("parameters/conditions/idle", false);
		animationTree.Set("parameters/conditions/running", true);
		animationTree.Set("parameters/conditions/attack", false);
	}
	public void AlertState()
	{ }
	public void PursuitState()
	{ }
	public void AttackState()
	{ }

	// Métodos Auxiliares
	public void UpdateLookAtAndDirection(Vector3 p)
	{
		navigationAgent3D.TargetPosition = p;
		LookAt(new Vector3(p.X, Position.Y, p.Z));
	}

	public void MoveAndSlideAgent()
	{
		direction = navigationAgent3D.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
	}

	public bool WeHaveArrived()
	{
		return navigationAgent3D.IsNavigationFinished() && navigationAgent3D.IsTargetReached();
	}
}
