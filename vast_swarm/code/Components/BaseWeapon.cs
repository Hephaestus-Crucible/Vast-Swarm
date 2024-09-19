using Input = Sandbox.Input;

class BaseWeapon : Component
{
	[Property] public GameObject Entity { get; set; }

	protected override void OnUpdate()
	{
		if ( IsProxy )                  //checks if it is a network object owned by another client
		{
			return;
		}

		var pc = Components.GetInAncestors<VastSwarmPlayer>(); //need to get Jeans Character Controller for here
		if ( pc != null )
			return;

		var lookDir = pc.EyeAngles.ToRotation();

		if ( Input.Pressed( "Attack1" ) )
		{
			var pos = Transform.Position + Vector3.Up * 40.0f + lookDir.Forward.WithZ( 0.0f ) * 50.0f;

			var o = Entity.Clone( pos );
			o.Enabled = true;

			var p = o.Components.Get<Rigidbody>();
			p.Velocity = lookDir.Forward * 500.0f + Vector3.Up * 540.0f;

			o.NetworkSpawn();
		}
	}
}

