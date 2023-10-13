using Godot;
using TicTacToeMultiplayer.scenes.controller.multiplayer_controller;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.game_state;

namespace TicTacToeMultiplayer.scenes.ui.game_over_menu;

public partial class GameOverMenu : CanvasLayer
{
	[Export]
	private Label _resultHeaderLabel = null!;
	[Export]
	private Button _openLobbyButton = null!;
	[Export]
	private GameOverSoundPlayer _soundPlayer = null!;

	private MultiplayerController _multiplayerController = null!;
	
	private bool _isWin;
	
	public void Initialize(bool isDraw, bool isWinner)
	{
		if (isDraw) {
			_resultHeaderLabel.Text = "Draw!";
		} else {
			_resultHeaderLabel.Text = isWinner ? "😄\nYou won!" : "😭\nYou lost!";
		}
		_isWin = !isDraw && isWinner;
	}

	public override void _Ready()
	{
		_openLobbyButton.Pressed += OnOpenLobbyButtonPressed;
		_soundPlayer.PlaySound(_isWin);
	}

	private void OnOpenLobbyButtonPressed()
	{
		EventBus.RaiseEvent<IOpenLobbyAttemptHandler>(h => h?.HandleOpenLobbyAttempt());
	}
}
