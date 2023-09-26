using System.Linq;
using Godot;
using TicTacToeMultiplayer.scenes.autoload.multiplayer_controller;
using TicTacToeMultiplayer.scenes.game_object.cell;
using TicTacToeMultiplayer.scenes.game_object.grid;
using TicTacToeMultiplayer.scripts.cell;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.cell;
using TicTacToeMultiplayer.scripts.extensions;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.game_object.gameplay;

public partial class Gameplay : Node2D, ICellSelectedHandler
{
	[Export]
	private Grid _grid = null!;
	[Export]
	private GameOverChecker _gameOverChecker = null!;
	
	private PlayerInfo _activePlayer = null!;
	
	public void HandleCellSelected(Cell cell)
	{
		cell.Select(_activePlayer.Side, _activePlayer.Id);
		_activePlayer = MultiplayerController.Players.First(player => player.Id != _activePlayer.Id);
		_grid.SetActivePlayer(_activePlayer.Id);

		if (!_gameOverChecker.IsGameOver(_grid, out int? winnerId)) {
			return;
		}
		_grid.SetInput(false);
		//TODO Show cross line animation
		//TODO Show game over screen after animation is finished
		GD.Print($"Game over. Winner is {winnerId}");
	}
	
	public override void _Ready()
	{
		EventBus.Subscribe(this);
		RenderingServer.SetDefaultClearColor(new Color("#3B3B3B", 1f));

		// Called from client to ensure that all players are ready because server is always ready
		if (Multiplayer.IsServer()) {
			return;
		}
		PlayerInfo player = MultiplayerController.Players.PickRandom();
		Rpc(nameof(SetCrossPlayer), player.Id);
	}
	
	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}
	
	[Rpc(RpcMode.AnyPeer, CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void SetCrossPlayer(int playerId)
	{
		PlayerInfo player = MultiplayerController.Players.First(player => player.Id == playerId);
		player.Side = CellType.Cross;
		_activePlayer = player;
		_grid.SetActivePlayer(_activePlayer.Id);
	}
}
