using System.Collections.Generic;
using System.Linq;
using Godot;
using TicTacToeMultiplayer.scenes.game_object.cell;
using TicTacToeMultiplayer.scenes.game_object.grid;
using TicTacToeMultiplayer.scripts.cell;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scenes.game_object.gameplay;

public partial class GameOverChecker : Node
{
	public bool IsGameOver(Grid grid, out bool isDraw, out PlayerModel? winner, out List<Cell>? cells)
	{
		winner = null;
		isDraw = false;
		cells = null;
		if (IsHorizontalLine(grid, out winner, out cells)) {
			return true;
		}
		if (IsVerticalLine(grid, out winner, out cells)) {
			return true;
		}
		if (IsDiagonalLine(grid, out winner, out cells)) {
			return true;
		}
		isDraw = IsDraw(grid);
		return isDraw;
	}

	private bool IsHorizontalLine(Grid grid, out PlayerModel? winner, out List<Cell>? cells)
	{
		winner = null;
		cells = null;
		for (int i = 0; i < 3; i++) {
			if (grid.Cells[i * 3].CellType == grid.Cells[i * 3 + 1].CellType &&
			    grid.Cells[i * 3].CellType == grid.Cells[i * 3 + 2].CellType &&
			    grid.Cells[i * 3].CellType != CellType.Empty) {
				winner = grid.Cells[i * 3].Player;
				cells = new List<Cell> {grid.Cells[i * 3], grid.Cells[i * 3 + 1], grid.Cells[i * 3 + 2]};
				return true;
			}
		}
		return false;
	}
	
	private bool IsVerticalLine(Grid grid, out PlayerModel? winner, out List<Cell>? cells)
	{
		winner = null;
		cells = null;
		for (int i = 0; i < 3; i++) {
			if (grid.Cells[i].CellType == grid.Cells[i + 3].CellType &&
			    grid.Cells[i].CellType == grid.Cells[i + 6].CellType &&
			    grid.Cells[i].CellType != CellType.Empty) {
				winner = grid.Cells[i].Player;
				cells = new List<Cell> {grid.Cells[i], grid.Cells[i + 3], grid.Cells[i + 6]};
				return true;
			}
		}
		return false;
	}
	
	private bool IsDiagonalLine(Grid grid, out PlayerModel? winner, out List<Cell>? cells)
	{
		winner = null;
		cells = null;
		if (grid.Cells[0].CellType == grid.Cells[4].CellType &&
		    grid.Cells[0].CellType == grid.Cells[8].CellType &&
		    grid.Cells[0].CellType != CellType.Empty) {
			winner = grid.Cells[0].Player;
			cells = new List<Cell> {grid.Cells[0], grid.Cells[4], grid.Cells[8]};
			return true;
		}
		if (grid.Cells[2].CellType == grid.Cells[4].CellType &&
		    grid.Cells[2].CellType == grid.Cells[6].CellType &&
		    grid.Cells[2].CellType != CellType.Empty) {
			winner = grid.Cells[2].Player;
			cells = new List<Cell> {grid.Cells[2], grid.Cells[4], grid.Cells[6]};
			return true;
		}
		return false;
	}
	
	private bool IsDraw(Grid grid)
	{
		return grid.Cells.All(cell => cell.CellType != CellType.Empty);
	}
}
