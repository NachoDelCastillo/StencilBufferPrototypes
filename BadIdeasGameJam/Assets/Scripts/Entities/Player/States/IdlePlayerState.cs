using Core.FSM;
using UnityEngine;

namespace Game.Entities.Player.States
{
    /// <summary>
    /// Estado de reposo del jugador.
    /// 
    /// - Decelerar horizontalmente hasta llegar 
    /// - Transición a estado de aire si el jugador deja el suelo.
    /// - Intento de salto
    /// </summary>
    public class IdlePlayerState : State<PlayerContext>
    {
        #region Constructor

        public IdlePlayerState(StateMachine sm, PlayerContext ctx) : base(sm, ctx) { }

        #endregion

        #region State Methods

        public override void Update()
        {
            // Transicion a Walk si hay input horizontal suficiente
            if (ctx.InputComponent.MoveInputMagnitude > ctx.Stats.MovementDeadZone)
            {
                stateMachine.ChangeState<RunPlayerState>();
                return;
            }

            // Transicion a estado de aire
            ctx.TryAirborne();

            // Desacelera al jugador horizontalmente.
            ctx.BrakeHorizontalMovement();

            // Comprobar salto
            if (ctx.TryJump()) return;
        }

        #endregion
    }
}