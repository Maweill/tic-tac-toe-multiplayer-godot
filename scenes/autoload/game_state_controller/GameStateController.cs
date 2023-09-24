using Godot;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.autoload.game_state_controller;

public partial class GameStateController : Node, IGameStartHandler
{
	[Export]
	private PackedScene _gameplayScene = null!;
	
	public void HandleGameStart()
	{
		Rpc(nameof(StartGameplay));
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
	private void StartGameplay()
	{
		GetTree().ChangeSceneToPacked(_gameplayScene);
	}
}
