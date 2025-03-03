using Sandbox;
using System.Collections.ObjectModel;

public partial class EnemySpawner : Component
{
    [Property] public int MaxEnemies { get; set; } = 10; //Max enemies allowed will be 10 
	[Property] public float SpawnInterval { get; set; } = 5.0f; //Time between enemies spawning in
    [Property] public List<Vector3> SpawnPoints { get; set; } = new(); //List of spawn locations

	private TimeSince _timeSinceLastSpawn;
	private List<Enemy> _activeEnemies = new();

	public override void OnStart()
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
		_activeEnemies.RemoveAll( equals => !equals.isValid() );
	}

	public void SpawnEnemy()
	{
		if ( SpawnPoints.Count == 0 ) return;

		//Picking a random spawn point
		Vector3 spawnPos = SpawnPoints[Game.Random.Int( 0, SpawnPoints.Count - 1 )];

		//Create an enemy instance
		var enemy = new GameObject();
		enemy.Components.Create<Enemy>(); //Attack the Enemy Component
		enemy.WorldPosition = spawnPos; //Setting the spawn position

		_activeEnemies.Add( enemy.Components.Get<Enemy>()); //Track the enemy
	}



	

}
