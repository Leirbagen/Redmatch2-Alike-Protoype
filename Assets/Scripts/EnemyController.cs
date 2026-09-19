using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float velocity = 10;
    [SerializeField] private float currentHealth;
    public GameObject damageEffect;
    private Renderer enemyRenderer;
    private Color originalColor;
    [SerializeField] private Directions currentDirection;
    [SerializeField] private Rigidbody enemyBody;
    private float coolDownWalls = 0f; 


    private void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        enemyBody = GetComponent<Rigidbody>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

    }
    public void TakeDamage(int damageInt) 
    {
        currentHealth -= damageInt;
        GameObject damageEffectClone = Instantiate(damageEffect, transform.position, transform.rotation);
        StartCoroutine(colorDamage());
        Destroy(damageEffectClone, 1f);
        if (currentHealth <= 0) 
        {
            Die();
        }
    }
    public IEnumerator colorDamage() 
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = originalColor;
    }
    private void Die() 
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        if (coolDownWalls > 0f)
        {
            coolDownWalls -= Time.deltaTime;
        }
        Vector3 movDirection = Vector3.zero;
        switch (currentDirection) 
        {
            case Directions.F:
                movDirection = Vector3.forward;
                break;
            case Directions.B:
                movDirection = Vector3.back;
                break;
            case Directions.R:
                movDirection = Vector3.right;
                break;
            case Directions.L:
                movDirection = Vector3.left;
                break;
        }
        Vector3 finalVelocity = movDirection * velocity;
        finalVelocity.y = enemyBody.linearVelocity.y; 
        enemyBody.linearVelocity = finalVelocity;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Floor") && coolDownWalls <= 0)
        {
            RandomDirection();
            coolDownWalls = 1.2f;
        }
    }
    private void RandomDirection() 
    {
        Directions oldDirection = currentDirection;
        do
        {
            int randomDirection = Random.Range(1, 5);
            switch (randomDirection)
            {
                case 1: 
                    currentDirection = Directions.F; 
                    break;
                case 2:
                    currentDirection = Directions.B; 
                    break;
                case 3:
                    currentDirection = Directions.R;
                    break;
                case 4: 
                    currentDirection = Directions.L; 
                    break;
            }
        }
        while (currentDirection == oldDirection); 
    }
}
enum Directions
{
    F,B,R,L
}
