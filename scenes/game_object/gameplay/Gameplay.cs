using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.DependencyInjection.Attributes;
using JetBrains.Annotations;
using TicTacToeMultiplayer.scenes.game_object.cell;
using TicTacToeMultiplayer.scenes.game_object.grid;
using TicTacToeMultiplayer.scripts.cell;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.cell;
using TicTacToeMultiplayer.scripts.events.game_state;
using TicTacToeMultiplayer.scripts.extensions;
using TicTacToeMultiplayer.scripts.models;
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
	
	private MultiplayerModel _multiplayerModel = null!;
	private PlayerModel _activePlayer = null!;
	
	public void HandleCellSelected(Cell cell)
	{
		cell.Select(_activePlayer);
		_activePlayer = _multiplayerModel.Players.First(player => player.Id != _activePlayer.Id);
		_grid.SetActivePlayer(_activePlayer.Id);

		if (!_gameOverChecker.IsGameOver(_grid, out bool isDraw, out PlayerModel? winner, out List<Cell>? winCells)) {
			return;
		}
		
		HandleGameOver(isDraw, winner, winCells);
	}

	private async void HandleGameOver(bool isDraw, PlayerModel? winner, List<Cell>? winCells)
	{
		GD.Print("Game over.");
		_grid.SetInput(false);
		if (!isDraw) {
			await MarkWinCells();
		}
		await ToSignal(GetTree().CreateTimer(1f), Timer.SignalName.Timeout);
		EventBus.RaiseEvent<IGameOverHandler>(h => h?.HandleGameOver(isDraw, winner, winCells));
		return;

		async Task MarkWinCells()
		{
			foreach (Cell winCell in winCells!) {
				await winCell.MarkAsWin();
			}
		}
	}
	
	[Inject] [UsedImplicitly]
	public void Construct(MultiplayerModel multiplayerModel)
	{
		_multiplayerModel = multiplayerModel;
	}
	
	public override void _Ready()
	{
		EventBus.Subscribe(this);
		
		_multiplayerModel.Players.ForEach(player => player.Side = CellType.Circle);
		
		// Called from client to ensure that all players are ready because server is always ready
		if (Multiplayer.IsServer()) {
			return;
		}
		PlayerModel player = _multiplayerModel.Players.PickRandom();
		Rpc(nameof(SetCrossPlayer), player.Id);
	}
	
	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}
	
	[Rpc(RpcMode.AnyPeer, CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void SetCrossPlayer(int playerId)
	{
		PlayerModel player = _multiplayerModel.Players.First(player => player.Id == playerId);
		player.Side = CellType.Cross;
		_activePlayer = player;
		_grid.SetActivePlayer(_activePlayer.Id);
	}
}
