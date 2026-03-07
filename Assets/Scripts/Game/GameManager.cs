using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Title("References")]
    [SerializeField] private GameplayController _gameplayController;
    [SerializeField] private GameConfigs _gameConfigs;
    public GameplayController GameplayController => _gameplayController;
    public bool IsInitialized { get; private set; }

    private void Start()
    {
        _gameConfigs.Initialize();
        Initialize();
    }

    private void Initialize()
    {
        _gameplayController.Initialize();
        IsInitialized = true;
    }
}