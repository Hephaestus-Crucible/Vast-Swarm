using Sandbox;

public sealed class Enemy : Component
{
	[Property] public float Health { get; set; } = 100.0f;
	private Rigidbody _rigidbody;
	private GameObject _player;
	private float _speed = 200f; // Movement speed

	protected override void OnStart()
	{
		// Assign a Model Renderer
		var model = Components.Create<ModelRenderer>();
		model.Model = Model.Load( "models/citizen/citizen.vmdl" );

		// Add Rigidbody for physics
		_rigidbody = Components.Create<Rigidbody>();
		_rigidbody.Gravity = true; // ✅ Enable gravity
		_rigidbody.LinearDamping = 0.1f; // ✅ Reduce air drag
		_rigidbody.AngularDamping = 0.1f; // ✅ Prevent spinning

		// Add a CapsuleCollider
		var collider = Components.Create<CapsuleCollider>();
		collider.Radius = 20f;
		collider.Start = new Vector3( 0, 0, 0 ); // ✅ Correct way to define capsule
		collider.End = new Vector3( 0, 0, 60 ); // ✅ Set end height

		// Find the player GameObject
		_player = Scene.GetAllComponents<VastSwarmPlayer>().FirstOrDefault()?.GameObject;
	}

	protected override void OnUpdate()
	{
		if ( _player == null ) return;

		// Check if the enemy is on the ground using a downward raycast
		bool isGrounded = Scene.Trace.Ray( GameObject.WorldPosition, GameObject.WorldPosition - new Vector3( 0, 0, 5 ) )
			.Run()
			.Hit;

		if ( !isGrounded ) return; // ✅ Ensures movement only when grounded

		// Get direction to the player
		Vector3 direction = (_player.WorldPosition - GameObject.WorldPosition).Normal;

		// Apply force toward the player
		_rigidbody.ApplyForce( direction * _speed * Time.Delta );

		// Limit speed to prevent excessive movement
		float maxSpeed = 300f;
		if ( _rigidbody.Velocity.Length > maxSpeed )
		{
			_rigidbody.Velocity = _rigidbody.Velocity.Normal * maxSpeed;
		}
	}





	public void TakeDamage( float damage )
	{
		Health -= damage;
		if ( Health <= 0 ) Die();
	}

	private void Die()
	{
		GameObject.Destroy();
	}
}
