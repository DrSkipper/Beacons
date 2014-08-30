﻿using UnityEngine;
using System.Collections;

public class WorldGenerator : VoBehavior
{
	public const uint TILE_TYPE_RED 	 = 0x000002;
	public const uint TILE_TYPE_BLUE 	 = 0x000004;
	public const uint TILE_TYPE_YELLOW  = 0x000008;
	public const uint TILE_TYPE_GREEN 	 = 0x000010;
	
	const int MAP_SIZE_X = 50;
	const int MAP_SIZE_Y = 50;

	public float initialConversionRate = 0.65f;
	public int stepIterations = 20;
	public int deathLimit = 7;
	public int birthLimit = 4;

	public WorldGenMap map;

	// Use this for initialization
	void Start()
	{
		this.map = new WorldGenMap(MAP_SIZE_X, MAP_SIZE_Y);
		
		//map.printMap();
		map.randomlyConvertTiles(WorldGenMap.TILE_TYPE_DEFAULT, WorldGenerator.TILE_TYPE_RED, this.initialConversionRate);
		//map.printMap();
		for (int i = 0; i < this.stepIterations; ++i)
			map.runAutomataStep(WorldGenMap.TILE_TYPE_DEFAULT, WorldGenerator.TILE_TYPE_RED, this.deathLimit, this.birthLimit, true, false);
		//map.printMap();
	}
	
	// Update is called once per frame
	void Update()
	{

	}
}