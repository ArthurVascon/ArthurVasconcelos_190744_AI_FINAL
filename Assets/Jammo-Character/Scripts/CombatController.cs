using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    Animator animator;
    public GameObject projetil;
    public Transform projetilSpawner;
    public float projetilVelocity = 1000f;

    public float attackTimer = 0f;
    float attackCD = 3f;

    bool attacking = false;

    private void Start()
    {

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !attacking)
        {
            attacking = true;
            if(attackTimer <= 0)
            { 
                Invoke("DeployAttack", 0.7f);
            }
            attackTimer = attackCD;
            
        }
        if(attacking == true)
        {
            if(attackTimer >= 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                attacking = false;
                animator.SetBool("attack", attacking);
            }
        } 
        animator.SetBool("attack", attacking);
    }

    void DeployAttack()
    {
            GameObject ball = Instantiate(projetil, projetilSpawner.position, Quaternion.LookRotation(transform.forward));
            ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, projetilVelocity));
            Destroy(ball, 4f);
    }
}
