using System.Collections.Generic;
using Tetramino.scripts.event_bus_system;
using TicTacToeMultiplayer.scenes.game_object.cell;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scripts.events.game_state;

public interface IGameOverHandler : IGlobalSubscriber
{
	void HandleGameOver(bool isDraw, PlayerModel? winner = null, List<Cell>? winCells = null);
}
