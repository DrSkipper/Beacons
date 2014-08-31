using UnityEngine;
using System.Collections;

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

		if (framesRemaining > 0 && !stageHit)
		{
			runSecondStage(startFrame, framesRemaining);
			framesRemaining = 0;
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

	private void runSecondStage(int startFrame, int frames)
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
	}
}
