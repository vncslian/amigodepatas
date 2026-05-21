using System.Collections;
using UnityEngine;

public class MonsterAlert : MonoBehaviour
{
    [Header("Ícone de Alerta")]
    public GameObject alertIcon;

    [Header("Configurações de Animação")]
    public float animScale = 1.3f;
    public float animTime = 0.15f;

    private Vector3 posicaoOriginalNaCena;
    private Vector3 escalaOriginal;
    private bool estaAnimando = false;

    void Awake()
    {
        if (alertIcon != null)
        {

            posicaoOriginalNaCena = alertIcon.transform.localPosition;
            escalaOriginal = alertIcon.transform.localScale;
            
            // Começa escondido
            alertIcon.SetActive(false);
        }
    }


    public void AtivarAlerta()
    {
        if (alertIcon == null || estaAnimando) return;

        alertIcon.SetActive(true);

        alertIcon.transform.localPosition = posicaoOriginalNaCena;

        StartCoroutine(AnimarAlertaRoutine());
    }

    IEnumerator AnimarAlertaRoutine()
    {
        estaAnimando = true;
        float timer = 0f;

        while (timer < animTime)
        {
            timer += Time.deltaTime;
            float progresso = timer / animTime;
            
            alertIcon.transform.localScale = escalaOriginal * Mathf.Lerp(0f, animScale, progresso);
            yield return null;
        }

        alertIcon.transform.localScale = escalaOriginal * animScale;
        
        estaAnimando = false;
    }

    public void DesativarAlerta()
    {
        if (alertIcon != null)
        {
            alertIcon.SetActive(false);
        }
    }
}