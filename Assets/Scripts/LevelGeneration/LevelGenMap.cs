using UnityEngine;
using System.Collections;

//NOTE - fcole - Cellular Automata level generation code heavily inspired by this link:
// http://gamedev.tutsplus.com/tutorials/implementation/cave-levels-cellular-automata/
public class LevelGenMap
{
	public const uint TILE_TYPE_DEFAULT = 0x000001;
	public const uint TILE_TYPE_INVALID = 0xFFFFFF;

	public uint[,] map;

	private int _sizeX;
	private int _sizeY;

	public LevelGenMap(int sizeX, int sizeY, uint defaultType = TILE_TYPE_DEFAULT)
	{
		this.map = new uint[sizeX, sizeX];
		_sizeX = sizeX;
		_sizeY = sizeY;

		for (int x = 0; x < _sizeX; ++x)
		for (int y = 0; y < _sizeY; ++y)
			map[x, y] = defaultType;
	}

	public uint tileTypeAtLocation(int x, int y, bool allowWrapping = false)
	{
		if (x >= 0 && x < _sizeX && 
		    y >= 0 && y < _sizeY)
			return map[x, y];

		if (allowWrapping)
		{
			if 		(x < 0) 	  x += _sizeX;
			else if (x >= _sizeX) x -= _sizeX;
			if 		(y < 0) 	  y += _sizeY;
			else if (y >= _sizeY) y -= _sizeY;

			return this.tileTypeAtLocation(x, y, false);
		}

		return TILE_TYPE_INVALID;
	}

	//TODO - fcole - Change from chance to have guaranteed percentage of tiles to convert, maybe with leeway parameter.
	//   Will probably require changing to array of tile objects, getting lists of them and shuffling the lists.
	public void randomlyConvertTiles(uint validBaseTypesMask, uint typeToConvertTo, float chance)
	{
		for (int x = 0; x < _sizeX; ++x)
		{
			for (int y = 0; y < _sizeY; ++y)
			{
				// If the type at this location matches a valid type, run a die roll to convert it
				if ((map[x, y] | validBaseTypesMask) == validBaseTypesMask && Random.value <= chance)
					map[x, y] = typeToConvertTo;
			}
		}
	}

	public void runAutomataStep(uint baseType, uint checkType, int limitForTileDeath, int limitForTileBirth, bool allowWrapping = true, bool countInvalids = false)
	{
		uint[,] newMap = new uint[_sizeX, _sizeY];

		for (int x = 0; x < _sizeX; ++x)
		{
			for (int y = 0; y < _sizeY; ++y)
			{
				uint oldType = map[x, y];
				uint newType = oldType;
				
				bool matchesBaseType = (oldType | baseType) == baseType;
				bool matchesCheckType = (oldType | checkType) == checkType;

				if (matchesBaseType || matchesCheckType)
				{
					int count = neighborTypeCount(x, y, checkType, allowWrapping, countInvalids);

					if (matchesBaseType)
						newType = count < limitForTileDeath ? checkType : baseType;
					else // if (matchesCheckType)
						newType = count > limitForTileBirth ? baseType : checkType;
				}

				newMap[x, y] = newType;
			}
		}

		this.map = newMap;
	}

	public void printMap()
	{
		string logString = "\n";
		for (int x = 0; x < _sizeX; ++x)
		{
			logString += ".";
			for (int y = 0; y < _sizeY; ++y)
			{
				logString += "" + map[x, y].ToString().PadLeft(2, '0') + ".";
			}
			logString += "\n";
		}

		Debug.Log(logString);
	}

	/**
	 * Private
	 */
	private int neighborTypeCount(int centerX, int centerY, uint searchType, bool wrapMap = true, bool countInvalids = false)
	{
		int count = 0;
		for (int x = centerX - 1; x <= centerX + 1; ++x)
		{
			for (int y = centerY - 1; y <= centerY + 1; ++y)
			{
				// Skip middle
				if (x == centerX && y == centerY)
					continue;

				uint type = this.tileTypeAtLocation(x, y, wrapMap);
				if (type == TILE_TYPE_INVALID)
				{
					if (countInvalids)
						++count;
				}
				else if ((type | searchType) == searchType)
				{
					++count;
				}
			}
		}
		return count;
	}
}
