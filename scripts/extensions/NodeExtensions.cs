using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tetramino.scripts.Extensions;

public static class NodeExtensions
{
	public static IEnumerable<T> GetChildrenOfType<T>(this Node node) where T : Node
	{
		return node.GetChildren().OfType<T>();
	}
	
	public static T RequireChildOfType<T>(this Node node) where T : Node
	{
		T? child = node.GetChildrenOfType<T>().FirstOrDefault();
		if (child == null) {
			throw new System.Exception($"Node does not have a child of such type. node={node.Name}, type={typeof(T).Name}");
		}
		return child;
	}
}
