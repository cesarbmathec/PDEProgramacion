using System;
using Godot;

namespace TutorialGame
{
    [Obsolete]
    public partial class EnemyController : CharacterBody3D
    {
        // Métodos para controlar los estados del enemigo
        public void PatrolState()
        {
            // Color verde
            SetColor(new Color(0, 1, 0));
            // Hemos llegado al punto de patrulla?
            if (WeHaveArrived())
            {
                // Generamos un número aleatorio entero para establecer los puntos a patrullar
                uint index = GD.Randi() % (uint)targetPoints.Length;
                // Actualizamos el nuevo punto de patrulla
                targetPoint = targetPoints[index].GlobalPosition;
            }

            // Verificamos si el player entra en el radio de busqueda
            if (distance < searchRadius)
            {
                // Pasamos al estado Alerta
                currentState = EnemyState.ALERT;
                return;
            }

            // Podemos ver al jugador?
            if (WatchingPlayer())
            {
                // Pasamos al estado persecución
                currentState = EnemyState.PURSUIT;
                return;
            }

            // Movemos y rotamos el personaje
            UpdateLookAtAndDirection(targetPoint);
            MoveAndSlideAgent();

            // Establecer las animaciones para este estado
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/running", true);
            animationTree.Set("parameters/conditions/attack", false);
        }

        public void AlertState(float time)
        {
            // Color ?
            SetColor(new Color(1, 1, 0));

            // Detenemos al enemigo para que entre en el estado alerta
            StopAgent();

            // Rotamos al enemigo (gira sobre si)
            Rotate(Vector3.Up, speedRotation * time);
            timeElapsed += time;

            // Si pasado algún tiempo no encontramos al enemigo, entremos al estado patrulla
            if (timeElapsed >= searchTime)
            {
                // Entramos al estado Patrulla
                currentState = EnemyState.PATROL;
                timeElapsed = 0f;
                return;
            }

            // Varificamos si podemos ver al jugador
            if (WatchingPlayer())
            {
                // Pasamos al estado persecución
                currentState = EnemyState.PURSUIT;
                return;
            }

            // Establecer las animaciones para este estado
            animationTree.Set("parameters/conditions/idle", true);
            animationTree.Set("parameters/conditions/running", false);
            animationTree.Set("parameters/conditions/attack", false);

        }

        public void PursuitState()
        {
            // Color azul
            SetColor(new Color(0, 0, 1));

            // Detenemos al Agente
            StopAgent();

            // Nos movemos hacia la nueva posición
            UpdateLookAtAndDirection(player.GlobalPosition);
            MoveAndSlideAgent();

            // Si ya no vemos al player o se aleja, entramos al estado Alerta
            if (!WatchingPlayer())
            {
                currentState = EnemyState.ALERT;
                return;
            }

            // Verificamos si estamos cerca para atacar
            if (distance < 1.3f)
            {
                currentState = EnemyState.ATTACK;
                return;
            }

            // Establecer las animaciones para este estado
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/running", true);
            animationTree.Set("parameters/conditions/attack", false);
        }



        public void AttackState()
        {
            // Color rojo
            SetColor(new Color(1, 0, 0));

            // Quitamos vida al player
            var pl = (Player)player;
            pl.life -= 0.1f;

            // Verificamos si nos hemos alejado para entrar a persecución
            if (distance > 1.5f)
            {
                // Estado persecución
                currentState = EnemyState.PURSUIT;
                return;
            }

            // Establecer las animaciones para este estado
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/running", false);
            animationTree.Set("parameters/conditions/attack", true);
        }

        

    }
}