using Input = Sandbox.Input;

public sealed class BaseWeapon : Component        //BaseWeapon is a child of Component
{
	[Property] public GameObject Projectile { get; set; }
	//Use [RequireComponent] when we have the final descision on what the projectile model will be maybe.

	/*The [Property] version provides flexibility and manual assignment through the editor.The [RequireComponent] version guarantees the
	 * existence of the required component on the game object, 
	 * making the code more robust and reducing the risk of runtime errors.*/
	//Property provides a reference to GameObject that the GameObject is attached,allowing the component to modify GameObject properties
	//[Property] public GameObject BaseWeapon { get; set; }
	//Property exposes a property for the model to be editable in the editor? working on this.

	
	[Property] ModelRenderer BaseWeaponModel { get; set; }	
	protected override void OnUpdate()
	{
		if ( IsProxy )                  //Checks if it is a network object owned by another client
		{
			return;
		}

		var pc = Components.GetInAncestors<VastSwarmPlayer>(); 
		//Assigns pc to type VastSwarmPlayer.
		//pc now refers to the VastSwarmPlayerComponent
		//var means if GetInAncestors cannot find VastSwarmPlayer component, pc will be null.

		/*Use: You have an entity hierarchy where some parts of the player (like weapons, clothing, or attachments) 
		 * are separate entities, but you need access to the VastSwarmPlayer component for player-specific logic, 
		 * this method will allow you to find it.*/

		if ( pc != null )
			return; /*If VastSwarmPlayer is found, this essentially cancels a pointless ongoing search.
			       	 Safety Mechanism, protects future overwriting of the component in this instance*/

		var lookDir = pc.EyeAngles.ToRotation();  /*EyeAngles represents where the player is looking in 3Space.
		lookDir stores the vector result of ToRoation*/

		if ( Input.Pressed( "Attack1" ) )   //If Attack1 bound key is pressed.
		{
			Log.Info( "Log: Shot Fired" );
			var ProjectileSpawnPos = Transform.Position + Vector3.Up * 40.0f + lookDir.Forward.WithZ( 0.0f ) * 50.0f;
			/*Use: Calculates a position where the projectile will spawn when the player fires.
				- Transform.Position is the current position of the Player.
				- Vector3.Up is a unit vector pointing 1 unit up, 40 multiplies it upward to around head height
					40 units above the origin.
				- lookDir.Forward.WithZ( 0.0f ) * 50.0f: 50 units infront of the players look direction on XY only.
				- Combined: Projectile will spawn 40 units up and 50 units in front of the player.
							A Rough estimate at where a barrel would be as of now.*/

			var ProjectileCopy = Projectile.Clone( ProjectileSpawnPos );
			/*Clone dulicates Projectile at ProjectileSpawnPos.*/

			ProjectileCopy.Enabled = true;
			/*Enabled method activated the Projectile,becoming active and behaving according to its defined logic.*/

			var ProjectilePhysics = ProjectileCopy.Components.Get<Rigidbody>();
			/*Retrieves the rigid body(physics component) that allows for the entity to interact with the engine.*/

			ProjectilePhysics.Velocity = lookDir.Forward * 500.0f + Vector3.Up * 540.0f;
			/*Use: Calculates the Ballistics of the projectile
				- ProjectilePhysics.Velocity sets speed and direction of the Projectile Entity in the world.
				- loodDir *500 sets the forward velocity of the Projectile to 500.
				- Vector3.Up unit vector multipled by 540 means the shot moves slightly upward simulated an arc
				- Combining the Forwared and Upward components simulates slight lift from something like recoil.*/

			ProjectileCopy.NetworkSpawn();
			/*Spawns on the network if possible.*/
		}
	}
	
}
