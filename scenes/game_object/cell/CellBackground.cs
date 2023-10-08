using Godot;

namespace TicTacToeMultiplayer.scenes.game_object.cell;

public partial class CellBackground : Sprite2D
{
	[Export]
	private Color _defaultColor;
	[Export]
	private Color _hoveredColor;
	[Export]
	private Color _winColor;
	
	public void SetHovered(bool hovered)
	{
		Modulate = hovered ? _hoveredColor : _defaultColor;
	}

	public override void _Ready()
	{
		Modulate = _defaultColor;
	}
}
