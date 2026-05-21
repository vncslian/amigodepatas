using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake ao tomar dano")]
    public float duracaoPadrao = 0.25f;
    public float intensidadePadrao = 0.15f;

    private bool shaking = false;
    private CameraFollow cameraFollow;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
        cameraFollow = GetComponent<CameraFollow>();
    }

    public void Shake() => Shake(duracaoPadrao, intensidadePadrao);

    public void Shake(float duracao, float intensidade)
    {
        if (shaking) StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duracao, intensidade));
    }

    IEnumerator ShakeRoutine(float duracao, float intensidade)
    {
        shaking = true;
        float timer = 0f;

        while (timer < duracao)
        {
            timer += Time.deltaTime;

            float forca = intensidade * (1f - timer / duracao);

            float x = Random.Range(-1f, 1f) * forca;
            float y = Random.Range(-1f, 1f) * forca;

            if (cameraFollow != null)
            {
                cameraFollow.transform.position += new Vector3(x, y, 0f);
            }

            yield return null;
        }

        shaking = false;
    }
}