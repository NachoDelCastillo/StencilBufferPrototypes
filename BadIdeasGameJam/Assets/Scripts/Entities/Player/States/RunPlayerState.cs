using Core.FSM;
using UnityEngine;

namespace Game.Entities.Player.States
{
    /// <summary>
    /// Estado de movimiento en tierra del jugador.
    /// </summary>
    public class RunPlayerState : State<PlayerContext>
    {
        #region Constructor

        public RunPlayerState(StateMachine sm, PlayerContext ctx) : base(sm, ctx) { }

        #endregion

        #region State Methods

        public override void Update()
        {
            // Transicion a Idle si no hay input horizontal suficiente
            if (ctx.InputComponent.MoveInputMagnitude < ctx.Stats.MovementDeadZone)
            {
                stateMachine.ChangeState<IdlePlayerState>();
                return;
            }

            // Transicion a estado de aire
            if (ctx.TryAirborne()) return;

            // Comprobar salto
            if (ctx.TryJump()) return;

            // Rotar gfx
            ctx.UpdateGfxTargetDirection();
        }

        public override void FixedUpdate()
        {
            // Aplica movimiento horizontal en el suelo seg�n input del jugador.
            ctx.HandleGroundHorizontalMovement(ctx.InputComponent.MoveInput);
        }

        #endregion
    }
}