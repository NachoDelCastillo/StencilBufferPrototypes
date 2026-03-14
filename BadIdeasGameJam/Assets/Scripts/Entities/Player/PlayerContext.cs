using Core.FSM;
using Core.CustomInput;
using Game.Entities.Base;
using Game.Entities.Player.States;
using UnityEngine;
using System;
using Game.Gameplay;

namespace Game.Entities.Player
{
    /// <summary>
    /// Contexto del jugador que contiene todos los datos a los que pueden acceder los estados.
    /// </summary>
    public class PlayerContext : EntityContext
    {
        #region Variables

        // Classes
        public PlayerStats Stats { get; private set; }
        public PlayerInputHandler InputComponent { get; private set; }
        public EntityMovementComponent MoveComponent { get; private set; }
        public EntityGfxTransform GfxTransform { get; private set; }
        public CameraSimpleBehaviour CameraBehaviour { get; private set; }
        public PlayerEvents PlayerEvents { get; set; } = new();

        // Input Buffers
        public float JumpBuffer { get; set; }
        public float CoyoteBuffer { get; set; }
        public bool JumpReleasedDuringBuffer { get; set; }

        // Collision Flags
        public bool onGroundFlag;

        // Actions counter
        public int JumpsUsed { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Crea un contexto de jugador con sus estadísticas, componente de input y máquina de estados.
        /// </summary>
        /// <param name="stats">Estadísticas del jugador.</param>
        /// <param name="inputComponent">Componente que gestiona el input del jugador.</param>
        /// <param name="stateMachine">Máquina de estados asociada al jugador.</param>
        public PlayerContext(PlayerStats stats, PlayerInputHandler inputComponent, 
            EntityMovementComponent moveComponent, EntityGfxTransform gfxTransform,
            CameraSimpleBehaviour cameraBehaviour, StateMachine stateMachine)
            : base(stateMachine)
        {
            Stats = stats;
            InputComponent = inputComponent;
            MoveComponent = moveComponent;
            GfxTransform = gfxTransform;
            CameraBehaviour = cameraBehaviour;

            OnMovementVelocityChanged += UpdateHorizontalMovementEvent;
        }

        #endregion

        #region Horizontal Movement

        /// <summary>
        /// Lerpea la velocidad horizontal del personaje a 0 teniendo en cuenta la desaceleracion para esta iteracion
        /// Si estos ejes se van a aplicar al Rigidbody, esta funcion debe actualizarse desde un FixedUpdate
        /// </summary>
        public void BrakeHorizontalMovement()
        {
            float deceleration = onGroundFlag ? Stats.GroundDecceleration : Stats.AirDecceleration;

            Vector3 currentVelocity = MovementVelocity;
            currentVelocity.y = 0;
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);

            MovementVelocity = new Vector3(currentVelocity.x, MovementVelocity.y, currentVelocity.z);
        }

        /// <summary>
        /// Interpola la velocidad actual hacia la velocidad objetivo usando la aceleración/deceleración apropiada.
        /// Si estos ejes se van a aplicar al Rigidbody, esta funcion debe actualizarse desde un FixedUpdate
        /// </summary>
        private void HandleHorizontalMovement(Vector2 movementDirection, float maxSpeed, float acceleration, float deceleration)
        {
            Vector3 currentVelocity = MovementVelocity;

            Transform cameraTransform = Camera.main.transform;

            // Tener en cuenta el transform de la camara
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Contruir movimiento relativo a la camara
            Vector3 moveDir = camRight * movementDirection.x + camForward * movementDirection.y;
            // Prevenir diagonales mas rapidas
            moveDir = Vector3.ClampMagnitude(moveDir, 1f);

            // Construimos el target usando X y Z del input, y mantenemos la Y actual
            Vector3 targetVelocity = new Vector3(
                moveDir.x * maxSpeed,
                currentVelocity.y,
                moveDir.z * maxSpeed
            );

            // Si hay input usamos aceleración, si no desaceleración
            float currentAcceleration =
                movementDirection.sqrMagnitude > 0f ? acceleration : deceleration;

            MovementVelocity = Vector3.Lerp(
                currentVelocity,
                targetVelocity,
                currentAcceleration * Time.fixedDeltaTime
            );
        }

        // Movimiento horizontal con los valores desde la tierra
        public void HandleGroundHorizontalMovement(Vector2 movementDirection)
        {
            HandleHorizontalMovement(movementDirection, Stats.GroundMaxSpeed, Stats.GroundAcceleration, Stats.GroundDecceleration);
        }

        // Movimiento horizontal con los valores desde el aire
        public void HandleAirHorizontalMovement(Vector2 movementDirection)
        {
            HandleHorizontalMovement(movementDirection, Stats.AirMaxSpeed, Stats.AirAcceleration, Stats.AirDecceleration);
        }

        private void UpdateHorizontalMovementEvent(Vector3 velocityMovement)
        {
            float horizontalMovementVelocityNormalized = new Vector3(MovementVelocity.x, 0, MovementVelocity.z).magnitude / Stats.GroundMaxSpeed;
            PlayerEvents.OnHorizontalMovementVelocityChangedNormalized.Invoke(horizontalMovementVelocityNormalized);
        }

        public void UpdateGfxTargetDirection()
        {
            GfxTransform.SetTargetDirectionXZ(MovementVelocity);
        }

        #endregion

        #region Gravity & Airborne

        /// <summary>
        /// Aplica gravedad al personaje y limita la velocidad de caída.
        /// </summary>
        /// <param name="gravityMultiplier"> Escala la gravedad </param>
        public void ApplyGravity(float gravityMultiplier = 1f)
        {
            // Clampear velocidad maxima de caida
            SetMovementVelocityYAxis(Mathf.Max(MovementVelocity.y + Stats.Gravity * gravityMultiplier * Time.fixedDeltaTime, -Stats.MaxFallSpeed));
        }

        /// <summary>
        /// Gestiona la lógica de aterrizaje cuando el jugador toca el suelo.
        /// </summary>
        public bool TryLanding()
        {
            if (onGroundFlag)
            {
                JumpsUsed = 0;
                SetMovementVelocityYAxis(0);

                PlayerEvents.OnLand?.Invoke();

                if (InputComponent.MoveInputMagnitude > Stats.MovementDeadZone)
                {
                    StateMachine.ChangeState<RunPlayerState>();
                }
                else
                    StateMachine.ChangeState<IdlePlayerState>();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Gestiona la transicion al estado aereo cuando el jugador ya no este en el suelo.
        /// </summary>
        public bool TryAirborne()
        {
            // Cambiar a estado de aire
            if (!onGroundFlag)
            {
                StateMachine.ChangeState<AirPlayerState>();
                return true;
            }

            return false;
        }

        #endregion

        #region Jump

        /// <summary>
        /// Comprueba si es posible un salto y lo ejecuta en caso afirmativo.
        /// </summary>
        public bool TryJump()
        {
            if (JumpBuffer > 0)
            {
                // Salto desde el suelo (Coyote o grounded)
                if (CoyoteBuffer > 0 || onGroundFlag)
                {
                    ApplyJump();
                    StateMachine.ChangeState<AirPlayerState>();
                    return true;
                }

                // Salto desde el aire
                if (JumpsUsed < Stats.NumberOfJumpAllowed)
                {
                    // Gastar 2 saltos si cae en vez de saltar desde plataforma
                    int jumpsUsed = JumpsUsed == 0 ? 2 : 1;
                    ApplyJump(jumpsUsed);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Aplica la velocidad de salto al componente de movimiento y resetea los flags correspondientes
        /// </summary>
        /// <param name="jumpsUsed">Numero de saltos que se descuentan</param>
        private void ApplyJump(int jumpsUsed = 1)
        {
            JumpBuffer = 0;
            CoyoteBuffer = 0;
            JumpsUsed += jumpsUsed;

            // En el caso en el que se haya soltado el salto mientras todavia estaba activo el jump buffer, salto pequeño
            SetMovementVelocityYAxis(JumpReleasedDuringBuffer ?
                Stats.InitialJumpVelocity * Stats.SmallJumpMultiplier : Stats.InitialJumpVelocity);

            JumpReleasedDuringBuffer = false;

            PlayerEvents.OnJump?.Invoke();
        }

        #endregion
    }
}