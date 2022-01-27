using System.Text;
using System.Timers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[AlwaysUpdateSystem, UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class GameSequenceSystem : SystemBase
{

    private enum SequenceStep
    {
        Reset,
        ShowReady,
        ShowingReady,
        ShowSteady,
        ShowingSteady,
        GameStart,
        ShowingGo,
        HideGo,
        GameRunning,
        GamePause,
        GamePaused,
        GameUnpause,
        GameOver,
        ShowingResults,
    }

    private SequenceStep CurrentStep = SequenceStep.Reset;
    private float StepTimer;
    private float GameTimer;
    private StringBuilder TimerStringBuilder = new StringBuilder(10);
    
    protected override void OnUpdate()
    {
        var goRefs = this.GetSingleton<GameObjectRefs>();
        
        switch (CurrentStep)
        {
            case SequenceStep.Reset:
                ResetGame();
                CurrentStep = SequenceStep.ShowReady;
                break;
            case SequenceStep.ShowReady:
                goRefs.IntroDisplay.text = "Ready...";
                goRefs.IntroDisplay.enabled = true;
                StepTimer = 1.85f;
                CurrentStep = SequenceStep.ShowingReady;
                break;
            case SequenceStep.ShowingReady:
                StepTimer -= Time.DeltaTime;
                if (StepTimer <= 0.0f)
                {
                    CurrentStep = SequenceStep.ShowSteady;
                }
                break;
            case SequenceStep.ShowSteady:
                goRefs.IntroDisplay.text = "Set...";
                StepTimer = 0.85f;
                CurrentStep = SequenceStep.ShowingSteady;
                break;
            case SequenceStep.ShowingSteady:
                StepTimer -= Time.DeltaTime;
                if (StepTimer <= 0.0f)
                {
                    CurrentStep = SequenceStep.GameStart;
                }
                break;
            case SequenceStep.GameStart:
                goRefs.IntroDisplay.text = "Go!";
                StepTimer = 1.0f;
                StartGame();
                CurrentStep = SequenceStep.ShowingGo;
                break;
            case SequenceStep.ShowingGo:
                StepTimer -= Time.DeltaTime;
                if (StepTimer <= 0.0f)
                {
                    CurrentStep = SequenceStep.HideGo;
                }
                break;
            case SequenceStep.HideGo:
                goRefs.IntroDisplay.enabled = false;
                CurrentStep = SequenceStep.GameRunning;
                break;
            case SequenceStep.GameRunning:
                if (Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Pause))
                {
                    CurrentStep = SequenceStep.GamePause;
                }

                if (GameTimer <= 0.0f)
                {
                    CurrentStep = SequenceStep.GameOver;
                }
                break;
            case SequenceStep.GamePause:
                StopGame();
                goRefs.IntroDisplay.text = "Paused.";
                goRefs.IntroDisplay.enabled = true;
                CurrentStep = SequenceStep.GamePaused;
                break;
            case SequenceStep.GameOver:
                StopGame();
                StepTimer = 10.0f;
                ShowResults();
                CurrentStep = SequenceStep.ShowingResults;
                break;
            case SequenceStep.GamePaused:
                if (Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Pause))
                {
                    CurrentStep = SequenceStep.GameUnpause;
                }
                break;
            case SequenceStep.GameUnpause:
                goRefs.IntroDisplay.enabled = false;
                StartGame();
                break;
            case SequenceStep.ShowingResults:
                StepTimer -= Time.DeltaTime;
                if (StepTimer <= 0.0f)
                {
                    CurrentStep = SequenceStep.Reset;
                }
                break;
        }

        if (HasSingleton<GameRunning>())
        {
            GameTimer = math.max(GameTimer - Time.DeltaTime, 0);
        }

        float minutes = GameTimer / 60f;
        TimerStringBuilder.Clear();
        TimerStringBuilder.Append((int)math.floor(minutes));
        TimerStringBuilder.Append(":");
        int seconds = (int)math.floor(GameTimer - math.floor(minutes));
        if (seconds < 10)
        {
            TimerStringBuilder.Append('0');
        }
        TimerStringBuilder.Append(seconds);
        goRefs.TimerDisplay.text = TimerStringBuilder.ToString();
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            StopGame();
        }
    }

    private void ShowResults()
    {
        var playersQuery = GetEntityQuery(
            ComponentType.ReadOnly<Player>(),
            ComponentType.ReadOnly<PlayerUIIndex>(),
            ComponentType.ReadOnly<Score>());
        var playerIndices = playersQuery.ToComponentDataArray<PlayerUIIndex>(Allocator.Temp);
        var playerScores = playersQuery.ToComponentDataArray<Score>(Allocator.Temp);

        int highestScoringPlayerIndex = -1;
        int highestPlayerScore = -1;
        for (int i = 0; i < playerIndices.Length; ++i)
        {
            if (playerScores[i].Value > highestPlayerScore)
            {
                highestPlayerScore = playerScores[i].Value;
                highestScoringPlayerIndex = playerIndices[i].Index;
            }
        }
        var goRefs = this.GetSingleton<GameObjectRefs>();

        if (highestScoringPlayerIndex >= 0 && highestScoringPlayerIndex < goRefs.VictoryPanels.Length)
        {
            goRefs.VictoryPanels[highestScoringPlayerIndex].gameObject.SetActive(true);
        }
        
        playerIndices.Dispose();
        playerScores.Dispose();
    }

    private void ResetGame()
    {
        // only trigger a map entity cleanup if the map was previously spawned
        if (HasSingleton<MapWasSpawned>())
        {
            EntityManager.AddComponent<MapReset>(GetSingletonEntity<MapSpawner>());
        }
        
        foreach (var victoryPanel in this.GetSingleton<GameObjectRefs>().VictoryPanels)
        {
            victoryPanel.gameObject.SetActive(false);
        }
        GameTimer = GetSingleton<Config>().GameTimeInSeconds;
        Entities
            .WithAll<Player>()
            .ForEach((ref Score score) =>
            {
                score.Value = 0;
            }).Run();
    }

    private void StartGame()
    {
        if (!HasSingleton<GameRunning>())
            EntityManager.CreateEntity(ComponentType.ReadWrite<GameRunning>());
    }

    private void StopGame()
    {
        if (HasSingleton<GameRunning>())
            EntityManager.DestroyEntity(GetSingletonEntity<GameRunning>());
    }
}