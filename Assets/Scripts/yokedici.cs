using UnityEngine;

public class yokedici : MonoBehaviour
{
    public string targetTag = "Destructible"; 

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Durum: Mermi parçalanabilir bir objeye çarparsa
        if (other.CompareTag(targetTag))
        {
            Destroy(other.gameObject); 
            Destroy(gameObject); 
        }
        // 2. Durum: Mermi bir duvara çarparsa
        else if (other.CompareTag("Wall")) 
        {
            Destroy(gameObject);
        }
    }
}