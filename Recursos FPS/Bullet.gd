extends Area3D

@export var speed: float = 80
@export var damage: float = 5


# Called when the node enters the scene tree for the first time.
func _ready():
	pass  # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position -= transform.basis.z * speed * delta


# Función para destruir la bala
func destroy():
	queue_free()


func _on_timer_timeout():
	destroy()


func _on_body_entered(_body: Node3D):
	destroy()
