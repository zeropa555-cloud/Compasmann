using UnityEngine;

public class yokedicikod : MonoBehaviour
{
    // Hedef etiketi Inspector'dan değiştirebilirsin
    public string targetTag = "Destructible"; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Destroy(other.gameObject); 
            Destroy(gameObject); 
        }
        else if (other.CompareTag("Wall")) 
        {
            Destroy(gameObject);
        }
    }
}