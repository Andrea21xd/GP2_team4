using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Oyuncu Can Bilgileri")]
    public int maxPlayerHealth = 10;
    public int currentPlayerHealth = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject); // Zaten var ise, kendini yok et
        }
    }

    // Oyuncunun canını sıfırla (örn. sahneye girerken)
    public void ResetPlayerHealth()
    {
        currentPlayerHealth = maxPlayerHealth;
    }

    // Oyuncu canını azaltmak için (enemy vurduğunda vs.)
    public void TakePlayerDamage(int damage)
    {
        currentPlayerHealth -= damage;
        if (currentPlayerHealth < 0) currentPlayerHealth = 0;
        Debug.Log("Oyuncu hasar aldı. Kalan can: " + currentPlayerHealth);
    }

    // Oyuncu öldü mü?
    public bool IsPlayerDead()
    {
        return currentPlayerHealth <= 0;
    }
}
