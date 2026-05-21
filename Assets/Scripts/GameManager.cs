using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private List<string> tamedPets       = new List<string>();
    private List<string> killedMonsters  = new List<string>();
    private List<string> activeQuests    = new List<string>();
    private List<string> completedQuests = new List<string>();

    [Header("Vitoria")]
    public int totalPetsParaVencer = 3;

    private bool jaExibiuGato     = false;
    private bool jaExibiuCachorro = false;
    private bool jaExibiuAbandono = false;
    private bool jaExibiuFome     = false;

    public void OnPetTamed(string petType)
    {
        tamedPets.Add(petType);
        Debug.Log($"[GameManager] Pet domesticado: {petType} (total: {tamedPets.Count})");

        AudioManager.Instance?.TocarAdotar();
        HUD.Instance?.RegistrarPetAdotado();

        string tipo = petType.ToLower();
        if ((tipo == "cat") && !jaExibiuGato)
        {
            jaExibiuGato = true;
            TelaEducativa.Instance?.MostrarAdocaoGato();
        }
        else if ((tipo == "dog" || tipo == "rusk") && !jaExibiuCachorro)
        {
            jaExibiuCachorro = true;
            TelaEducativa.Instance?.MostrarAdocaoCachorro();
        }

        if (tamedPets.Count >= totalPetsParaVencer)
            TriggerVitoria();
    }

    void TriggerVitoria()
    {
        int pontos = HUD.Instance != null ? HUD.Instance.GetPontuacao() : 0;
        TelaVitoria.Instance?.MostrarVitoria(pontos);
    }

    public int  GetTamedPetCount()          => tamedPets.Count;
    public bool IsPetTamed(string petType) => tamedPets.Contains(petType.ToLower());

    public void OnMonsterKilled(string monsterName)
    {
        killedMonsters.Add(monsterName);
        Debug.Log($"[GameManager] Monstro morto: {monsterName} (total: {killedMonsters.Count})");

        HUD.Instance?.RegistrarMonstroEliminado();

        string nome = monsterName.ToLower();

        if ((nome.Contains("zan") || nome.Contains("abandono")) && !jaExibiuAbandono)
        {
            jaExibiuAbandono = true;
            TelaEducativa.Instance?.MostrarMonstroAbandono();
        }
        else if ((nome.Contains("esu") || nome.Contains("fome")) && !jaExibiuFome)
        {
            jaExibiuFome = true;
            TelaEducativa.Instance?.MostrarMonstroFome();
        }
    }

    public int GetKillCount() => killedMonsters.Count;

    public void OnQuestStarted(string questId)
    {
        if (!activeQuests.Contains(questId)) { activeQuests.Add(questId); }
    }

    public void OnQuestCompleted(string questId)
    {
        activeQuests.Remove(questId);
        if (!completedQuests.Contains(questId)) completedQuests.Add(questId);
    }

    public bool IsQuestActive(string id)    => activeQuests.Contains(id);
    public bool IsQuestCompleted(string id) => completedQuests.Contains(id);

    public Int32 CalculateHealth(Entity e)
    {
        Int32 r = (e.resistence * 10) + (e.level * 4) + 10;
        return Mathf.Max(r, 10);
    }

    public Int32 CalculateMana(Entity e)
    {
        Int32 r = (e.intelligence * 10) + (e.level * 4) + 5;
        return Mathf.Max(r, 5);
    }

    public Int32 CalculateStamina(Entity e)
    {
        Int32 r = (e.resistence + e.willpower) + (e.level * 2) + 5;
        return Mathf.Max(r, 5);
    }

    public Int32 CalculateDamage(Entity e, int weaponDamage)
    {
        System.Random rnd = new System.Random();
        Int32 r = (e.strength * 2) + (weaponDamage * 2) + (e.level * 3) + rnd.Next(1, 20);
        return Mathf.Max(r, 1);
    }

    public Int32 CalculateDefense(Entity e, int armorDefense)
    {
        Int32 r = (e.resistence * 2) + (e.level * 3) + armorDefense;
        return Mathf.Max(r, 0);
    }
}