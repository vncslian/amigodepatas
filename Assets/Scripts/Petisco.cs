using UnityEngine;

public class Petisco : MonoBehaviour
{
    [SerializeField] private int quantidadeGanha = 1;
    [SerializeField] private GameObject efeitoVisualPrefab; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem passou por cima foi o Player
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