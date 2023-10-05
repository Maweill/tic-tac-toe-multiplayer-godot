using System;
using Godot;
using TicTacToeMultiplayer.scripts.models;

namespace TicTacToeMultiplayer.scenes.autoload.models_container;

//TODO Заменить на DI
public partial class ModelsContainer : Node
{
	private static ModelsContainer? _instance;

	public static MultiplayerModel MultiplayerModel { get; private set; } = null!;

	public override void _Ready()
	{
		MultiplayerModel = new MultiplayerModel();
	}
	
	private static ModelsContainer Instance
	{
		get { return _instance ?? throw new NullReferenceException("ModelsContainer is not loaded"); }
	}
}
