using Core.FSM;
using UnityEngine;

namespace Game.Entities.Player.States
{
    /// <summary>
    /// Estado de movimiento en tierra del jugador.
    /// </summary>
    public class TeleportingState : State<PlayerContext>
    {
        #region Constructor

        public TeleportingState(StateMachine sm, PlayerContext ctx) : base(sm, ctx) { }

        #endregion

        #region State Methods

        public override void OnEnter()
        {
            ctx.MovementVelocity = Vector3.zero;
        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {

        }

        #endregion
    }
}