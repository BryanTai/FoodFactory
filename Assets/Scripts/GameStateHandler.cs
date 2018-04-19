using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Intro, NotDetected, Detected, GameOver }

static class GameStateMethods
{
    public static string GetString(this GameState state)
    {
        switch (state)
        {
            case GameState.Intro:
                return "Intro";
            case GameState.NotDetected:
                return "Not Detected";
            case GameState.Detected:
                return "Detected";
            case GameState.GameOver:
                return "Game Over";
            default:
                throw new ArgumentException();
        }
    }

    public static bool InGame(this GameState state)
    {
        return (state == GameState.NotDetected) || (state == GameState.Detected);
    }
}

public class GameStateHandler
{
    public GameState CurrentGameState { get; private set; }

    public void OnStart()
    {
        CurrentGameState = GameState.Intro;
    }

    public void SetCurrentState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Intro:
            case GameState.GameOver:
                CurrentGameState = newState;
                break;
            case GameState.NotDetected:
            case GameState.Detected:
                if (CurrentGameState.InGame())
                {
                    CurrentGameState = newState;
                }
                break;
            default:
                throw new ArgumentException();
        }
    }
}
