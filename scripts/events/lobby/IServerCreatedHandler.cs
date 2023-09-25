using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events.lobby;

public interface IServerCreatedHandler : IGlobalSubscriber
{
	void HandleServerCreated(string hostIp, int port);
}
