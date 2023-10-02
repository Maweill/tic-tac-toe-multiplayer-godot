using Godot;
using TicTacToeMultiplayer.scenes.autoload.autoload_helper;
using TicTacToeMultiplayer.scenes.autoload.multiplayer_controller;
using TicTacToeMultiplayer.scenes.ui.game_over_menu;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.game_state;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.autoload.game_state_controller;

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

	public void HandleGameOver(bool isDraw, PlayerModel? winner = null)
	{
		GameOverMenu gameOverMenu = _gameOverMenuScene.Instantiate<GameOverMenu>();
		GetTree().CurrentScene.AddChild(gameOverMenu);
		gameOverMenu.Initialize(isDraw, winner);
	}

	public override void _Ready()
	{
		_multiplayerController = AutoloadHelper.GetAutoload<MultiplayerController>();
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
