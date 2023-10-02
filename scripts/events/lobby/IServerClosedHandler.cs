using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events.lobby;

public interface IServerClosedHandler : IGlobalSubscriber
{
	void HandleServerClosed();
}
