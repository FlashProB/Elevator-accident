using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Animator anim;
    public int maxHealth = 100;
    int currentHealth;
    private bool isDed;
    void Start()
    {
        currentHealth = maxHealth;
        anim.SetBool("Ded", false);
    }

    public void TakeDamage(int damage)
    {
        if (isDed == false)
        {
            currentHealth -= damage;
            anim.SetTrigger("Hurt");

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        if (isDed == false)
        {
            isDed = true;
            Debug.Log("Ded");
            anim.SetBool("Ded", true);
            this.enabled = false;
        }
    }
}
