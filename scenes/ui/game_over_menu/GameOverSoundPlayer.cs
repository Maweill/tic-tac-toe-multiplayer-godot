using Godot;

namespace TicTacToeMultiplayer.scenes.ui.game_over_menu;

public partial class GameOverSoundPlayer : AudioStreamPlayer
{
	[Export]
	private AudioStreamWav _winSound = null!;
	[Export]
	private AudioStreamWav _loseSound = null!;
	
	public void PlaySound(bool isWin)
	{
		Stream = isWin ? _winSound : _loseSound;
		Play();
	}
}
