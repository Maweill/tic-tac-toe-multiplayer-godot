using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events.lobby;

public interface IHostAttemptHandler : IGlobalSubscriber
{
	void HandleHostAttempt(string ip, int port);
}
