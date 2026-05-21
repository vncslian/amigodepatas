using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TelaEducativa : MonoBehaviour
{
    public static TelaEducativa Instance { get; private set; }

    [Header("UI")]
    public GameObject      painel;
    public TextMeshProUGUI textoCabecalho;
    public TextMeshProUGUI textoConteudo;
    public TextMeshProUGUI textoDica;
    public Button          botaoFechar;
    public Image           iconeIlustracao;

    [Header("Cards de Adoção")]
    public List<InfoCard> cardsAdocao  = new List<InfoCard>();

    [Header("Cards de Monstro")]
    public List<InfoCard> cardsMonstro = new List<InfoCard>();

    private bool jaMostrouGato     = false;
    private bool jaMostrouCachorro = false;
    private bool jaMostrouAbandono = false;
    private bool jaMostrouFome     = false;

    [System.Serializable]
    public class InfoCard
    {
        public string cabecalho;
        [TextArea(3, 6)] public string conteudo;
        [TextArea(1, 3)] public string dica;
        public Sprite ilustracao;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (painel) painel.SetActive(false);
        botaoFechar?.onClick.AddListener(Fechar);
        PopularCardsDefault();
    }

    void PopularCardsDefault()
    {
        if (cardsAdocao.Count == 0)
        {
            cardsAdocao.Add(new InfoCard
            {
                cabecalho = "Você adotou um gato!",
                conteudo  = "Os gatos são os animais mais abandonados no Brasil. " +
                            "Cerca de 10 milhões vivem nas ruas sem cuidado. " +
                            "Adotar um gatinho muda a vida dele para sempre!",
                dica      = "Dica: leve ao veterinário para vacinar e castrar logo após a adoção."
            });
            cardsAdocao.Add(new InfoCard
            {
                cabecalho = "Você adotou um cachorro!",
                conteudo  = "No Brasil existem cerca de 20 milhões de cães em situação de rua. " +
                            "Cães abandonados sofrem fome, frio e doenças todos os dias. " +
                            "A sua adoção faz toda a diferença!",
                dica      = "Dica: cães precisam de exercício diário e muito carinho."
            });
        }

        if (cardsMonstro.Count == 0)
        {
            cardsMonstro.Add(new InfoCard
            {
                cabecalho = "Monstro do Abandono derrotado!",
                conteudo  = "Abandonar animais é crime no Brasil! " +
                            "A Lei Federal 9.605/98 prevê prisão de 3 meses a 1 ano " +
                            "e multa para quem maltratar ou abandonar animais. " +
                            "Denuncie maus-tratos ao CRMV ou à delegacia.",
                dica      = "Dica: se ver um animal sofrendo, ligue 153 ou 190."
            });
            cardsMonstro.Add(new InfoCard
            {
                cabecalho = "Monstro da Fome derrotado!",
                conteudo  = "A fome é um dos maiores sofrimentos dos animais abandonados. " +
                            "Animais de rua passam dias sem comer e ficam doentes. " +
                            "Você pode ajudar doando ração para ONGs da sua cidade!",
                dica      = "Dica: pesquise protetores independentes no Instagram da sua cidade."
            });
        }
    }

    public void MostrarAdocaoGato()
    {
        if (jaMostrouGato) return;
        jaMostrouGato = true;
        if (cardsAdocao.Count > 0) Exibir(cardsAdocao[0]);
    }

    public void MostrarAdocaoCachorro()
    {
        if (jaMostrouCachorro) return;
        jaMostrouCachorro = true;
        if (cardsAdocao.Count > 1) Exibir(cardsAdocao[1]);
    }

    public void MostrarMonstroAbandono()
    {
        if (jaMostrouAbandono) return;
        jaMostrouAbandono = true;
        if (cardsMonstro.Count > 0) Exibir(cardsMonstro[0]);
    }

    public void MostrarMonstroFome()
    {
        if (jaMostrouFome) return;
        jaMostrouFome = true;
        if (cardsMonstro.Count > 1) Exibir(cardsMonstro[1]);
    }

    public void MostrarAdocao()  { }
    public void MostrarMonstro() { }

    void Exibir(InfoCard card)
    {
        if (painel)         painel.SetActive(true);
        if (textoCabecalho) textoCabecalho.text = card.cabecalho;
        if (textoConteudo)  textoConteudo.text  = card.conteudo;
        if (textoDica)      textoDica.text       = "💡 " + card.dica;
        if (iconeIlustracao != null && card.ilustracao != null)
            iconeIlustracao.sprite = card.ilustracao;

        Time.timeScale = 0f;
        AudioManager.Instance?.TocarUI();
    }

    public void Fechar()
    {
        if (painel) painel.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.Instance?.TocarUI();
    }
}