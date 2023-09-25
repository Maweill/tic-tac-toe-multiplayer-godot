using System.Collections.Generic;
using Godot;

namespace TicTacToeMultiplayer.scripts.extensions;

public static class ListExtensions
{
	public static T PickRandom<T>(this List<T> list)
	{
		if (list.Count == 0) {
			throw new System.Exception("List is empty");
		}
		return list[GD.RandRange(0, list.Count - 1)];
	}
}
