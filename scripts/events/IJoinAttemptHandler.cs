using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events;

public interface IJoinAttemptHandler : IGlobalSubscriber
{
	void HandleJoinAttempt(string ip, int port);
}
