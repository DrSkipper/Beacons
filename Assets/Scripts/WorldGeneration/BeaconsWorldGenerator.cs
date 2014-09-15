using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeaconsWorldGenerator : WorldGenerator
{
	public int areaTypes = 5;
	public int overlayTypes = 1;

	public float areaInitialConversionRate = 0.65f;
	public int areaStepIterations = 30;
	public int areaDeathLimit = 7;
	public int areaBirthLimit = 4;

	public float overlayInitialConversionRate = 0.5f;
	public int overlayStepIterations = 20;
	public int overlayDeathLimit = 7;
	public int overlayBirthLimit = 4;

	private const int NUM_STAGES = 5;
	private delegate void RunStageDelegate(int stageIndex, int frames);

	void Start() { }
	void Update() { }
	
	public override void runGenerationFrames(int frames)
	{
		if (_stagesCompleted[_currentStage])
			++_currentStage;

		if (_currentStage >= NUM_STAGES)
		{
			this.generationComplete = true;
		}
		else
		{
			_stageDelegates[_currentStage](_currentStage, frames);
			if (_stagesCompleted[_currentStage])
			{
				++_currentStage;

				if (_currentStage >= NUM_STAGES)
					this.generationComplete = true;
			}
		}
		
		if (this.updateDelegate != null)
		{
			this.updateDelegate();
		}
	}

	public override void clearMap()
	{
		base.clearMap();
		_islands = new List<WorldGenTile>[areaTypes];
		_stagesCompleted = new bool[NUM_STAGES];
		_framesRunByStage = new int[NUM_STAGES];
		_currentStage = 0;

		if (_stageDelegates == null)
		{
			_stageDelegates = new RunStageDelegate[] {
				randomConversionStage, 
				areaCellularAutomataStage, 
				islandAssignmentStage, 
				islandExpansionStage, 
				overlayCellularAutomataStage
			};
		}
	}

	/**
	 * Private
	 */
	private List<WorldGenTile>[] _islands;
	private bool[] _stagesCompleted;
	private int[] _framesRunByStage;
	private int _currentStage;
	private RunStageDelegate[] _stageDelegates;

	private void randomConversionStage(int stageIndex, int frames)
	{
		map.randomlyConvertTiles(WorldGenMap.TILE_TYPE_DEFAULT, WorldGenerator.TILE_TYPE_A, this.areaInitialConversionRate);
		_framesRunByStage[stageIndex] += 1;
		_stagesCompleted[stageIndex] = true;
	}

	// Returns frames run
	private void areaCellularAutomataStage(int stageIndex, int frames)
	{
		int startingIteration = _framesRunByStage[stageIndex];
		int iterations = startingIteration + frames;

		if (iterations >= this.areaStepIterations)
		{
			iterations = areaStepIterations;
			_stagesCompleted[stageIndex] = true;
		}
		
		for (int i = startingIteration; i < iterations; ++i)
			map.runAutomataStep(WorldGenMap.TILE_TYPE_DEFAULT, WorldGenerator.TILE_TYPE_A, this.areaDeathLimit, this.areaBirthLimit, true, false);

		_framesRunByStage[stageIndex] = iterations;
	}

	// Returns frames run
	private void islandAssignmentStage(int stageIndex, int frames)
	{
//		if (_islands == null)
//			_islands = new List<WorldGenTile>[this.areaTypes];
//
//		uint[] types = new uint[] {TILE_TYPE_A, TILE_TYPE_B, TILE_TYPE_C, TILE_TYPE_D, TILE_TYPE_E, TILE_TYPE_F};
//
//		int nextIslandType = _islands.Length;
//		int i = 0;
//		for (; i < frames; ++i)
//		{
//			if (_islands.Length >= areaTypes || _islands.Length >= types.Length)
//				break;
//		}
//
//		return frames - i;
		//TODO - fcole - If num islands < areaTypes / 2 + 1 then clear map and set us back to stage 1 (or use some other "good enough" tolerance)
	}
	
	private void islandExpansionStage(int stageIndex, int frames)
	{
	}
	
	private void overlayCellularAutomataStage(int stageIndex, int frames)
	{
	}
	
	/*
	 * Returns list of tiles with same time as rootTile within reach of flood fill traversal
	 * (returns an "island" that includes rootTile)
	 * 
	 * http://en.wikipedia.org/wiki/Flood_fill
	 * 
	 * Flood-fill (node, target-color, replacement-color):
	 * 1. If target-color is equal to replacement-color, return.
	 * 2. Set Q to the empty queue.
	 * 3. Add node to the end of Q.
	 * 4. While Q is not empty: 
	 * 5.     Set n equal to the last element of Q.
	 * 6.     Remove last element from Q.
	 * 7.     If the color of n is equal to target-color:
	 * 8.         Set the color of n to replacement-color and mark "n" as processed.
	 * 9.         Add west node to end of Q if west has not been processed yet.
	 * 10.        Add east node to end of Q if east has not been processed yet.
	 * 11.        Add north node to end of Q if north has not been processed yet.
	 * 12.        Add south node to end of Q if south has not been processed yet.
	 * 13. Return.
 	 */
	private List<WorldGenTile> floodFill(WorldGenTile rootTile, bool allowWrapping)
	{
		this.map.clearProcessedFlags();

		List<WorldGenTile> reachedTiles = new List<WorldGenTile>();
		List<WorldGenTile> stack = new List<WorldGenTile>();
		uint checkType = rootTile.type;
		processIntoStack(stack, rootTile);

		while (reachedTiles.Count > 0)
		{
			WorldGenTile currentTile = stack[0];
			stack.RemoveAt(0);

			//if ((currentTile.type & checkType) != 0x000000) // More lenient/inclusive check
			if (currentTile.type == checkType) // Exact equality
			{
				reachedTiles.Add(currentTile);
				processIntoStack(stack, this.map.tileAtLocation(currentTile.x - 1, currentTile.y, allowWrapping));
				processIntoStack(stack, this.map.tileAtLocation(currentTile.x + 1, currentTile.y, allowWrapping));
				processIntoStack(stack, this.map.tileAtLocation(currentTile.x, currentTile.y - 1, allowWrapping));
				processIntoStack(stack, this.map.tileAtLocation(currentTile.x, currentTile.y + 1, allowWrapping));
			}
		}

		return reachedTiles;
	}

	private void processIntoStack(List<WorldGenTile> stack, WorldGenTile tile)
	{
		if (tile != null && !tile.processed)
		{
			stack.Insert(0, tile);
			tile.processed = true;
		}
	}
}
