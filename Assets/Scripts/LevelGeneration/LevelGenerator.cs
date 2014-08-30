using UnityEngine;
using System.Collections;

public class LevelGenerator : VoBehavior
{
	const uint TILE_TYPE_RED 	 = 0x000002;
	const uint TILE_TYPE_BLUE 	 = 0x000004;
	const uint TILE_TYPE_YELLOW  = 0x000008;
	const uint TILE_TYPE_GREEN 	 = 0x000010;
	
	const int MAP_SIZE_X = 50;
	const int MAP_SIZE_Y = 50;

	public LevelGenMap map;

	// Use this for initialization
	void Start()
	{
		this.map = new LevelGenMap(MAP_SIZE_X, MAP_SIZE_Y);
		
		map.printMap();
		map.randomlyConvertTiles(LevelGenMap.TILE_TYPE_DEFAULT, LevelGenerator.TILE_TYPE_RED, 0.65f);
		map.printMap();
		map.runAutomataStep(LevelGenMap.TILE_TYPE_DEFAULT, LevelGenerator.TILE_TYPE_RED, 7, 4, true, false);
		map.printMap();
	}
	
	// Update is called once per frame
	void Update()
	{

	}
}
