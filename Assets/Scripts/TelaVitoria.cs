using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TelaVitoria : MonoBehaviour
{
    public static TelaVitoria Instance { get; private set; }

    [Header("UI")]
    public GameObject       painelVitoria;
    public TextMeshProUGUI  textoPrincipal;   
    public TextMeshProUGUI  textoSub;         
    public TextMeshProUGUI  textoPontuacao;   
    public TextMeshProUGUI  textoMensagem;    
    public Button           botaoMenuPrincipal;
    public Button           botaoJogarNovamente;

    [Header("Animação")]
    public float delayParaAparecer = 1.1f;   

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (painelVitoria) painelVitoria.SetActive(false);
        botaoMenuPrincipal?.onClick.AddListener(IrMenuPrincipal);
        botaoJogarNovamente?.onClick.AddListener(JogarNovamente);
    }

    public void MostrarVitoria(int pontuacao)
    {
        StartCoroutine(AparecerComDelay(pontuacao));
    }

    IEnumerator AparecerComDelay(int pontuacao)
    {
        yield return new WaitForSeconds(delayParaAparecer);

        Time.timeScale = 0f;   
        if (painelVitoria) painelVitoria.SetActive(true);

        if (textoPrincipal)
            textoPrincipal.text = " Parabéns!";

        if (textoSub)
            textoSub.text = "Todos os animais foram salvos!";

        if (textoPontuacao)
            textoPontuacao.text = $"Pontuação final: {pontuacao}";

        if (textoMensagem)
            textoMensagem.text =
                "Você mostrou que é possível combater o abandono!\n" +
                "Na vida real, adote, doe ração e apoie ONGs de proteção animal. ";

        AudioManager.Instance?.TocarVitoria();
    }

    void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.TocarUI();
        SceneManager.LoadScene("StartScene");
    }

    void JogarNovamente()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.TocarUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
