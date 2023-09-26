using Godot;
using TicTacToeMultiplayer.scenes.game_object.grid;
using TicTacToeMultiplayer.scripts.cell;

namespace TicTacToeMultiplayer.scenes.game_object.gameplay;

public partial class GameOverChecker : Node
{
	public bool IsGameOver(Grid grid, out int? winnerId)
	{
		winnerId = null;
		if (IsHorizontalLine(grid, out winnerId)) {
			return true;
		}
		if (IsVerticalLine(grid, out winnerId)) {
			return true;
		}
		if (IsDiagonalLine(grid, out winnerId)) {
			return true;
		}
		return false;
	}

	private bool IsHorizontalLine(Grid grid, out int? winnerId)
	{
		winnerId = null;
		for (int i = 0; i < 3; i++) {
			if (grid.Cells[i * 3].CellType == grid.Cells[i * 3 + 1].CellType &&
			    grid.Cells[i * 3].CellType == grid.Cells[i * 3 + 2].CellType &&
			    grid.Cells[i * 3].CellType != CellType.Empty) {
				winnerId = grid.Cells[i * 3].PlayerId;
				return true;
			}
		}
		return false;
	}
	
	private bool IsVerticalLine(Grid grid, out int? winnerId)
	{
		winnerId = null;
		for (int i = 0; i < 3; i++) {
			if (grid.Cells[i].CellType == grid.Cells[i + 3].CellType &&
			    grid.Cells[i].CellType == grid.Cells[i + 6].CellType &&
			    grid.Cells[i].CellType != CellType.Empty) {
				winnerId = grid.Cells[i].PlayerId;
				return true;
			}
		}
		return false;
	}
	
	private bool IsDiagonalLine(Grid grid, out int? winnerId)
	{
		winnerId = null;
		if (grid.Cells[0].CellType == grid.Cells[4].CellType &&
		    grid.Cells[0].CellType == grid.Cells[8].CellType &&
		    grid.Cells[0].CellType != CellType.Empty) {
			winnerId = grid.Cells[0].PlayerId;
			return true;
		}
		if (grid.Cells[2].CellType == grid.Cells[4].CellType &&
		    grid.Cells[2].CellType == grid.Cells[6].CellType &&
		    grid.Cells[2].CellType != CellType.Empty) {
			winnerId = grid.Cells[2].PlayerId;
			return true;
		}
		return false;
	}
}
