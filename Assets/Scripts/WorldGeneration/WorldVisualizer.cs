using UnityEngine;
using System.Collections;

public class WorldVisualizer : VoBehavior
{
	public float updateStepLength = 0.2f;
	public int generationFramesPerUpdate = 2;

	// Use this for initialization
	void Start()
	{
		_generator = this.gameObject.GetComponent<WorldGenerator>();
		_generator.updateDelegate += this.mapWasUpdated;
		_generator.clearMap();
		setupMapDisplay();
		_animationTimer = new Timer(this.updateStepLength, true, false, this.animationTimerCallback);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (_animatingLastUpdate)
		{
			updateAnimation();
		}
		else if (_mapWasUpdated)
		{
			_mapWasUpdated = false;
			startAnimation();
		}
		else if (!_generationComplete)
		{
			_generator.runGenerationFrames(this.generationFramesPerUpdate);
		}
	}

	public void mapWasUpdated()
	{
		_mapWasUpdated = true;
		if (_generator.generationComplete)
			_generationComplete = true;
	}

	public void animationTimerCallback()
	{
		_animationTimer.paused = true;
		_animatingLastUpdate = false;
	}

	/**
	 * Private
	 */
	private bool _mapWasUpdated;
	private bool _animatingLastUpdate;
	private bool _generationComplete;
	private WorldGenerator _generator;

	private Sprite _spriteA;
	private Sprite _spriteB;
	private SpriteRenderer[,] _tileRenderers;
	private Timer _animationTimer;

	private void startAnimation()
	{
		_animatingLastUpdate = true;
		_animationTimer.paused = false;

		WorldGenTile[,] map = _generator.map.map;
		
		int width = map.GetLength(0);
		int height = map.GetLength(1);
		
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				_tileRenderers[x, y].sprite = spriteForType(map[x, y].type);
			}
		}
	}

	private void updateAnimation()
	{
		_animationTimer.update();
	}

	private void setupMapDisplay()
	{
		WorldGenTile[,] map = _generator.map.map;

		_spriteA = Resources.Load<Sprite>("Sprites/blue_square");
		_spriteB = Resources.Load<Sprite>("Sprites/yellow_square");
		
		int width = map.GetLength(0);
		int height = map.GetLength(1);
		int halfWidth = width / 2;
		int halfHeight = height / 2;

		_tileRenderers = new SpriteRenderer[width, height];
		
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				GameObject go = new GameObject();
				SpriteRenderer sprite = go.AddComponent<SpriteRenderer>();
				sprite.sprite = spriteForType(map[x, y].type);
				Bounds bounds = sprite.bounds;
				go.transform.position = new Vector3((x - halfWidth) * bounds.size.x, (y - halfHeight) * bounds.size.y, 0);
				_tileRenderers[x, y] = sprite;
			}
		}
	}

	private Sprite spriteForType(uint type)
	{
		return type == WorldGenerator.TILE_TYPE_A ? _spriteA : _spriteB;
	}
}
