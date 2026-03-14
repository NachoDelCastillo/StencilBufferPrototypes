using Core.CustomInput;
using Game.Entities.Base;
using Game.Entities.Player.Animation;
using UnityEngine;

namespace Game.Entities.Player
{
    /// <summary>
    /// Inicializador del jugador.
    /// Se encarga de cachear componentes y de inicializar los componentes del jugador.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerAnimationSynchronizer))]
    [RequireComponent(typeof(PlayerSfx))]
    [RequireComponent(typeof(PlayerParticleManager))]
    public class PlayerInitializer : EntityInitializer<PlayerController, PlayerContext>
    {
        #region References

        private PlayerAnimationSynchronizer animationSync;
        private PlayerGfxTransform gfxTransform;
        private PlayerSfx playerSfx;
        private PlayerParticleManager playerParticleManager;
        private PlayerShadow playerShadow;

        #endregion

        #region Initialization

        // Para no complicar las cosas inecesariamente, el jugador se inicia a si mismo
        // Pero lo suyo seria inicializar al jugador desde un Initialize del GameplayManager
        private void Awake() => InitializePlayer(PlayerInputHandler.Instance);
        private void Start() => LateInitialize();

        public void InitializePlayer(PlayerInputHandler playerInputHandler)
        {
            base.Initialize(); // Cachea controller y movement component

            // Cachea componentes específicos del Player
            animationSync = GetComponent<PlayerAnimationSynchronizer>();
            playerSfx = GetComponent<PlayerSfx>();
            playerParticleManager = GetComponent<PlayerParticleManager>();

            gfxTransform = GetComponentInChildren<PlayerGfxTransform>();
            playerShadow = GetComponentInChildren<PlayerShadow>();

            // Inicializa PlayerController con todos los componentes necesarios
            entityController.Initialize(gfxTransform, moveComponent);

            // Inicializa la sincronización de animaciones
            animationSync.Initialize(entityController);

            playerSfx.Initialize(entityController);

            playerParticleManager.Initialize(entityController);

            gfxTransform.Initialize(entityController.PlayerEvents);

            playerShadow.Initialize(GetEntityMovementComponent().GroundLayer);
        }

        // Cualquier componente que necesite PlayerEvents, debera inicializarse en el LateInitialize
        public void LateInitialize()
        {
            entityController.LateInitialize();
        }

        #endregion
    }
}