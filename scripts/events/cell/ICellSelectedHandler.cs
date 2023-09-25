using Tetramino.scripts.event_bus_system;
using TicTacToeMultiplayer.scenes.game_object.cell;

namespace TicTacToeMultiplayer.scripts.events.cell;

public interface ICellSelectedHandler : IGlobalSubscriber
{
	void HandleCellSelected(Cell cell);
}
