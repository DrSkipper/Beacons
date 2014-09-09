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

	private const int FIRST_STAGE_FRAMES = 1;

	void Start() { }
	void Update() { }
	
	public override void runGenerationFrames(int frames)
	{
		int framesRemaining = frames;
		int startFrame = _frames;
		bool stageHit = false;

		// Random fill
		if (_frames < FIRST_STAGE_FRAMES)
		{
			stageHit = true;
			int stageFramesRemaining = FIRST_STAGE_FRAMES - startFrame;
			bool willCompleteStage = stageFramesRemaining <= framesRemaining;

			if (willCompleteStage)
			{
				runFirstStage(startFrame, stageFramesRemaining);
				framesRemaining -= stageFramesRemaining;
				startFrame += stageFramesRemaining;
			}
			else
			{
				runFirstStage(startFrame, framesRemaining);
				framesRemaining -= frames; // go to 0
			}
		}

		// CA
		if (framesRemaining > 0 && !stageHit)
		{
			stageHit = true;
			int framesRun = runSecondStage(startFrame, framesRemaining);
			framesRemaining -= framesRun;
		}

		// Detect islands
		if (framesRemaining > 0 && !stageHit)
		{
			stageHit = true;
			int framesRun = runThirdStage(startFrame, framesRemaining);
			framesRemaining -= framesRun;

			//TODO - fcole - If num islands < areaTypes / 2 + 1 then clear map and set us back to stage 1 (or use some other "good enough" tolerance)

		}

		// Expand islands
		if (framesRemaining > 0 && !stageHit)
		{
			stageHit = true;
			int framesRun = runFourthStage(startFrame, framesRemaining);
			framesRemaining -= framesRun;
		}

		// Generate overlay map
		if (framesRemaining > 0 && !stageHit)
		{
			stageHit = true;
			int framesRun = runFifthStage(startFrame, framesRemaining);
			framesRemaining -= framesRun;
		}

		_frames += frames - framesRemaining;

		if (this.updateDelegate != null)
		{
			this.updateDelegate();
		}
	}

	/**
	 * Private
	 */
	private void runFirstStage(int startFrame, int frames)
	{
		map.randomlyConvertTiles(WorldGenMap.TILE_TYPE_DEFAULT, WorldGenerator.TILE_TYPE_A, this.areaInitialConversionRate);
	}

	// Returns frames run
	private int runSecondStage(int startFrame, int frames)
	{
		int startingIteration = startFrame - FIRST_STAGE_FRAMES;
		int iterations = startingIteration + frames;

		if (iterations >= this.areaStepIterations)
		{
			iterations = areaStepIterations;
			this.generationComplete = true;
		}
		
		for (int i = startingIteration; i < iterations; ++i)
			map.runAutomataStep(WorldGenMap.TILE_TYPE_DEFAULT, WorldGenerator.TILE_TYPE_A, this.areaDeathLimit, this.areaBirthLimit, true, false);

		return iterations - startingIteration;
	}

	// Returns frames run
	private int runThirdStage(int startFrame, int frames)
	{
		return 0;
	}
	
	private int runFourthStage(int startFrame, int frames)
	{
		return 0;
	}
	
	private int runFifthStage(int startFrame, int frames)
	{
		return 0;
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
