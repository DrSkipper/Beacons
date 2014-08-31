using UnityEngine;
using System.Collections;

public class WorldVisualizer : VoBehavior
{
	public float updateStepLength = 0.2f;
	public int generationFramesPerUpdate = 2;
	public bool runOnStartup = false;

	// Use this for initialization
	void Start()
	{
		_generator = this.gameObject.GetComponent<WorldGenerator>();
		_generator.updateDelegate += this.mapWasUpdated;
		if (this.runOnStartup)
			this.restart();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (_running)
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
		else if (Input.GetKeyUp(KeyCode.Space))
		{
			this.restart();
		}
	}

	public void mapWasUpdated()
	{
		_mapWasUpdated = true;
		if (_generator.generationComplete)
		{
			_generationComplete = true;
			_running = false;
		}
	}

	public void animationTimerCallback()
	{
		_animationTimer.paused = true;
		_animatingLastUpdate = false;
	}

	public void restart()
	{
		_generator.clearMap();
		setupMapDisplay();
		if (_animationTimer != null)
			_animationTimer.invalidate();
		_animationTimer = new Timer(this.updateStepLength, true, false, this.animationTimerCallback);
		_animatingLastUpdate = false;
		_mapWasUpdated = false;
		_generationComplete = false;
		_running = true;
	}

	/**
	 * Private
	 */
	private bool _running;
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
		if (_tileRenderers != null)
		{
			for (int x = 0; x < _tileRenderers.GetLength(0); ++x)
			{
				for (int y = 0; y < _tileRenderers.GetLength(1); ++y)
				{
					Destroy(_tileRenderers[x, y].gameObject);
				}
			}
		}

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
