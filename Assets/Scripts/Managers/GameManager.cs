using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(HexaGrid))]
[RequireComponent(typeof(PlayersManager))]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Get the GameManager Singleton
    /// </summary>
    public static GameManager Instance;

    /// <summary>
    /// The the hexagrid unique instance
    /// </summary>
    public HexaGrid HexaGrid { get => _hexaGrid; }

    HexaGrid _hexaGrid;
    PlayersManager _players;

    /// <summary>
    /// Start or Restart a new game
    /// </summary>
    public void RestartGame()
    {
        _players.SpawnControlledPlayer();
    }

    /// <summary>
    /// Called when player's base is destroyed and the player is destroy
    /// </summary>
    public void GameOver()
    {
        Debug.Log("GameOver!");
    }

    private void Awake()
    {
        Instance = this;
        _hexaGrid = GetComponent<HexaGrid>();
        _players = GetComponent<PlayersManager>();
        _hexaGrid.Generate();
    }

    private void Start()
    {
        RestartGame();
    }
}
