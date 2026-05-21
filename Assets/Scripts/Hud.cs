using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    [Header("Pontuacao")]
    public TextMeshProUGUI textoPontuacao;
    public TextMeshProUGUI textoMonstrosEliminados;
    public TextMeshProUGUI textoPetsAdotados;
    public TextMeshProUGUI textoPetsEmPerigo;
    
    [Header("Inventário")]
    public TextMeshProUGUI textoPetiscos; 
    [Header("Objetivo")]
    public TextMeshProUGUI textoObjetivo;
    public Image          barraObjetivo;

    [Header("Feedback")]
    public TextMeshProUGUI textoFeedback;
    public float tempFeedback = 2.5f;

    [Header("Meta")]
    public int totalPetsParaAdotar = 3;

    int   pontuacao          = 0;
    int   monstrosEliminados = 0;
    int   petsAdotados       = 0;
    int   petsCapturados     = 0;
    float feedbackTimer      = 0f;
    int   cachePetiscos      = 0; 

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        AtualizarUI();
        if (textoFeedback) textoFeedback.gameObject.SetActive(false);
    }

    void Update()
    {
        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f && textoFeedback)
                textoFeedback.gameObject.SetActive(false);
        }
    }

    public int GetPontuacao() => pontuacao;

    public void AdicionarPontos(int pts)    { pontuacao += pts; AtualizarUI(); }

    public void RegistrarMonstroEliminado()
    {
        monstrosEliminados++;
        AdicionarPontos(50);
        MostrarFeedback("Monstro eliminado! +50 pts", Color.red);
    }

    public void RegistrarPetAdotado()
    {
        petsAdotados++;
        AdicionarPontos(100);
        MostrarFeedback("Animal adotado! +100 pts", Color.green);
        AtualizarUI();
    }

    public void RegistrarPetCapturado()
    {
        petsCapturados++;
        MostrarFeedback("Pet capturado! Salve-o!", new Color(1f, 0.4f, 0f));
        AtualizarUI();
    }

    public void RegistrarPetLibertado()
    {
        petsCapturados = Mathf.Max(0, petsCapturados - 1);
        MostrarFeedback("Pet libertado!", Color.cyan);
        AtualizarUI();
    }

    public void AtualizarTextoPetiscos(int quantidade)
    {
        cachePetiscos = quantidade;
        if (textoPetiscos != null)
        {
            textoPetiscos.text = $"x{quantidade}";
        }
    }

    public void MostrarFeedback(string msg, Color cor)
    {
        if (!textoFeedback) return;
        textoFeedback.text  = msg;
        textoFeedback.color = cor;
        textoFeedback.gameObject.SetActive(true);
        feedbackTimer = tempFeedback;
    }

    void AtualizarUI()
    {
        if (textoPontuacao)
            textoPontuacao.text = $"Pontuação: {pontuacao}";

        if (textoMonstrosEliminados)
            textoMonstrosEliminados.text = $"Monstros eliminados: {monstrosEliminados}";

        if (textoPetsAdotados)
            textoPetsAdotados.text = $"Pets adotados: {petsAdotados}/{totalPetsParaAdotar}";

        if (textoPetsEmPerigo)
        {
            textoPetsEmPerigo.text  = petsCapturados > 0
                ? $"Pets em perigo: {petsCapturados} "
                : "Pets seguros ";
            textoPetsEmPerigo.color = petsCapturados > 0
                ? new Color(1f, 0.4f, 0f) : Color.green;
        }

        if (textoObjetivo)
            textoObjetivo.text = $"Adote {totalPetsParaAdotar - petsAdotados} animal(is) mais!";

        if (barraObjetivo)
            barraObjetivo.fillAmount = (float)petsAdotados / totalPetsParaAdotar;

        if (textoPetiscos)
            textoPetiscos.text = $"x{cachePetiscos}";
    }
}