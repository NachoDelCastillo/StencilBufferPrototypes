using Core.FSM;
using Core.CustomInput;
using Game.Entities.Base;
using Game.Entities.Player.States;
using UnityEngine;

namespace Game.Entities.Player
{
    /// <summary>
    /// Controlador del jugador.
    /// </summary>
    public class PlayerController : EntityController<PlayerContext>
    {
        #region Variables

        [SerializeField] private PlayerStats stats;

        private PlayerInputHandler inputHandler;
        private EntityGfxTransform gfxTransform;
        public PlayerStats Stats => stats;

        /// <summary>
        /// Acceso público a los eventos del jugador.
        /// </summary>
        public PlayerEvents PlayerEvents => ctx.PlayerEvents;

        #endregion

        #region Initialize

        /// <summary>
        /// Inicializa el controlador con el input y el componente de movimiento.
        /// Registra los estados de esta entidad.
        /// </summary>
        public void Initialize(EntityGfxTransform gfxTransform, EntityMovementComponent moveComponent)
        {
            this.inputHandler = PlayerInputHandler.Instance;
            this.gfxTransform = gfxTransform;

            base.Initialize(moveComponent);
        }

        public void LateInitialize()
        {
            stateMachine.ChangeState<IdlePlayerState>();
        }

        /// <summary>
        /// Añade los estados que se usaran para esta entidad
        /// </summary>
        protected override void AddStates(StateMachine stateMachine)
        {
            // Registrar estados del jugador
            stateMachine.AddState(new IdlePlayerState(stateMachine, ctx));
            stateMachine.AddState(new RunPlayerState(stateMachine, ctx));
            stateMachine.AddState(new AirPlayerState(stateMachine, ctx));
            stateMachine.AddState(new TeleportingState(stateMachine, ctx));
        }

        /// <summary>
        /// Crea el contexto del jugador con estadísticas, input y máquina de estados.
        /// </summary>
        protected override PlayerContext CreateContext(StateMachine stateMachine)
        {
            return new PlayerContext(stats, inputHandler, moveComponent, gfxTransform, stateMachine);
        }

        #endregion

        #region Monobehaviour

        protected override void Update()
        {
            HandleBuffers();
            base.Update();
        }

        #endregion

        #region Update Flags and Buffers

        protected override void UpdateCollisionFlags()
        {
            base.UpdateCollisionFlags();

            // Actualizar flags específicos del jugador al contexto para que los estados tengan acceso
            ctx.onGroundFlag = moveComponent.OnGround;
        }

        /// <summary>
        /// Gestiona los buffers y actualiza los timers.
        /// </summary>
        private void HandleBuffers()
        {
            if (inputHandler.IsPressed(PlayerInputAction.Jump))
            {
                ctx.JumpBuffer = stats.JumpBufferTime;
            }

            if (inputHandler.IsReleased(PlayerInputAction.Jump) && ctx.JumpBuffer > 0)
            {
                ctx.JumpReleasedDuringBuffer = true;
            }

            UpdateTimers();
        }

        /// <summary>
        /// Actualiza los timers de buffers
        /// </summary>
        private void UpdateTimers()
        {
            ctx.JumpBuffer = Mathf.Max(0f, ctx.JumpBuffer - Time.deltaTime, 0f);

            // Cuando el tiempo del buffer de este salto se agota, el flag de "JumpReleasedDuringBuffer" ya no es valido
            if (ctx.JumpBuffer == 0)
                ctx.JumpReleasedDuringBuffer = false;

            if (moveComponent.OnGround && ctx.MovementVelocity.y <= 0)
                ctx.CoyoteBuffer = stats.CoyoteBufferTime;
            else
                ctx.CoyoteBuffer -= Time.deltaTime;

            //Debug.Log("JumpBuffer = " + ctx.JumpBuffer);
            //Debug.Log("CoyoteBuffer = " + ctx.CoyoteBuffer);
        }

        #endregion

        #region Interactions

        public void EnterTeleport() => ctx.EnterTeleport();
        public void ExitTeleport() => ctx.ExitTeleport();

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (stats == null)
                Debug.LogWarning("PlayerStats not referenced in PlayerController", this);
        }
#endif
    }
}