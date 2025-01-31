using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Düşman öldü!");
            // CombatManager endCombat fonksiyonu handle ettiği için 
            // burada Destroy(gameObject) yapabilirsin veya 
            // CombatManager bittiği an sahne değişiyor. 
            // Basitçe yok edebilirsin:
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Düşmanın kalan canı: " + currentHealth);
        }
    }
}
