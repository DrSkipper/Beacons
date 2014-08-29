using UnityEngine;
using System.Collections;

//NOTE - fcole - Cellular Automata level generation code heavily inspired by this link:
// http://gamedev.tutsplus.com/tutorials/implementation/cave-levels-cellular-automata/
public class LevelGenMap : VoBehavior {

	const uint TILE_TYPE_DEFAULT = 0x000000;
	const uint TILE_TYPE_RED 	 = 0x000001;
	const uint TILE_TYPE_BLUE 	 = 0x000002;
	const uint TILE_TYPE_YELLOW  = 0x000003;
	const uint TILE_TYPE_GREEN 	 = 0x000004;
	const uint TILE_TYPE_INVALID = 0xFFFFFF;

	const int MAP_SIZE_X = 50;
	const int MAP_SIZE_Y = 50;

	public uint[,] map;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void generateMap()
	{
		this.map = new uint[MAP_SIZE_X, MAP_SIZE_Y];


	}

	uint tileTypeAtLocation(int x, int y, bool allowWrapping = false)
	{
		if (x >= 0 && x < MAP_SIZE_X && 
		    y >= 0 && y < MAP_SIZE_Y)
			return map[x, y];

		if (allowWrapping)
		{
			if 		(x < 0) 		  x += MAP_SIZE_X;
			else if (x >= MAP_SIZE_X) x -= MAP_SIZE_X;
			if 		(y < 0) 		  y += MAP_SIZE_Y;
			else if (y >= MAP_SIZE_Y) y -= MAP_SIZE_Y;

			return this.tileTypeAtLocation(x, y, false);
		}

		return TILE_TYPE_INVALID;
	}

	//TODO - fcole - Change from chance to have guaranteed percentage of tiles to convert, maybe with leeway parameter.
	//   Will probably require changing to array of tile objects, getting lists of them and shuffling the lists.
	void randomlyConvertTiles(uint validBaseTypesMask, uint typeToConvertTo, float chance)
	{
		for (int x = 0; x < MAP_SIZE_X; ++x)
		{
			for (int y = 0; y < MAP_SIZE_Y; ++y)
			{
				// If the type at this location matches a valid type, run a die roll to convert it
				if ((map[x, y] | validBaseTypesMask) == validBaseTypesMask && Random.value <= chance)
					map[x, y] = typeToConvertTo;
			}
		}
	}

	void runAutomataStep(uint baseType, uint checkType, int limitForTileDeath, int limitForTileBirth)
	{
		uint[,] newMap = new uint[MAP_SIZE_X, MAP_SIZE_Y];

		for (int x = 0; x < MAP_SIZE_X; ++x)
		{
			for (int y = 0; y < MAP_SIZE_Y; ++y)
			{
				uint oldType = map[x, y];

				bool matchesBaseType = (oldType | baseType) == baseType;
				bool matchesCheckType = (oldType | checkType) == checkType;

				if (matchesBaseType || matchesCheckType)
				{
					uint newType = oldType;
					int count = neighborTypeCount(x, y, checkType);

					if (matchesBaseType)
						newType = count < limitForTileDeath ? checkType : baseType;
					else // if (matchesCheckType)
						newType = count > limitForTileBirth ? baseType : checkType;

					newMap[x, y] = newType;
				}
			}
		}

		this.map = newMap;
	}

	int neighborTypeCount(int centerX, int centerY, uint searchType, bool wrapMap = true, bool countInvalids = false)
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
