using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    public GameObject catPrefab;
    public Transform[] spawnPoints;
    public int maxCats = 4;

    void Start()
    {
        for (int i = 0; i < maxCats; i++)
        {
            Transform pt = spawnPoints[
                Random.Range(0, spawnPoints.Length)];
            Instantiate(catPrefab, pt.position, 
                        Quaternion.identity);
        }
    }

    public void SpawnCat(Vector2 position)
    {
        Instantiate(catPrefab, position, 
                    Quaternion.identity);
    }
}