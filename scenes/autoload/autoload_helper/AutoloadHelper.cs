using System;
using Godot;

namespace TicTacToeMultiplayer.scenes.autoload.autoload_helper;

public partial class AutoloadHelper : Node
{
	private static AutoloadHelper? _instance;

	public override void _Ready()
	{
		_instance = this;
	}

	public static T GetAutoload<T>() where T : Node
	{
		return Instance.GetNode<T>($"/root/{typeof(T).Name}");
	}

	private static AutoloadHelper Instance
	{
		get { return _instance ?? throw new NullReferenceException("AutoloadHelper is not loaded"); }
	}
}
