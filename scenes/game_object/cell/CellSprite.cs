using Godot;
using TicTacToeMultiplayer.scripts.cell;

namespace TicTacToeMultiplayer.scenes.game_object.cell;

public partial class CellSprite : Sprite2D
{
	[Export]
	private CompressedTexture2D _crossTexture = null!;
	[Export]
	private CompressedTexture2D _circleTexture = null!;

	public void SetTexture(CellType cellType)
	{
		switch (cellType) {
			case CellType.Empty:
				Texture = null;
				break;
			case CellType.Cross:
				Texture = _crossTexture;
				break;
			case CellType.Circle:
				Texture = _circleTexture;
				break;
		}
	}
}
