using Core.Audio;
using Core.FSM;
using Game.Entities.Player;
using Game.Entities.Player.States;
using System;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    public void Initialize(PlayerController playerController)
    {
        StateMachine stateMachine = playerController.GetStateMachine();

        playerController.PlayerEvents.OnJump.AddListener(() => AudioManager.Instance.PlaySFX(SFX_Id.Jump, true));
    }
}
