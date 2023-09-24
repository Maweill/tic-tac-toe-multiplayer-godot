using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events;

public interface IGameStartHandler : IGlobalSubscriber
{
	void HandleGameStart();
}
