using System;
using UnityEngine;

public class GameModel
{
    public float CurrentTime { get; private set; }
    public bool IsTimerRunning { get; private set; }

    public Action<float> OnTimeChanged;
    public Action OnTimeUp;

    public GameModel(GameData data)
    {
        CurrentTime = data.gameTimeSeconds;
        IsTimerRunning = false;
    }

    public void StartTimer()
    {
        IsTimerRunning = true;
    }
    public void StopTimer()
    {
        IsTimerRunning = false;
    }

    public void Tick(float deltaTime)
    {
        if (!IsTimerRunning) return;

        CurrentTime -= deltaTime;

        if (CurrentTime <= 0)
        {
            CurrentTime = 0;
            IsTimerRunning = false;
            OnTimeUp?.Invoke();
        }

        OnTimeChanged?.Invoke(CurrentTime);
    }
}
