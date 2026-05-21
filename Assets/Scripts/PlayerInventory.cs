using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Petiscos")]
    [SerializeField] private int quantidadePetiscos = 0;

    void Start()
    {
        HUD.Instance?.AtualizarTextoPetiscos(quantidadePetiscos);
    }

    public void AddTreat()
    {
        quantidadePetiscos++;
        Debug.Log($"[Inventário] Petisco coletado! Total: {quantidadePetiscos}");
        
        AudioManager.Instance?.TocarTreat();

        HUD.Instance?.AtualizarTextoPetiscos(quantidadePetiscos);
    }

    public bool UseTreat()
    {
        if (quantidadePetiscos > 0)
        {
            quantidadePetiscos--;
            Debug.Log($"[Inventário] Petisco usado! Restam: {quantidadePetiscos}");
            
            HUD.Instance?.AtualizarTextoPetiscos(quantidadePetiscos);
            return true; 
        }

        Debug.LogWarning("[Inventário] Você não tem petiscos! Explore o mapa para achar mais.");
        return false; 
    }

    public int GetTreatCount()
    {
        return quantidadePetiscos;
    }
}