using UnityEngine;

public class SpawnManagerPetiscos : MonoBehaviour
{
    [Header("Configurações do Prefab")]
    public GameObject petiscoPrefab; 

    [Header("Área de Spawn (Limites do Mapa)")]
    public float xMinimo;
    public float xMaximo;
    public float yMinimo;
    public float yMaximo;

    [Header("Quantidade")]
    public int quantidadeInicial = 10;
    public int limiteMaximoNoMapa = 20;
    public float tempoEntreSpawns = 5f;

    private int petiscosAtuaisNoMapa = 0;

    void Start()
    {
        for (int i = 0; i < quantidadeInicial; i++)
        {
            SpawnarPetiscoAleatorio();
        }

        InvokeRepeating(nameof(TentarSpawnarPetisco), tempoEntreSpawns, tempoEntreSpawns);
    }

    void TentarSpawnarPetisco()
    {
        if (petiscosAtuaisNoMapa < limiteMaximoNoMapa)
        {
            SpawnarPetiscoAleatorio();
        }
    }

    void SpawnarPetiscoAleatorio()
    {
        float xAleatorio = Random.Range(xMinimo, xMaximo);
        float yAleatorio = Random.Range(yMinimo, yMaximo);
        Vector3 posicaoSpawn = new Vector3(xAleatorio, yAleatorio, 0f);

        GameObject novoPetisco = Instantiate(petiscoPrefab, posicaoSpawn, Quaternion.identity);
        
        petiscosAtuaisNoMapa++;
        
        DestroyDetector detector = novoPetisco.AddComponent<DestroyDetector>();
        detector.onDestroyed += () => petiscosAtuaisNoMapa--;
    }
}

public class DestroyDetector : MonoBehaviour
{
    public System.Action onDestroyed;
    private void OnDestroy() => onDestroyed?.Invoke();
}