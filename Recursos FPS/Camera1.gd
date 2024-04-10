extends Camera3D
class_name CameraShake

# Variable para controlar el trauma
@export var trauma: float = 5
@export var trauma_reduction_rate: float = 1

# Declaramos 3 variables para controlar al trauma en cada eje
@export var max_x: float = 5
@export var max_y: float = 5
@export var max_z: float = 5

# Variable para generar un ruido o patrón de ruido
@export var noise: FastNoiseLite
# Velocidad del ruido
@export var noise_speed: float = 50
# Tiempo transcurrido
var time: float = 0

@onready var initial_rotation: Vector3 = rotation_degrees as Vector3
@export var weaponHolder: Node3D

@export var textureRect1: TextureRect
@export var textureRect2: TextureRect


func _ready():
	make_current()
	textureRect1.visible = true


func _process(delta):
	if Input.is_action_pressed("zoom"):
		position.z = lerp(position.z, -1.0, delta * 20)
		fov = lerp(fov, 15.0, delta * 20)
		textureRect1.visible = false
		textureRect2.visible = true
		textureRect2.rotation = lerp(textureRect2.rotation, 0.0, delta * 20)
		weaponHolder.position.x = lerp(weaponHolder.position.x, -0.40, delta * 20)
	else:
		position.z = lerp(position.z, 0.0, delta * 20)
		fov = lerp(fov, 75.0, delta * 20)
		textureRect1.visible = true
		textureRect2.visible = false
		textureRect2.rotation = lerp(textureRect2.rotation, PI / 3, delta * 20)
		weaponHolder.position.x = lerp(weaponHolder.position.x, 0.0, delta * 20)


func _physics_process(delta):
	time += delta

	trauma = max(trauma - delta * trauma_reduction_rate, 0)
	rotation_degrees.x = (initial_rotation.x + max_x * get_shake_intensity() * get_noise_from_seed(0))
	rotation_degrees.y = (initial_rotation.y + max_y * get_shake_intensity() * get_noise_from_seed(1))
	rotation_degrees.z = (initial_rotation.z + max_z * get_shake_intensity() * get_noise_from_seed(2))


func get_shake_intensity():
	return trauma * trauma


func get_noise_from_seed(_seed: int):
	noise.seed = _seed
	return noise.get_noise_1d(time * noise_speed)


func add_trauma(trauma_amount: float):
	trauma = clamp(trauma + trauma_amount, 0, 0.5)
