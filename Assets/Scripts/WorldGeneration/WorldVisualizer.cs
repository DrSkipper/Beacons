using UnityEngine;
using System.Collections;

public class WorldVisualizer : VoBehavior
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

			WorldGenerator generator = this.gameObject.GetComponent<WorldGenerator>();
			uint[,] map = generator.map.map;

			Sprite spriteA = Resources.Load<Sprite>("Sprites/blue_square");
			Sprite spriteB = Resources.Load<Sprite>("Sprites/yellow_square");

			int width = map.GetLength(0);
			int height = map.GetLength(1);
			int halfWidth = width / 2;
			int halfHeight = height / 2;

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					GameObject go = new GameObject();
					SpriteRenderer sprite = go.AddComponent<SpriteRenderer>();
					sprite.sprite = map[x, y] == WorldGenerator.TILE_TYPE_RED ? spriteA : spriteB;
					Bounds bounds = sprite.bounds;
					go.transform.position = new Vector3((x - halfWidth) * bounds.size.x, (y - halfHeight) * bounds.size.y, 0);
				}
			}
		}
	}
}
