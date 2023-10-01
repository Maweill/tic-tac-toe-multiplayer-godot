using Tetramino.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scripts.events.game_state;

public interface IGameOverHandler : IGlobalSubscriber
{
	void HandleGameOver(bool isDraw, PlayerInfo? winner = null);
}
