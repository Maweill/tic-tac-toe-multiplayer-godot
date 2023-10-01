using Godot;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.game_state;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scenes.ui.game_over_menu;

public partial class GameOverMenu : CanvasLayer
{
	[Export]
	private Label _resultHeaderLabel = null!;
	[Export]
	private Button _openLobbyButton = null!;

	public void Initialize(bool isDraw, PlayerInfo? winner)
	{
		_resultHeaderLabel.Text = isDraw ? "Draw!" : (Multiplayer.GetUniqueId() == winner!.Id ? "You won!" : "You lost :(");
	}

	public override void _Ready()
	{
		_openLobbyButton.Pressed += OnOpenLobbyButtonPressed;
	}

	private void OnOpenLobbyButtonPressed()
	{
		EventBus.RaiseEvent<IOpenLobbyAttemptHandler>(h => h?.HandleOpenLobbyAttempt());
	}
}
