using System.Collections.Generic;
using System.Linq;
using Godot;
using TicTacToeMultiplayer.scenes.game_object.cell;

namespace TicTacToeMultiplayer.scenes.game_object.grid;

public partial class Grid : Node2D
{
	private List<Cell> _cells = null!;

	public void SetActivePlayer(int playerId)
	{
		if (playerId != Multiplayer.GetUniqueId()) {
			_cells.ForEach(cell => cell.SetInput(false));
		} else {
			_cells.ForEach(cell => cell.SetInput(true));
		}
		SetMultiplayerAuthority(playerId);
	}

	public override void _Ready()
	{
		_cells = GetChildren().OfType<Cell>().ToList();
	}
}
