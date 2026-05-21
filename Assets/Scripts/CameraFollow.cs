using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo para Seguir")]
    public Transform alvo; 

    [Header("Configurações")]
    public float suavidade = 5f;
    private float zOriginal;

    void Start()
    {
        zOriginal = transform.position.z;
        
        if (alvo == null) TentarAcharPlayer();
    }

    void LateUpdate()
    {
        if (alvo == null)
        {

            TentarAcharPlayer();
            return; 
        }

        Vector3 posicaoDesejada = new Vector3(alvo.position.x, alvo.position.y, zOriginal);
        
        transform.position = Vector3.Lerp(transform.position, posicaoDesejada, suavidade * Time.deltaTime);
    }

    private void TentarAcharPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            alvo = playerObj.transform;
        }
    }
}