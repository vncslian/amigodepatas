using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip musicaFundo;
    [Range(0f, 1f)] public float volumeMusica = 0.104f;

    public AudioClip somAdotar;
    public AudioClip somTreat;
    public AudioClip somPetLibertado;
    public AudioClip somCachorroCapturado; 
    public AudioClip somGatoCapturado;     

    public AudioClip somAtaquePlayer;
    public AudioClip somHitPlayer;
    public AudioClip somDanoPlayer;
    public AudioClip somMortePlayer;

    public AudioClip somAtaqueMonstro;
    public AudioClip somHitMonstro;
    public AudioClip somMorteMonstro;

    public AudioClip somPassoPlayer;
    public AudioClip somPassoMonstro;
    public AudioClip somPassoCachorro; 
    public AudioClip somPassoGato;     

    [Range(0f, 1f)] public float volumePassoPlayer = 1.0f;   
    [Range(0f, 1f)] public float volumePassoGato = 1.0f;     
    [Range(0f, 1f)] public float volumePassoCachorro = 1.0f; 
    [Range(0f, 1f)] public float volumePassoMonstro = 0.2f;  

    public AudioClip somLevelUp;
    public AudioClip somUI;
    public AudioClip somVitoria;
    public AudioClip somErro;

    private AudioSource audioSourcePrincipal;
    private AudioSource canalPassoPlayer;
    private AudioSource canalPassoMonstro;
    private AudioSource canalPassoPet;

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSourcePrincipal = GetComponent<AudioSource>();
        if (audioSourcePrincipal == null) audioSourcePrincipal = gameObject.AddComponent<AudioSource>();

        canalPassoPlayer = gameObject.AddComponent<AudioSource>();
        canalPassoMonstro = gameObject.AddComponent<AudioSource>();
        canalPassoPet = gameObject.AddComponent<AudioSource>();

        canalPassoPlayer.spatialBlend = 0f;
        canalPassoMonstro.spatialBlend = 0f;
        canalPassoPet.spatialBlend = 0f;
    }

    void Start()
    {
        TocarMusicaFundo();
    }

    public void TocarMusicaFundo()
    {
        if (musicaFundo != null && audioSourcePrincipal != null)
        {
            audioSourcePrincipal.clip = musicaFundo;
            audioSourcePrincipal.volume = volumeMusica;
            audioSourcePrincipal.loop = true;
            audioSourcePrincipal.Play();
        }
    }

    public void PararMusica() { if (audioSourcePrincipal != null && audioSourcePrincipal.isPlaying) audioSourcePrincipal.Pause(); }
    public void TocarMusica() { if (audioSourcePrincipal != null && !audioSourcePrincipal.isPlaying) audioSourcePrincipal.UnPause(); }

    public void TocarPassoPlayer() 
    { 
        if (somPassoPlayer != null && canalPassoPlayer != null) 
        {
            if (canalPassoPlayer.isPlaying) return;
            canalPassoPlayer.clip = somPassoPlayer;
            canalPassoPlayer.volume = volumePassoPlayer;
            canalPassoPlayer.Play(); 
        }
    }
    public void PararPassoPlayer() { if (canalPassoPlayer != null && canalPassoPlayer.isPlaying) canalPassoPlayer.Stop(); }

    public void TocarPassoMonstro() 
    { 
        if (somPassoMonstro != null && canalPassoMonstro != null) 
        {
            if (canalPassoMonstro.isPlaying) return;
            canalPassoMonstro.clip = somPassoMonstro;
            canalPassoMonstro.volume = volumePassoMonstro;
            canalPassoMonstro.Play(); 
        }
    }
    public void PararPassoMonstro() { if (canalPassoMonstro != null && canalPassoMonstro.isPlaying) canalPassoMonstro.Stop(); }

    public void TocarPassoCachorro() 
    { 
        if (somPassoCachorro != null && canalPassoPet != null) 
        {
            if (canalPassoPet.isPlaying && canalPassoPet.clip == somPassoCachorro) return;
            canalPassoPet.clip = somPassoCachorro;
            canalPassoPet.volume = volumePassoCachorro;
            canalPassoPet.Play();
        }
    }
    public void TocarPassoGato() 
    { 
        if (somPassoGato != null && canalPassoPet != null) 
        {
            if (canalPassoPet.isPlaying && canalPassoPet.clip == somPassoGato) return;
            canalPassoPet.clip = somPassoGato;
            canalPassoPet.volume = volumePassoGato;
            canalPassoPet.Play();
        }
    }
    public void PararPassoPet() { if (canalPassoPet != null && canalPassoPet.isPlaying) canalPassoPet.Stop(); }

    public void TocarAtaquePlayer() { if (somAtaquePlayer != null) audioSourcePrincipal.PlayOneShot(somAtaquePlayer); }
    public void TocarHitPlayer() { if (somHitPlayer != null) audioSourcePrincipal.PlayOneShot(somHitPlayer); }
    public void TocarDanoPlayer() { if (somDanoPlayer != null) audioSourcePrincipal.PlayOneShot(somDanoPlayer); }
    public void TocarMortePlayer() { if (somMortePlayer != null) { PararPassoPlayer(); audioSourcePrincipal.PlayOneShot(somMortePlayer); } }

    public void TocarAtaqueMonstro() { if (somAtaqueMonstro != null) audioSourcePrincipal.PlayOneShot(somAtaqueMonstro); }
    public void TocarHitMonstro() { if (somHitMonstro != null) audioSourcePrincipal.PlayOneShot(somHitMonstro); }
    public void TocarMorteMonstro() { if (somMorteMonstro != null) { PararPassoMonstro(); audioSourcePrincipal.PlayOneShot(somMorteMonstro); } }

    public void TocarAdotar() { if (somAdotar != null) audioSourcePrincipal.PlayOneShot(somAdotar); }
    public void TocarTreat() { if (somTreat != null) audioSourcePrincipal.PlayOneShot(somTreat); }
    public void TocarPetLibertado() { if (somPetLibertado != null) audioSourcePrincipal.PlayOneShot(somPetLibertado); }
    public void TocarCachorroCapturado() { if (somCachorroCapturado != null) audioSourcePrincipal.PlayOneShot(somCachorroCapturado); }
    public void TocarGatoCapturado() { if (somGatoCapturado != null) audioSourcePrincipal.PlayOneShot(somGatoCapturado); }

    public void TocarLevelUp() { if (somLevelUp != null) audioSourcePrincipal.PlayOneShot(somLevelUp); }
    public void TocarUI() { if (somUI != null) audioSourcePrincipal.PlayOneShot(somUI); }
    public void TocarErro() { if (somErro != null) audioSourcePrincipal.PlayOneShot(somErro); }

    public void TocarVitoria() 
    { 
        if (somVitoria != null && audioSourcePrincipal != null) 
        {
            PararPassoPlayer();
            PararPassoMonstro();
            PararPassoPet();
            audioSourcePrincipal.loop = false; 
            audioSourcePrincipal.Stop(); 
            audioSourcePrincipal.clip = somVitoria;
            audioSourcePrincipal.volume = 1.0f; 
            audioSourcePrincipal.Play(); 
        } 
    }
}