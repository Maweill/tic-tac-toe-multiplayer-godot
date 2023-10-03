using System.Linq;
using Godot;
using TicTacToeMultiplayer.scenes.autoload.multiplayer_controller;
using TicTacToeMultiplayer.scenes.game_object.cell;
using TicTacToeMultiplayer.scenes.game_object.grid;
using TicTacToeMultiplayer.scripts.cell;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.cell;
using TicTacToeMultiplayer.scripts.events.game_state;
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
	
	private PlayerModel _activePlayer = null!;
	
	public void HandleCellSelected(Cell cell)
	{
		cell.Select(_activePlayer);
		_activePlayer = MultiplayerController.Players.First(player => player.Id != _activePlayer.Id);
		_grid.SetActivePlayer(_activePlayer.Id);

		if (!_gameOverChecker.IsGameOver(_grid, out bool isDraw, out PlayerModel? winner)) {
			return;
		}
		_grid.SetInput(false);
		//TODO Show cross line animation
		//TODO Show game over screen after animation is finished
		GD.Print($"Game over.");
		EventBus.RaiseEvent<IGameOverHandler>(h => h?.HandleGameOver(isDraw, winner));
	}
	
	public override void _Ready()
	{
		EventBus.Subscribe(this);

		MultiplayerController.Players.ForEach(player => player.Side = CellType.Circle);
		
		// Called from client to ensure that all players are ready because server is always ready
		if (Multiplayer.IsServer()) {
			return;
		}
		PlayerModel player = MultiplayerController.Players.PickRandom();
		Rpc(nameof(SetCrossPlayer), player.Id);
	}
	
	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}
	
	[Rpc(RpcMode.AnyPeer, CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void SetCrossPlayer(int playerId)
	{
		PlayerModel player = MultiplayerController.Players.First(player => player.Id == playerId);
		player.Side = CellType.Cross;
		_activePlayer = player;
		_grid.SetActivePlayer(_activePlayer.Id);
	}
}
