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

			//LevelGenerator generator = this.gameObject.GetComponent<LevelGenerator>();
			//uint[,] map = generator.map.map;
			//
			//for (int x = 0; x < map

			GameObject testObject = new GameObject();
			SpriteRenderer testSprite = testObject.AddComponent<SpriteRenderer>();
			testSprite.sprite = Resources.Load<Sprite>("Sprites/green_square");
			//testObject.transform.position = transform.position;
		}
	}
}
