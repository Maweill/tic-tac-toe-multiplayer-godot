using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.events.game_state;

public interface IGameStartAttemptHandler : IGlobalSubscriber
{
	void HandleGameStartAttempt();
}
