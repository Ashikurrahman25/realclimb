﻿public static class FloatExtensions
{
	public static float ReMap(this float x, float x1, float x2, float y1, float y2)
	{
		var m = (y2 - y1) / (x2 - x1);
		var
			c = y1 - m * x1; // point of interest: c is also equal to y2 - m * x2, though float math might lead to slightly different results.

		return m * x + c;
	}

	public static bool WithinBounds(this float x, float min, float max)
	{
		return x >= min && x <= max;
	}
}