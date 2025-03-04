using Sandbox;
using System.Collections.Generic;

public partial class EnemySpawner : Component
{
    [Property] public int MaxEnemies { get; set; } = 10; //Max enemies allowed will be 10 
	[Property] public float SpawnInterval { get; set; } = 5.0f; //Time between enemies spawning in
    [Property] public List<Vector3> SpawnPoints { get; set; } = new(); //List of spawn locations

	private TimeSince _timeSinceLastSpawn;
	private List<Enemy> _activeEnemies = new();

	protected override void OnStart()
	{
		_timeSinceLastSpawn = 0;
	}

	protected override void OnUpdate()
	{
		//Checking if we can spawn a new enemy
		if(_timeSinceLastSpawn > SpawnInterval && _activeEnemies.Count < MaxEnemies)
		{
			SpawnEnemy();
			_timeSinceLastSpawn = 0;
		}

		//Removing any dead enemies
		_activeEnemies.RemoveAll( e => e.GameObject == null || !e.GameObject.IsValid );
	}

	private void SpawnEnemy()
	{
		if ( SpawnPoints.Count == 0 ) return;

		Vector3 spawnPos = SpawnPoints[Game.Random.Int( 0, SpawnPoints.Count - 1 )];

		// Create a new GameObject in the scene
		var enemyObject = new GameObject();
		enemyObject.WorldPosition = spawnPos; // Set the spawn position

		// Attach the Enemy component to the GameObject
		var enemyComponent = enemyObject.Components.Create<Enemy>();

		if ( enemyComponent != null )
		{
			_activeEnemies.Add( enemyComponent );
		}
	}








}
