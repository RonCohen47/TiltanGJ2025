using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputManager _inputManager;
    [SerializeField] private Transform[] _spawnPoints; // size 4

    private readonly List<PlayerInput> _players = new();

    private void Reset()
    {
        _inputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        if (_inputManager == null) _inputManager = GetComponent<PlayerInputManager>();
        _inputManager.onPlayerJoined += OnPlayerJoined;
        _inputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        if (_inputManager == null) return;
        _inputManager.onPlayerJoined -= OnPlayerJoined;
        _inputManager.onPlayerLeft -= OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput pi)
    {
        _players.Add(pi);
        // Place at spawn
        int idx = Mathf.Clamp(pi.playerIndex, 0, _spawnPoints.Length - 1);
        if (_spawnPoints != null && _spawnPoints.Length > 0 && _spawnPoints[idx] != null)
        {
            pi.transform.SetPositionAndRotation(_spawnPoints[idx].position, _spawnPoints[idx].rotation);
        }
        // Define colors for up to 4 players
        Color[] playerColors = new Color[]
        {
        Color.red,
        Color.blue,
        Color.yellow,
        new Color(0.5f, 0.8f, 1f) // light blue (custom)
        };

        // Assign color to the player's sprite
        var sr = pi.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = playerColors[pi.playerIndex % playerColors.Length];
        }
        else
        {
            Debug.LogWarning($"Player {pi.playerIndex} has no SpriteRenderer to color.");
        }
        Debug.Log($"<color=green>Player {pi.playerIndex} joined the game.</color>");
    }

    private void OnPlayerLeft(PlayerInput pi)
    {
        _players.Remove(pi);
        Debug.Log($"<color=red>Player {pi.playerIndex} left the game.</color>");
        Destroy(pi.gameObject);
    }
}
