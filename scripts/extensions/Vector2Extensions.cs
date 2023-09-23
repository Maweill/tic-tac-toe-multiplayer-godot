using System;
using Godot;

namespace Tetramino.scripts.Extensions;

public static class Vector2Extensions
{
	public static Vector2 RotatedDegreesFixed(this Vector2 point, float angle)
	{
		angle = (float) (Math.Round(angle) % 360);
		switch (angle) {
			case 0:
				return point;
			case 90:
				return new Vector2(-point.Y, point.X);
			case 180:
				return new Vector2(-point.X, -point.Y);
			case 270:
				return new Vector2(point.Y, -point.X);
			default:
				throw new ArgumentException($"Invalid rotation angle: {angle}. Only 0, 90, 180 and 270 are supported.");
		}
	}
}
