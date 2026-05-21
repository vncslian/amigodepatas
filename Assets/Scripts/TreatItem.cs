using UnityEngine;

public class TreatItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = 
                other.GetComponent<PlayerInventory>();
            if (inv != null) inv.AddTreat();
            Destroy(gameObject);
        }
    }
}