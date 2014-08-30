using UnityEngine;
using System.Collections;

public class WorldGenTile
{
	public int x;
	public int y;
	public uint type;

	public WorldGenTile(int x, int y, uint type)
	{
		this.x = x;
		this.y = y;
		this.type = type;
	}
}
