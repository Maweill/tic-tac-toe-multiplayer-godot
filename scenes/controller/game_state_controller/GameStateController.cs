using System.Collections.Generic;
using Godot;
using Godot.DependencyInjection.Attributes;
using JetBrains.Annotations;
using TicTacToeMultiplayer.scenes.game_object.cell;
using TicTacToeMultiplayer.scenes.ui.game_over_menu;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.game_state;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.MultiplayerPeer;
using MultiplayerController = TicTacToeMultiplayer.scenes.controller.multiplayer_controller.MultiplayerController;

namespace TicTacToeMultiplayer.scenes.controller.game_state_controller;

public partial class GameStateController : Node, IGameStartAttemptHandler, IOpenLobbyAttemptHandler, IGameOverHandler
{
	[Export]
	private PackedScene _lobbyScene = null!;
	[Export]
	private PackedScene _gameplayScene = null!;
	[Export]
	private PackedScene _gameOverMenuScene = null!;
	
	private MultiplayerController _multiplayerController = null!;

	public void HandleOpenLobbyAttempt()
	{
		OpenLobby();
	}

	public void HandleGameStartAttempt()
	{
		Rpc(nameof(StartGameplay));
	}

	public void HandleGameOver(bool isDraw, PlayerModel? winner, List<Cell>? winCells)
	{
		GameOverMenu gameOverMenu = _gameOverMenuScene.Instantiate<GameOverMenu>();
		GetTree().CurrentScene.AddChild(gameOverMenu);
		gameOverMenu.Initialize(isDraw, winner);
	}
	
	[Inject] [UsedImplicitly]
	public void Construct(MultiplayerController multiplayerController)
	{
		_multiplayerController = multiplayerController;
	}

	public override void _Ready()
	{
		RenderingServer.SetDefaultClearColor(new Color("#00A4F5", 1f));
		EventBus.Subscribe(this);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}

	private void OpenLobby()
	{
		GetTree().ChangeSceneToPacked(_lobbyScene);
		_multiplayerController.Rpc(MultiplayerController.MethodName.ChangePlayerStatus, 
		                           Multiplayer.GetUniqueId(), (int) PlayerStatus.LOBBY);
	}

	[Rpc(CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void StartGameplay()
	{
		GetTree().ChangeSceneToPacked(_gameplayScene);
		_multiplayerController.Rpc(MultiplayerController.MethodName.ChangePlayerStatus, 
		                           Multiplayer.GetUniqueId(), (int) PlayerStatus.GAME);
	}
}
