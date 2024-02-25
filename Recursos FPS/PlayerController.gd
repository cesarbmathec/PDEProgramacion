extends CharacterBody3D

@export_group("Player")
@export var speed = 5.0
@export var jump_velocity = 4.5

@export_group("Camera")
@export var cam_controller: Node3D
@export var cam_speed: float = 1
@export var cam_rotation_amount: float = 1
@export var camera: Camera3D

@export_group("Weapon")
@export var weapon_holder: Node3D
@export var weapon_rotation_amount: float = 1
@export var weapon_sway_amount: float = 1
@export var wobble_amount: float = 1
@export var wobble_freq: float = 1

var mouse_input: Vector2
var weapon_holder_pos: Vector3

enum player_state {
	IDLE,
	FIRE,
	RELOAD,
}

var Current_state: player_state = player_state.IDLE

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")


# Inicialización
func _ready():
	# Capturamos el cursor del mouse
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

	# Capturamos la posición del porta armas
	if weapon_holder:
		weapon_holder_pos = weapon_holder.position


# Entradas
func _input(event):
	if !cam_controller:
		return
	if event is InputEventMouseMotion:
		cam_controller.rotation.x -= event.relative.y * cam_speed
		cam_controller.rotation.x = clamp(cam_controller.rotation.x, -1.25, 1.5)
		rotation.y -= event.relative.x * cam_speed
		mouse_input = event.relative


func _process(delta):
	match Current_state:
		player_state.IDLE:
			idle()
		player_state.FIRE:
			fire(delta)
		player_state.RELOAD:
			reload()


func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta

	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = jump_velocity

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_dir = Input.get_vector("left", "right", "up", "down")
	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		velocity.x = direction.x * speed
		velocity.z = direction.z * speed
	else:
		velocity.x = move_toward(velocity.x, 0, speed * 0.1)
		velocity.z = move_toward(velocity.z, 0, speed * 0.1)

	move_and_slide()
	cam_tilt(input_dir.x, delta)
	weapon_tilt(input_dir.x, delta)
	weapon_sway(delta)
	weapon_wobble(velocity.length(), delta)


# Métodos auxiliares
func cam_tilt(input_x, delta):
	if cam_controller:
		cam_controller.rotation.z = lerp(cam_controller.rotation.z, -input_x * cam_rotation_amount, 10 * delta)


func weapon_tilt(input_x, delta):
	if weapon_holder:
		weapon_holder.rotation.z = lerp(weapon_holder.rotation.z, -input_x * weapon_rotation_amount, 10 * delta)


func weapon_sway(delta):
	mouse_input = lerp(mouse_input, Vector2.ZERO, delta)
	weapon_holder.rotation.x = lerp(weapon_holder.rotation.x, mouse_input.y * weapon_sway_amount, 10 * delta)
	weapon_holder.rotation.y = lerp(weapon_holder.rotation.y, mouse_input.x * weapon_sway_amount, 5 * delta)


func weapon_wobble(vel: float, delta):
	if weapon_holder:
		if vel > 0 and is_on_floor():
			weapon_holder.position.y = lerp(
				weapon_holder.position.y, weapon_holder_pos.y + sin(Time.get_ticks_msec() * wobble_freq) * wobble_amount, 10 * delta
			)
			weapon_holder.position.x = lerp(
				weapon_holder.position.x, weapon_holder_pos.x + sin(Time.get_ticks_msec() * wobble_freq * 0.5) * wobble_amount, 10 * delta
			)
		else:
			weapon_holder.position.y = lerp(weapon_holder.position.y, weapon_holder_pos.y, 10 * delta)
			weapon_holder.position.x = lerp(weapon_holder.position.x, weapon_holder_pos.x, 10 * delta)


# Métodos para controlar los estados del player
func idle():
	pass


func fire(_delta):
	pass


func reload():
	pass
