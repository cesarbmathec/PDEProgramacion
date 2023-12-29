using Godot;

namespace TutorialGame
{
	public partial class EnemyController : CharacterBody3D
	{
		// Variables de movimiento y física
		[ExportGroup("Física y Movimiento")]
		[Export]
		public float Speed = 5.0f;
		[Export]
		public float JumpVelocity = 4.5f;
		private Vector3 direction;
		[Export]
		public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
		private Vector3 targetPoint; // Almacena la posición del target actual
		[Export]
		public float searchRadius = 4f;
		[Export]
		public float speedRotation = 15f;
		[Export]
		public float searchTime = 4f;
		[Export]
		public float raycastLength = 10f;
		[Export]
		public float life = 10f;

		private float distance;
		private float timeElapsed = 0f;

		// Variables para navegación
		private NavigationAgent3D navigationAgent3D;

		[ExportGroup("Nodos Extras")]
		// Nodos extras
		[Export]
		public Node3D[] targetPoints; // Almacena un arreglo de los target
		private CharacterBody3D player;
		private Node3D eyes;
		private AnimationTree animationTree;
		private MeshInstance3D boxMesh;

		// Variables para controlar los estados
		public enum EnemyState
		{
			ALERT,
			PATROL,
			PURSUIT,
			ATTACK,
		}
		[ExportGroup("Manejo de Estados")]
		[Export]
		public EnemyState initialState = EnemyState.PATROL;
		private EnemyState currentState;

		public override void _Ready()
		{
			navigationAgent3D = GetNode<NavigationAgent3D>("NavigationAgent3D");
			player = GetTree().GetNodesInGroup("Player")[0] as CharacterBody3D;
			eyes = GetNode<Node3D>("Eyes");
			animationTree = GetNode<AnimationTree>("AnimationTree");
			boxMesh = GetNode<MeshInstance3D>("BoxMesh");

			animationTree.Active = true;
			// Establecemos el estado incial
			currentState = initialState;
		}

		public override void _Process(double delta)
		{
			CheckDeath();
			switch (currentState)
			{
				case EnemyState.PATROL:
					PatrolState();
					break;
				case EnemyState.ALERT:
					AlertState((float)delta);
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
			distance = player.Position.DistanceTo(Position);

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
	}
}
