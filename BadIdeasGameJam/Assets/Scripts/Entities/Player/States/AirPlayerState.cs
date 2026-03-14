using Core.FSM;
using Core.CustomInput;
using UnityEngine;

namespace Game.Entities.Player.States
{
    /// <summary>
    /// Estado de aire del jugador.
    /// 
    /// - Movimiento horizontal y vertical en el aire.
    /// - Corte de salto al soltar el botón de salto.
    /// - Transición a aterrizaje cuando toca el suelo.
    /// - Eventos de inicio de caída.
    /// </summary>
    public class AirPlayerState : State<PlayerContext>
    {
        #region Variables

        // Registro de la velocidad vertical en el frame anterior
        float previousVerticalVelocity;

        #endregion

        #region Constructor

        public AirPlayerState(StateMachine sm, PlayerContext context) : base(sm, context) { }

        #endregion

        #region State Methods

        public override void OnEnter()
        {
            // Guarda la velocidad vertical inicial para detectar cambios de dirección.
            previousVerticalVelocity = ctx.MovementVelocity.y;
        }

        public override void Update()
        {
            if (ctx.MovementVelocity.y > 0)
            {
                // Corte de salto al soltar el botón
                if (ctx.InputComponent.IsReleased(PlayerInputAction.Jump))
                {
                    ctx.SetMovementVelocityYAxis(ctx.MovementVelocity.y * ctx.Stats.SmallJumpMultiplier);
                }
            }
            else
            {
                // Comprobar si se ha aterrizado
                if (ctx.TryLanding()) return;

                // Evento de inicio de caída
                if (previousVerticalVelocity >= 0)
                {
                    ctx.PlayerEvents.OnStartFallingDown?.Invoke();
                }
            }

            // Es necesario para comprobar coyote time para los saltos
            // Tambien en caso de que haya doble salto o superior
            ctx.TryJump();

            // Rotar gfx
            ctx.UpdateGfxTargetDirection();

            previousVerticalVelocity = ctx.MovementVelocity.y;
        }

        /// <summary>
        /// Aplica gravedad y maneja movimiento horizontal en el aire.
        /// </summary>
        public override void FixedUpdate()
        {
            // Si hay suficiente input, aplicar aceleraciones al jugador
            if (ctx.InputComponent.MoveInputMagnitude > ctx.Stats.MovementDeadZone)
            {
                ctx.HandleAirHorizontalMovement(ctx.InputComponent.MoveInput);
            }
            else
                ctx.BrakeHorizontalMovement();

            // Aplicar un multiplicador de gravedad para el descenso
            float gravityMultiplier = ctx.MovementVelocity.y > 0 ? 1 : ctx.Stats.GravityOnFallMultiplier;
            ctx.ApplyGravity(gravityMultiplier);
        }

        #endregion
    }
}