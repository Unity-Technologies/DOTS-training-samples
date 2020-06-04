using System.Text;
using Unity.Entities;

public class RoundGameplaySystem : SystemBase
{
    private bool m_Initialised;

    private float m_RoundLength;
    private float m_RemainingRoundTime;

    private Entity m_GameplayEntity;

    protected override void OnCreate()
    {
        m_GameplayEntity = EntityManager.CreateEntity(ComponentType.ReadWrite<GameModeComponent>());
        SetSingleton(new GameModeComponent {Value = GameMode.Intro});
    }

    protected override void OnStartRunning()
    {
        EntityManager.CreateEntity(ComponentType.ReadOnly<GenerateGridRequestComponent>());

        base.OnStartRunning();
    }


    bool Init()
    {
        if (m_Initialised)
        {
            return true;
        }

        var constantData = ConstantData.Instance;
        if (constantData == null)
        {
            return false;
        }

        m_RoundLength = constantData.RoundLength;

        return true;
    }

    protected override void OnUpdate()
    {
        if (!Init())
        {
            return;
        }


        var currentMode = GetSingleton<GameModeComponent>().Value;
        if (currentMode == GameMode.Intro)
        {
            StartGame();
        }
        else if (currentMode == GameMode.GamePlay)
        {
            UpdateGame();
        }
    }

    void StartGame()
    {
        SetSingleton(new GameModeComponent() {Value = GameMode.GamePlay});
        m_RemainingRoundTime = m_RoundLength;
    }

    void UpdateGame()
    {
        m_RemainingRoundTime -= Time.DeltaTime;


        var uiHelper = UIHelper.Instance;
        if (uiHelper)
        {
            uiHelper.SetRemainingTime(m_RemainingRoundTime);
        }

        if (m_RemainingRoundTime <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        SetSingleton(new GameModeComponent() {Value = GameMode.GameOver});

        var uiHelper = UIHelper.Instance;
        if (uiHelper)
        {
            var scoreSystem = World.GetExistingSystem<ScoreSystem>();
            var winningPlayers = scoreSystem.GetWinningPlayers();

            var gameOverText = "Nobody wins";
            if (winningPlayers.Count == 1)
            {
                gameOverText = $"Player {winningPlayers[0]} Wins!";
            }
            else if (winningPlayers.Count > 1)
            {
                gameOverText = "Draw between Players: ";
                for (int i = 0; i < winningPlayers.Count; i++)
                {
                    if (i + 1 < winningPlayers.Count)
                        gameOverText += i + ",";
                    else
                        gameOverText += i;
                }
            }

            uiHelper.ShowGameOverScreen(gameOverText);
        }
    }
}