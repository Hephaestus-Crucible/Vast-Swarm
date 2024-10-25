using Sandbox;
using Sandbox.UI;
using System.Diagnostics;




public class Staff_Of_Might : Weapon
{
	TimeSince timeSinceLastShot = 0.0f;
	//GameObject viewmodel;



	/*
	public override void Attack()
	{
		if ( !PrimaryAutomatic && !Input.Pressed( "Primary Attack" ) )
		{
			return;
		}
		if ( timeSinceLastShot < PrimaryFireRate )
		{
			return;
		}
		//Handles Exceptions

		timeSinceLastShot = 0;

		Vector3 shotPosition = WorldPosition;
		//Sets shotPosition to the position of the weapon component


		//viewmodel is a gameobject created when the game is being played. This assigns vm to the SkinnedModelRenderer component in viewmodel
		if ( viewmodel.Components.TryGet<SkinnedModelRenderer>( out var vm ) )
		{
			vm.Set( "b_attack", true );
			//sets the b_attack parameter to true in the editor and in the animgraph

			var muzzle = vm.GetBoneObject( vm.Model.Bones.GetBone( "muzzle" ) );
			if ( muzzle == null )
			{
				Log.Warning( "Muzzle bone not found. Using default GameObject position." );
				muzzle = vm.GameObject;
			}
			//Retrieves bone named muzzle from model's skeleton and sets it to muzzle, if it isnot found it defaults to using the vm default GameObject

			shotPosition = muzzle.WorldPosition;
			//Assigns shot position to the position of the muzzle bone


			GameObject.Clone( "/Effects/Weapons/muzzle.prefab", global::Transform.Zero, muzzle );
			//This spawn a clone of the prefab in the world and parented to the muzzle bone, spawning a muzzle flash at the muzzle bone in this case

			Sound.Play( PrimaryFireSound, shotPosition );
			//plays the primary fire sound at shotPosition

			FireProjectile();
		}
	}*/



	public override void FireProjectile()
	{
		var ray = Scene.Camera.Transform.World.ForwardRay;
		//Assigns ray to the ForwardRay of the camera in the scene. Generates a ray originatomg from the cameras position pointing in the forward dir.

		ray.Forward += Vector3.Random * 0.01f;
		//Generates a random 3D vector with random values for X,Y,Z ,+ 0.01f constant adjustment that ensures the ray direction is altered by a fixed amount in addition to the randomness 

		var trace = Scene.Trace.Ray( ray, 4096 ).IgnoreGameObjectHierarchy( GameObject.Parent ).Run();
		/*Trace casts a ray through the scene for hit detection. We cast ray 4096 units, IgnoreGameObjectHierarch ignores the parent of the game object,
		 the to avoid having the ray detect the gun istelf, .Run() executes the trace.*/

		//Figure out a way to draw debug trace line for viewing in the editor. (Gizmos and or the library(?)
		//Gizmo.Draw.Line(ray.Position, trace.EndPosition );
		//DrawGizmos.DrawRay();


		if ( !trace.Hit || trace.GameObject is null )
		{
			return;
		}
		//if trace does not hit anything, or the GameObject it hits is null, return.

		GameObject.Clone( "/Effects/Weapons/impact_default.prefab", new Transform( trace.HitPosition + trace.Normal * 2.0f, Rotation.LookAt( trace.Normal ) ) );
		/*Creates a new instance of the specified prefab,new Transform defines the position of the new game object.
		 * trace.normal is the perpendicular normal vector to the surface and scales it by 2 to display at the HitPosition.Rotation.LookAt aligns the impact it hit with the normal
		   vector or in other words the surface that it hit.*/
		{
			var WeaponHit = GameObject.Clone( "/Effects/Weapons/decal_bullet_default.prefab" );
			WeaponHit.Transform.World = new Transform( trace.HitPosition + trace.Normal * 2.0f, Rotation.LookAt( -trace.Normal, Vector3.Random ), System.Random.Shared.Float( 0.8f, 1.2f ) );
			WeaponHit.SetParent( trace.GameObject );
			/*Should find the bullet-metal decal or whatever decal/prefab you want to use from the \\Assets\\decals folder or whichever folder you set your
			 GameOBject clone to. It makes it look more realistic by adding world Transform to make the decal more viewable and whatnot.
			Then attaches the WeaponHit decal to the GameObject that it hit.*/
		}

		if ( trace.Body != null )
		{
			trace.Body.ApplyImpulseAt( trace.HitPosition, trace.Direction * 200.0f * trace.Body.Mass.Clamp( 0, 200 ) );
		}
		/*Applies an impulse force to the physics body at the location where the trace hit, applies the force of 200 in the direction of the trace and
		 * clampm ensures that the impulse produces realistic behavior depending on the mass of opbjects*/


		//Draw Debug Lines
		//Gizmo.Draw.Line( ray );

		var damage = new DamageInfo( 10, GameObject, GameObject, trace.Hitbox );
		damage.Position = trace.HitPosition;
		damage.Shape = trace.Shape;
		/*damage is assigned to a damage event where 1o is the damage amount, and Hitbox defines the area hit.
		  damage.Position = trace.HitPosition sets the position of the hit so the damage knows where it happened.
		  damage.shape = trace.shape  attaches the shape of the hit object tot the damage event.*/


		foreach ( var damageable in trace.GameObject.Components.GetAll<IDamageable>() )
		{
			damageable.OnDamage( damage );
		}
		/*Finds all components on the hit object that can take damage, then calls the damage handling function applying damage to the object.*/

	}

}
