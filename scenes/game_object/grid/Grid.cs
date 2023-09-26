using System.Collections.Generic;
using System.Linq;
using Godot;
using TicTacToeMultiplayer.scenes.game_object.cell;

namespace TicTacToeMultiplayer.scenes.game_object.grid;

public partial class Grid : Node2D
{
	public List<Cell> Cells { get; private set; } = null!;

	public void SetActivePlayer(int playerId)
	{
		SetInput(playerId == Multiplayer.GetUniqueId());
		SetMultiplayerAuthority(playerId);
	}
	
	public void SetInput(bool active)
	{
		Cells.ForEach(cell => cell.SetInput(active));
	}

	public override void _Ready()
	{
		Cells = GetChildren().OfType<Cell>().ToList();
	}
}
