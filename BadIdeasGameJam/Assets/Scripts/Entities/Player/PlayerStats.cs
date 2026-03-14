using UnityEngine;

namespace Game.Entities.Player
{
    /// <summary>
    /// Contiene todas las estadísticas de movimiento del jugador.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        #region Jump

        [Header("Jump")]
        [Space(5)]
        [Tooltip("Altura máxima que alcanza el salto del jugador, la gravedad se calcula en torno a esta variable")]
        [SerializeField] private float jumpHeight = 6.5f;

        [Tooltip("Tiempo que tarda el jugador en alcanzar la altura máxima del salto.")]
        [SerializeField] private float timeTillJumpApex = .35f;

        [Tooltip("Multiplicador para el salto más pequeño posible (salto corto).")]
        [SerializeField] [Range(0.1f, 1f)] private float smallestJumpPossibleMultiplier = .7f;

        [Tooltip("Duración del buffer de salto (permite presionar el salto ligeramente antes de tocar el suelo).")]
        [SerializeField] private float jumpBufferTime = 0.125f;

        [Tooltip("Duración del Coyote Time (permite saltar poco después de perder contacto con el suelo).")]
        [SerializeField] private float coyoteBufferTime = 0.125f;

        [Tooltip("Número máximo de saltos permitidos antes de tocar el suelo.")]
        [SerializeField] [Range(1, 3f)] private int allowedJumps = 1;

        #endregion

        #region Input

        [Header("Input")]
        [Space(5)]

        [Tooltip("Zona muerta para input horizontal.")]
        [SerializeField] [Range(0, 1f)] private float movementDeadZone = 0.2f;

        #endregion

        #region Horizontal Movement

        [Header("Horizontal Movement")]
        [Space(5)]

        [Tooltip("Velocidad maxima horizontal del jugador mientras está en el suelo.")]
        [SerializeField] [Range(0, 50f)] private float groundMaxSpeed = 30f;

        [Tooltip("Velocidad maxima horizontal del jugador mientras está en el aire.")]
        [SerializeField] [Range(0, 50f)] private float airMaxSpeed = 30f;

        [Tooltip("Aceleracion horizontal del jugador mientras está en el suelo.")]
        [SerializeField] [Range(0, 50f)] private float groundAcceleration = 30f;

        [Tooltip("Aceleracion horizontal del jugador mientras está en el aire.")]
        [SerializeField] [Range(0, 50f)] private float airAcceleration = 30f;

        [Tooltip("Desaceleración horizontal del jugador mientras está en el suelo.")]
        [SerializeField] [Range(0, 50f)] private float groundDecceleration = 20f;

        [Tooltip("Desaceleración horizontal del jugador mientras está en el aire.")]
        [SerializeField] [Range(0, 50f)] private float airDecceleration = 10f;

        #endregion

        #region Vertical Movement

        [Header("Vertical Movement")]
        [Space(5)]

        [Tooltip("Multiplicador de gravedad aplicado al caer.")]
        [SerializeField] private float gravityOnFallMultiplier = 2f;

        [Tooltip("Velocidad máxima de caída del jugador.")]
        [SerializeField] [Range(0, 50f)] private float maxFallSpeed = 20f;

        #endregion

        #region Read Only Properties

        // Jump
        public float JumpHeight => jumpHeight;
        public float TimeTillJumpApex => timeTillJumpApex;
        public float SmallJumpMultiplier => smallestJumpPossibleMultiplier;
        public int NumberOfJumpAllowed => allowedJumps;
        public float JumpBufferTime => jumpBufferTime;
        public float CoyoteBufferTime => coyoteBufferTime;

        // Deadzone
        public float MovementDeadZone => movementDeadZone;

        // Horizontal movement
        public float GroundMaxSpeed => groundMaxSpeed;
        public float AirMaxSpeed => airMaxSpeed;
        public float GroundAcceleration => groundAcceleration;
        public float AirAcceleration => airAcceleration;
        public float GroundDecceleration => groundDecceleration;
        public float AirDecceleration => airDecceleration;

        // Vertical movement
        public float GravityOnFallMultiplier => gravityOnFallMultiplier;
        public float MaxFallSpeed => maxFallSpeed;

        #endregion

        #region GravityCalculations

        public float Gravity { get; private set; }
        public float InitialJumpVelocity { get; private set; }

        private void OnValidate()
        {
            UpdateGravityValues();
        }

        private void OnEnable()
        {
            UpdateGravityValues();
        }

        /// <summary>
        /// Calculo de la gravedad y la velocidad de salto en base a la altura de salto y el tiempo para llegar a dicha altura
        /// </summary>
        private void UpdateGravityValues()
        {
            Gravity = -(2f * jumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
            InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
        }

        #endregion
    }
}