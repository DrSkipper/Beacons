using UnityEngine;
using System.Collections;

public class LevelVisualizer : VoBehavior
{
	private bool _once;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		//TODO - fcole - This is temp execution logic
		if (!_once)
		{
			_once = true;

			LevelGenerator generator = this.gameObject.GetComponent<LevelGenerator>();
			uint[,] map = generator.map.map;

			Sprite greenSprite = Resources.Load<Sprite>("Sprites/green_square");
			Sprite redSprite = Resources.Load<Sprite>("Sprites/red_square");

			int width = map.GetLength(0);
			int height = map.GetLength(1);
			int halfWidth = width / 2;
			int halfHeight = height / 2;
			int texWidth = greenSprite.texture.width;
			int texHeight = greenSprite.texture.height;

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					GameObject go = new GameObject();
					SpriteRenderer sprite = go.AddComponent<SpriteRenderer>();
					sprite.sprite = map[x, y] == LevelGenerator.TILE_TYPE_RED ? redSprite : greenSprite;
					Bounds bounds = sprite.bounds;
					go.transform.position = new Vector3((x - halfWidth) * bounds.size.x, (y - halfHeight) * bounds.size.y, 0);
					//Vector3 screenPoint = new Vector3(Camera.main.(x - halfWidth) * texWidth, (y - halfHeight) * texHeight, 0);
					//go.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
				}
			}
		}
	}
}
