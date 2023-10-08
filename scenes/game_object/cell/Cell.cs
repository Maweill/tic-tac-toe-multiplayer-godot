using System.Threading.Tasks;
using Godot;
using Godot.DependencyInjection.Attributes;
using JetBrains.Annotations;
using TicTacToeMultiplayer.scenes.controller.multiplayer_controller;
using TicTacToeMultiplayer.scripts.cell;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.cell;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.game_object.cell;

public partial class Cell : Node2D
{
	[Export]
	private CellBackground _background = null!;
	[Export]
	private CellSprite _sprite = null!;
	[Export]
	private Area2D _clickableArea = null!;
	[Export]
	private AnimationPlayer _animationPlayer = null!;
	
	private MultiplayerController _multiplayerController = null!;
	
	public CellType CellType { get; private set; } = CellType.Empty;
	public PlayerModel? Player { get; private set; }

	public void Select(PlayerModel player)
	{
		_sprite.SetTexture(player.Side);
		CellType = player.Side;
		Player = player;
		_animationPlayer.Play("select");
	}

	public void SetInput(bool active)
	{
		_clickableArea.InputPickable = active;
		_background.SetHovered(false);
		Input.SetDefaultCursorShape();
		
	}

	public async Task MarkAsWin()
	{
		_animationPlayer.Play("win");
		await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
	}

	[Inject] [UsedImplicitly]
	public void Construct(MultiplayerController multiplayerController)
	{
		_multiplayerController = multiplayerController;
	}

	public override void _Ready()
	{
		_clickableArea.InputEvent += OnClickableAreaInputEvent;
		_clickableArea.MouseEntered += OnClickableAreaMouseEntered;
		_clickableArea.MouseExited += OnClickableAreaMouseExited;
	}

	private void OnClickableAreaMouseEntered()
	{
		if (CellType != CellType.Empty) {
			return;
		}
		_sprite.SetTexture(_multiplayerController.Player.Side, true);
		_background.SetHovered(true);
		Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
	}

	private void OnClickableAreaMouseExited()
	{
		if (CellType != CellType.Empty) {
			return;
		}
		_sprite.SetTexture(CellType.Empty);
		Input.SetDefaultCursorShape();
		_background.SetHovered(false);
	}

	private void OnClickableAreaInputEvent(Node viewport, InputEvent @event, long shapeidx)
	{
		if (!@event.IsActionPressed("select_cell")) {
			return;
		}
		SetInput(false);
		Rpc(nameof(OnCellClicked));
	}

	[Rpc(CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	private void OnCellClicked()
	{
		GD.Print("Cell clicked");
		EventBus.RaiseEvent<ICellSelectedHandler>(h => h?.HandleCellSelected(this));
	}
}
