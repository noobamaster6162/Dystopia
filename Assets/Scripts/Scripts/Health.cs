
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] private float maxHealth;
    private float _currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {

        _currentHealth -= damage;

        if (_currentHealth < 0f)
        {
            Die();
        }

    }

    public void Die()
    {
        Destroy(gameObject);
    }

}
