using Game.Entities.Base;
using UnityEngine;

namespace Game.Entities.Player
{
    public class PlayerGfxTransform : EntityGfxTransform
    {
        public void Initialize(PlayerEvents playerEvents)
        {
            base.Initialize();

            playerEvents.OnJump.AddListener(DoStretch);
            playerEvents.OnLand.AddListener(DoSquash);
        }
    }
}
