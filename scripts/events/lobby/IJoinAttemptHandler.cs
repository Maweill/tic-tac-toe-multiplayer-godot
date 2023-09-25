using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events.lobby;

public interface IJoinAttemptHandler : IGlobalSubscriber
{
	void HandleJoinAttempt(string ip, int port);
}
