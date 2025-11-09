using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : MonoBehaviour
{

    public Transform planet;

    public Transform followArrow;

    public Transform dude1;
    public Transform dude2;
    public Transform dude3;
    public Transform dude4;
    public Transform dude5;

    public Transform dude1Title;
    public Transform dude2Title;
    public Transform dude3Title;
    public Transform dude4Title;
    public Transform dude5Title;

    private Color dude1ColorVelocity;

    private Vector3 velocityPos;
    
    private float fromY;
    private float velocityY;
    private Vector3 fromVec3;
    private Vector3 velocityVec3;
    private Color fromColor;
    private Color velocityColor;

    private void Start()
    {
        // Animación de la flecha
        LeanTween.delayedCall(followArrow.gameObject, 3f, MoveArrow)
            .setOnStart(MoveArrow)
            .setRepeat(-1);

        // === SECCIÓN 1: SEGUIR POSICIÓN ===
        FollowY(dude1, LeanTweenType.easeOutSine, 1.1f);
        FollowY(dude2, LeanTweenType.easeSpring, 1.1f);
        FollowY(dude3, LeanTweenType.easeOutBounce, 1.1f);
        FollowY(dude4, LeanTweenType.easeSpring, 1.1f);
        FollowY(dude5, LeanTweenType.linear, 0.5f);

        // === SECCIÓN 2: SEGUIR COLOR ===
        FollowColor(dude1, LeanTweenType.easeOutSine, 1.1f);
        FollowColor(dude2, LeanTweenType.easeSpring, 1.1f);
        FollowColor(dude3, LeanTweenType.easeOutBounce, 1.1f);
        FollowColor(dude4, LeanTweenType.easeSpring, 1.1f);
        FollowColor(dude5, LeanTweenType.linear, 0.5f);

        // === SECCIÓN 3: SEGUIR ESCALA ===
        FollowScale(dude1, LeanTweenType.easeOutSine, 1.1f);
        FollowScale(dude2, LeanTweenType.easeSpring, 1.1f);
        FollowScale(dude3, LeanTweenType.easeOutBounce, 1.1f);
        FollowScale(dude4, LeanTweenType.easeSpring, 1.1f);
        FollowScale(dude5, LeanTweenType.linear, 1.5f);

        // === SECCIÓN 4: TITULOS SIGUIENDO A DUDES ===
        var titleOffset = new Vector3(0f, -20f, -18f);
        FollowTitle(dude1Title, dude1, titleOffset, LeanTweenType.easeOutSine);
        FollowTitle(dude2Title, dude2, titleOffset, LeanTweenType.easeSpring);
        FollowTitle(dude3Title, dude3, titleOffset, LeanTweenType.easeOutBounce);
        FollowTitle(dude4Title, dude4, titleOffset, LeanTweenType.easeSpring);
        FollowTitle(dude5Title, dude5, titleOffset, LeanTweenType.linear);

        // === ROTAR PLANETA ===
        if (Camera.main)
        {
            var localPos = Camera.main.transform.InverseTransformPoint(planet.position);
            LeanTween.rotateAround(Camera.main.gameObject, Vector3.left, 360f, 300f)
                .setPoint(localPos)
                .setRepeat(-1);
        }
    }

    private void Update()
    {
        // Ejemplo de uso de LeanSmooth
        fromY = LeanSmooth.spring(fromY, followArrow.localPosition.y, ref velocityY, 1.1f);
        fromVec3 = LeanSmooth.spring(fromVec3, dude5Title.localPosition, ref velocityVec3, 1.1f);
        fromColor = LeanSmooth.spring(fromColor, dude1.GetComponent<Renderer>().material.color, ref velocityColor, 1.1f);
    }

    private void MoveArrow()
    {
        LeanTween.moveLocalY(followArrow.gameObject, Random.Range(-100f, 100f), 0f);
        var randomCol = new Color(Random.value, Random.value, Random.value);
        LeanTween.color(followArrow.gameObject, randomCol, 0f);
        var randomVal = Random.Range(5f, 10f);
        followArrow.localScale = Vector3.one * randomVal;
    }

    // ---------- HELPERS MODERNOS ----------
    private void FollowY(Transform target, LeanTweenType ease, float time)
    {
        LeanTween.value(target.gameObject,
            target.localPosition.y,
            followArrow.localPosition.y,
            time)
            .setEase(ease)
            .setOnUpdate((float val) =>
            {
                var pos = target.localPosition;
                pos.y = val;
                target.localPosition = pos;
            })
            .setRepeat(-1);
    }

    private void FollowColor(Transform target, LeanTweenType ease, float time)
    {
        var rend = target.GetComponent<Renderer>();
        if (rend == null) return;

        LeanTween.value(target.gameObject,
            rend.material.color,
            followArrow.GetComponent<Renderer>().material.color,
            time)
            .setEase(ease)
            .setOnUpdate((Color col) =>
            {
                rend.material.color = col;
            })
            .setRepeat(-1);
    }

    private void FollowScale(Transform target, LeanTweenType ease, float time)
    {
        LeanTween.value(target.gameObject,
            target.localScale,
            followArrow.localScale,
            time)
            .setEase(ease)
            .setOnUpdate((Vector3 val) =>
            {
                target.localScale = val;
            })
            .setRepeat(-1);
    }

    private void FollowTitle(Transform title, Transform target, Vector3 offset, LeanTweenType ease)
    {
        LeanTween.value(title.gameObject,
            title.localPosition,
            target.localPosition + offset,
            0.6f)
            .setEase(ease)
            .setOnUpdate((Vector3 val) =>
            {
                title.localPosition = val;
            })
            .setRepeat(-1);
    }
}
