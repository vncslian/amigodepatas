# Amigo de Patas — Jogo Educativo 2D

**Disciplina:** Desenvolvimento de Jogos Educativos (TED 3)  
**Engine:** Unity 6.4 | **Linguagem:** C#  
**Tema:** Conscientização sobre adoção responsável e combate ao abandono animal.

---

## Sobre o Jogo
O **Amigo de Patas** é um jogo top-down em pixel art focado na causa social da proteção animal. O jogador deve percorrer o mapa procurando os petiscos spawnados pelo mapa e fornecendo 2 unidades aos animais abandonados para adotá-los, enquanto enfrenta duas ameaças simbólicas:
* **Ésúris:** O monstro que personifica a **Fome**.
* **Zangas:** O monstro que personifica o **Abandono**.

Os monstros perseguem e capturam os pets. O jogador precisa combater essas ameaças para libertar os animais e concluir as adoções.

---

## Tecnologias e Arquitetura Implementadas (TED 3)

### 1. Sistema de UI & Telas Educativas
* **Menu de Início (`StartScene`):** Canvas em modo *Screen Space - Overlay* gerenciando o fluxo de entrada e saída.
* **Menu de Pausa (`PauseMenu.cs`):** Ativado via tecla `ESC`. Congela as físicas do jogo com `Time.timeScale = 0` e interrompe as trilhas musicais.
* **HUD Responsivo (`HUD.cs`):** Utiliza *Canvas Scaler* em modo *Scale With Screen Size* (Base 1920x1080) com atualização em tempo real de Sliders de atributos.
* **Telas Educativas (`TelaEducativa.cs`):** Pop-ups com dados reais de ONGs e legislações (ex: Lei Federal 9.605/98). Controlado por travas booleanas no `GameManager.cs` para garantir exibição única por partida.

### 2. Sonorização Adaptativa
* **Audio-Managers Persistentes:** Dividido em `AudioManager.cs` e `MusicManager.cs` usando o padrão Singleton e `DontDestroyOnLoad`.
* **Áudio 2D Puro:** Componentes injetados dinamicamente via script com parâmetro *Spatial Blend* fixado em `0`.
* **Trilha Sonora Adaptativa:** Coroutine que avalia o estado de combate a cada 0.5s e executa um *crossfade* suave utilizando `Mathf.Lerp` em um intervalo de 1.2s entre a música ambiente e de combate.
* **Efeitos Estocásticos:** Utilização de `PlayOneShot()` para sobreposição harmônica de sons de combate e passos.

### 3. Refatoração e Evolução do Código
* **Desacoplamento por Singletons:** Redução drástica de referências manuais no Inspector através de instâncias globais com dupla checagem no `Awake()`.
* **Proteção de Atributos:** Inclusão de funções matemáticas como `Mathf.Max()` para sanear o cálculo de dano e impedir bugs de imortalidade ou morte instantânea.

---

## Estrutura do Repositório
* `/Assets/Scripts/` — Código-fonte em C# (Singletons, controladores de IA e combate).
* `/Assets/Animations/` — Clipes de animação e *Animator Controllers*.
* `/Assets/Prefabs/` — Entidades configuradas (Player, Monstros e Pets).
* `/Assets/Sounds/` — Arquivos de áudio digital (`.wav` e `.mp3`).
* `/Assets/Scenes/` — Cenas estruturadas (`StartScene` e `SampleScene`).
* `/.gitignore` — Arquivo de filtragem para a Unity.

---

## 🛠️ Como Executar o Projeto Localmente
1. Baixe e instale o **Unity Hub** e a versão correspondente da engine (**Unity 6**).
2. Clone este repositório em sua máquina:
   ```bash
   git clone [https://github.com/vncslian/amigodepatas.git](https://github.com/vncslian/amigodepatas.git)

3. Abra o Unity Hub, clique em **Add > Add project from disk** e selecione a pasta clonada.
4. Abra o projeto e certifique-se de iniciar pela cena **StartScene** localizada em `Assets/Scenes/` para garantir o fluxo correto do jogo.

---

## Equipe Desenvolvedora
* **Felix Vinícius Liandro de Freitas** (Líder de Projeto)
* **Emanuela da Conceição Barbosa** (Vice-líder / Designer de Interface)
* **Marcos Miguel Coutinho** (Desenvolvedor Full-Stack)

---
_Nota: O relatório de evolução acadêmica completo com a análise aprofundada dos prints e o detalhamento técnico estendido foi submetido em formato PDF no portal acadêmico da instituição UniBALSAS._
