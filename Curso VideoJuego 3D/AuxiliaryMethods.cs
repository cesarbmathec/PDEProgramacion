using Godot;
using Godot.Collections;

namespace TutorialGame
{
    public partial class EnemyController : CharacterBody3D
    {
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

        public void StopAgent()
        {
            direction = Vector3.Zero;
        }

        public bool WatchingPlayer()
        {
            var query = PhysicsRayQueryParameters3D.Create(eyes.GlobalPosition, eyes.GlobalPosition - GlobalTransform.Basis.Z * raycastLength);
            query.Exclude = new Array<Rid> { GetRid() };
            Dictionary collisionQuery = GetWorld3D().DirectSpaceState.IntersectRay(query);

            // Hay colision con objetos?
            if (collisionQuery.Count > 0)
            {
                // Tomamos al objeto
                Node collisionObject = (Node)collisionQuery["collider"];

                if (collisionObject.IsInGroup("Player"))
                {
                    return true;
                }
            }
            return false;
        }

        public void SetColor(Color color)
        {
            if (boxMesh.MaterialOverride is StandardMaterial3D material)
            {
                material.AlbedoColor = color;
            }
        }

        public void CheckDeath()
        {
            if (life > -20f && life < 0f)
            {
                StopAgent();
                // Animación de morir
                animationTree.Set("parameters/conditions/death", true);
                life = -30f;
                // Color negro
                SetColor(new Color(0, 0, 0));
            }
        }
    }
}