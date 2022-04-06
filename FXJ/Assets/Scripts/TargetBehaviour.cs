using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    [SerializeField] Vector2 clamps = new Vector2(-9.0f, 9.0f);
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;
    [SerializeField] bool bMoveWhenDead = true;

    private int health = 0;
    private int movementDir = 1;

    private void Update()
    {
        if (!TargetManager.instance.bIsGameActive)
            return;

        if (!IsAlive() && !bMoveWhenDead)
            return;

        float moveDelta = movementDir * moveSpeed * Time.deltaTime;

        if (movementDir == -1 && transform.position.z - moveDelta <= clamps.x || movementDir == 1 && transform.position.z + moveDelta >= clamps.y)
        {
            movementDir *= -1;
            moveDelta *= -1;
        }

        transform.position += new Vector3(0, 0, moveDelta);
    }

    public void OnHit(Vector3 hitPos)
    {
        if (!IsAlive())
            return;

        bool bHeadShot = false;
        if (hitPos.y > transform.position.y + 0.37f)
            bHeadShot = true;

        if (bHeadShot)
            health -= 3;
        else
            health -= 1;

        if (health <= 0)
            Kill();
    }

    public void Kill()
    {
        animator.SetBool("isAlive", false);
        TargetManager.instance.OnTargetKilled();
    }

    public void Deploy()
    {
        animator.SetBool("isAlive", true);
        health = maxHealth;
    }

    private bool IsAlive() { return health > 0; }

}
