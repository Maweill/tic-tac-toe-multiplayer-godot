using Godot;
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

	public void HandleOpenLobbyAttempt()
	{
		Rpc(nameof(OpenLobby));
	}

	public void HandleGameStartAttempt()
	{
		Rpc(nameof(StartGameplay));
	}

	public void HandleGameOver(bool isDraw, PlayerInfo? winner = null)
	{
		GameOverMenu gameOverMenu = _gameOverMenuScene.Instantiate<GameOverMenu>();
		GetTree().CurrentScene.AddChild(gameOverMenu);
		gameOverMenu.Initialize(isDraw, winner);
	}

	public override void _Ready()
	{
		EventBus.Subscribe(this);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}

	[Rpc(CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void OpenLobby()
	{
		GetTree().ChangeSceneToPacked(_lobbyScene);
	}

	[Rpc(CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void StartGameplay()
	{
		GetTree().ChangeSceneToPacked(_gameplayScene);
	}
}
