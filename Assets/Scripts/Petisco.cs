using UnityEngine;

public class Petisco : MonoBehaviour
{
    [SerializeField] private int quantidadeGanha = 1;
    [SerializeField] private GameObject efeitoVisualPrefab; 

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.TocarUI(); 

            Debug.Log($"Coletou {quantidadeGanha} petisco!");

            if (efeitoVisualPrefab != null)
            {
                Instantiate(efeitoVisualPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
