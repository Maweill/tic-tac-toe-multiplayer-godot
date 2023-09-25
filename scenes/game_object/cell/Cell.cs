using Godot;
using TicTacToeMultiplayer.scripts.cell;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.cell;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.game_object.cell;

public partial class Cell : Node2D
{
	[Export]
	private CellSprite _sprite = null!;
	[Export]
	private Area2D _clickableArea = null!;
	
	public void Select(CellType cellType)
	{
		_sprite.SetTexture(cellType);
	}

	public void SetInput(bool active)
	{
		_clickableArea.InputPickable = active;
	}

	public override void _Ready()
	{
		_clickableArea.InputEvent += OnClickableAreaInputEvent;
	}

	private void OnClickableAreaInputEvent(Node viewport, InputEvent @event, long shapeidx)
	{
		if (!@event.IsActionPressed("select_cell")) {
			return;
		}
		Rpc(nameof(OnCellClicked));
	}
	
	[Rpc(CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void OnCellClicked()
	{
		GD.Print("Cell clicked");
		EventBus.RaiseEvent<ICellSelectedHandler>(h => h?.HandleCellSelected(this));
	}
}
