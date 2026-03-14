using Core.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    [RequireComponent(typeof(Animator))]
    public class FootstepsSfx : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<FootstepId, List<AudioClip>> footstepClips;

        private List<AudioClip> clipsInUse;

        public void OnFootstepTriggered(FootstepId footstepId)
        {
            clipsInUse = footstepClips[footstepId];
            AudioManager.Instance.PlaySFX(clipsInUse[UnityEngine.Random.Range(0, clipsInUse.Count)], .5f, true);
        }
    }
}
