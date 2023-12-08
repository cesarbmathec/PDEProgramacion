using Godot;
using Godot.Collections;

public partial class CameraController : Node3D
{
	private CharacterBody3D player;
	[Export]
	public float SensitiveHorizontal = 0.01f;
	[Export]
	public float SensitiveVertical = 0.01f;

	[Export]
	public float rayLength = 45f;

	// Esta variable va a almacenar código
	private Weapon weaponClass;

	private Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetNodesInGroup("Player")[0] as CharacterBody3D;

		weaponClass = GetTree().GetNodesInGroup("Player")[0].GetNode<Weapon>("Weapon");
		camera = GetNode<Camera3D>("SpringArm3D/Camera3D");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Capturamos el espacio
		var spaceState = GetWorld3D().DirectSpaceState;

		// Capturamos el vector posición de las coord. del cursor
		Vector2 mousePos = GetViewport().GetMousePosition();
		if (camera != null)
		{
			// Proyectamos un rayo desde la camara
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
			// Proyectamos el rayo con logitud de 45
			Vector3 rayEnd = rayOrigin + camera.ProjectRayNormal(mousePos) * rayLength;
			var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
			query.Exclude = new Array<Rid> { player.GetRid() };

			Dictionary collisionQuery = spaceState.IntersectRay(query);

			Vector3 collisionPosition = rayEnd;

			if (collisionQuery.Count > 0)
			{
				collisionPosition = (Vector3)collisionQuery["position"];
			}

			if (weaponClass != null)
			{
				weaponClass.lookAt = collisionPosition;
			}
		}

		GlobalPosition = player.GlobalTransform.Origin;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			float aux = Rotation.X - eventMouseMotion.Relative.Y * SensitiveVertical / 100f;

			aux = Mathf.Clamp(aux, -0.7f, 0.3f);

			Rotation = new(
				aux,
				Rotation.Y - eventMouseMotion.Relative.X * SensitiveHorizontal / 100f,
				Rotation.Z
			);
		}
	}
}
