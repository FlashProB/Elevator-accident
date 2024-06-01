using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private Health playerHealth;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var playerHealth = other.GetComponent<Health>();
        if (other.tag == "Player")
        {
            playerHealth.TakeDamage(1);
            Invoke("DestroyGameObject", 3);
            GetComponent<ParticleSystem>().Play();
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
    }
    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
