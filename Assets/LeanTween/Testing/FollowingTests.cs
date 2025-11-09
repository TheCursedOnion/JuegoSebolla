using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingTests : MonoBehaviour
{

    public Transform followTrans;

    public Transform cube1;
    private float cube1VelocityX;

    public Transform cube2;
    private float cube2VelocityX;

    public Transform cube3;
    private float cube3VelocityX;

    public Transform cube4;
    private float cube4VelocityX;

    public Transform cube5;
    private float cube5VelocityX;

    public Transform cube6;
    private Vector3 cube6Velocity;

    public Transform fly1;

    private void Start()
    {
        // Movimiento aleatorio del objetivo
        LeanTween.delayedCall(followTrans.gameObject, 3f, MoveFollow)
            .setOnStart(MoveFollow)
            .setRepeat(-1);

        // Simular followDamp con LeanTween.value
        LeanTween.value(cube6.gameObject, cube6.position, followTrans.position, 0.6f)
            .setEase(LeanTweenType.easeOutSine)
            .setOnUpdate((Vector3 val) => cube6.position = val)
            .setRepeat(-1);
    }

    private void MoveFollow()
    {
        Vector3 newPos = new Vector3(
            Random.Range(-50f, 50f),
            Random.Range(-10f, 10f),
            0f
        );
        LeanTween.move(followTrans.gameObject, newPos, 0f);
    }

    private void Update()
    {
        Vector3 pos;

        // === Diferentes tipos de seguimiento ===

        // Damp: amortiguado clásico (como Mathf.SmoothDamp)
        pos = cube1.position;
        pos.x = LeanSmooth.damp(cube1.position.x, followTrans.position.x, ref cube1VelocityX, 1.1f);
        cube1.position = pos;

        // Spring: efecto elástico
        pos = cube2.position;
        pos.x = LeanSmooth.spring(cube2.position.x, followTrans.position.x, ref cube2VelocityX, 1.1f);
        cube2.position = pos;

        // BounceOut: rebote al final
        pos = cube3.position;
        pos.x = LeanSmooth.bounceOut(cube3.position.x, followTrans.position.x, ref cube3VelocityX, 1.1f);
        cube3.position = pos;

        // Quintic suavizado manual (imitando el smoothQuint antiguo)
        pos = cube4.position;
        pos.x = Mathf.Lerp(cube4.position.x, followTrans.position.x, Time.deltaTime * 2f);
        cube4.position = pos;

        // Linear simple
        pos = cube5.position;
        pos.x = LeanSmooth.linear(cube5.position.x, followTrans.position.x, 10f);
        cube5.position = pos;

        // "Smooth gravity" simulado (suavizado tipo gravedad)
        cube6.position = Vector3.SmoothDamp(
            cube6.position,
            followTrans.position,
            ref cube6Velocity,
            1.1f
        );

        // Debug para ver si hay tweens activos
        if (LeanTween.isTweening(followTrans.gameObject))
        {
            Debug.Log("Tweening...");
        }
    }
}
