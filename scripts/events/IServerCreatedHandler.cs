using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events;

public interface IServerCreatedHandler : IGlobalSubscriber
{
	void HandleServerCreated(string hostIp);
}
