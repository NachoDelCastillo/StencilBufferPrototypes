using System;
using UnityEngine;
using UnityEngine.Events;

public enum FootstepId
{
    wood,
    grass,
    concrete
}

public class FootstepNotifier : MonoBehaviour
{
    [SerializeField] private FootstepId footstepId;
    [SerializeField] private UnityEvent<FootstepId> onFootstepPerformedEvent;

    private Animator animator;
    private int HorizontalMovementVelocityHash;

    private void Awake() => Initialize();
    private void Initialize()
    {
        animator = GetComponent<Animator>();
        HorizontalMovementVelocityHash = Animator.StringToHash("HorizontalMovementVelocity");
    }

    public void TriggerFootstep()
    {
        if (animator.GetFloat(HorizontalMovementVelocityHash) > 0.025f)
            onFootstepPerformedEvent.Invoke(footstepId);
    }
}
