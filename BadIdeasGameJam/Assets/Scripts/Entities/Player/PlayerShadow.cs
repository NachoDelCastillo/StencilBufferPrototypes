using DG.Tweening;
using Game.Entities.Player;
using Game.Entities.Player.States;
using System;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    #region References

    [Header("References")]
    [SerializeField] private Transform shadowObject;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float minScale = 0.3f;
    [SerializeField] private float maxScale = 1f;

    #endregion

    #region Variables

    private LayerMask groundMask;

    private float initialShadowAlpha;
    private float alphaMultiplier = 1;

    #endregion

    #region Initialize

    public void Initialize(LayerMask groundMask)
    {
        this.groundMask = groundMask;
        initialShadowAlpha = spriteRenderer.color.a;
    }

    #endregion

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDistance, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (angle > 30)
            {
                SetSpriteRendererAlpha(0.1f);
                return;
            }

            SetSpriteRendererAlpha(1);

            shadowObject.gameObject.SetActive(true);

            // Posicionar sombra
            shadowObject.position = hit.point + hit.normal * 0.02f;

            // Rotar según la superficie
            shadowObject.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Escalar según altura
            float distance = hit.distance;
            float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
            shadowObject.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            shadowObject.gameObject.SetActive(false);
            SetSpriteRendererAlpha(0);
        }
    }

    private void SetSpriteRendererAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha * alphaMultiplier * initialShadowAlpha);
    }
}
