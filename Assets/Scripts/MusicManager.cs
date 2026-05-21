using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Trilhas Sonoras")]
    public AudioClip musicaAmbiente;
    public AudioClip musicaCombate;

    [Header("Configurações")]
    public float fadeTime = 1.2f;
    [Range(0f, 1f)] public float volumeMaxAmbiente = 0.4f;
    [Range(0f, 1f)] public float volumeMaxCombate = 0.6f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private bool usandoSourceA = true;
    private bool emCombate = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.loop = true; sourceB.loop = true;
        sourceA.playOnAwake = false; sourceB.playOnAwake = false;
    }

    void Start()
    {
        if (musicaAmbiente != null)
        {
            sourceA.clip = musicaAmbiente;
            sourceA.volume = volumeMaxAmbiente;
            sourceA.Play();
        }
    }

    void Update()
    {
        Monster[] monstros = Object.FindObjectsByType<Monster>(FindObjectsSortMode.None);
        bool algumMonstroEmCombate = false;

        foreach (var m in monstros)
        {
            if (m != null && m.entity.inCombat && !m.entity.dead)
            {
                algumMonstroEmCombate = true;
                break;
            }
        }

        if (algumMonstroEmCombate && !emCombate)
        {
            emCombate = true;
            TrocarPara(musicaCombate, volumeMaxCombate);
        }
        else if (!algumMonstroEmCombate && emCombate)
        {
            emCombate = false;
            TrocarPara(musicaAmbiente, volumeMaxAmbiente);
        }
    }

    public void PararMusica()
    {
        sourceA.Stop();
        sourceB.Stop();
    }

    public void RetomarMusica()
    {
        if (usandoSourceA) sourceA.Play(); else sourceB.Play();
    }

    private void TrocarPara(AudioClip novoClip, float volumeAlvo)
    {
        AudioSource sourceAtual = usandoSourceA ? sourceA : sourceB;
        AudioSource proximoSource = usandoSourceA ? sourceB : sourceA;

        usandoSourceA = !usandoSourceA;

        proximoSource.clip = novoClip;
        proximoSource.Play();

        StartCoroutine(FadeRoutine(sourceAtual, proximoSource, volumeAlvo));
    }

    IEnumerator FadeRoutine(AudioSource antigo, AudioSource novo, float volMaxNovo)
    {
        float timer = 0f;
        float volInicialAntigo = antigo.volume;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;

            antigo.volume = Mathf.Lerp(volInicialAntigo, 0f, t);
            novo.volume = Mathf.Lerp(0f, volMaxNovo, t);
            yield return null;
        }

        antigo.volume = 0f;
        antigo.Stop();
        novo.volume = volMaxNovo;
    }
}