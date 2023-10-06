using Tetramino.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.models;

namespace TicTacToeMultiplayer.scripts.events.models.multiplayer_model;

public interface IMultiplayerModelChangedHandler : IGlobalSubscriber
{
	void HandleMultiplayerModelChanged(MultiplayerModel multiplayerModel);
}
