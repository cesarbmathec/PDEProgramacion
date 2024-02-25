extends Camera3D

@export var Camera1: Camera3D


func _process(_delta):
	global_transform = Camera1.global_transform
