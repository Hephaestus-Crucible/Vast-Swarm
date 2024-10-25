using Sandbox;
using Sandbox.UI;
using System.Diagnostics;

public sealed class Weapon : Component
{
	[Property, Group("View Model")]
	public Model ViewModel { get; set; }
	/*Property holds the 3d model for the weapons view model, can set in the editor to change the appearance.*/


	[Property, Group("View Model")]
	public Vector3 ViewModelOffset { get; set; }
	/*Defines the offset for the view model. Can adjust the pos of the weapon.*/

	[Property, Group( "Primary Fire" )]
	public float PrimaryFireRate { get; set; } = 0.01f;
	/*Defines the time delay between shots*/

	[Property, Group( "Primary Fire" )]
	public bool PrimaryAutomatic { get; set; } = true;
	/*Defines when the primary weapon attack is automatic*/

	[Property, Group( "Primary Fire" )]
	public SoundEvent PrimaryFireSound { get; set; }
	/*USed to assign a specific sound effect in the editor.*/

	[Property]
	public GameObject PrimaryFireVFX_Prefab { get; set; }
	/*Stores weapon firing effect when fired*/

	[Property]
	public GameObject WeaponPrefab { get; set; }
	/*stores the weapon's model*/

	[Property]
	public AnimationGraph GraphOverride { get; set; }
	/*Allows for overriding the weapons animation graph when changiung animations with the weapon*/

	[Property]
	public SkinnedModelRenderer BodyRenderer { get; set; }
	/*Handles animations or blending with skeletal bones*/


	float ironsights;
	/*For tracking how zoomed in the player is with the weapon*/
	GameObject viewmodel;
	/*creates a GameObject called viewmodel*/

	Rotation lastRot;
	//Creates a Rotation Struct called lastRot

	TimeSince timeSinceLastShot = 0.0f;


	protected override void OnUpdate() /*Called every frame, use for real time tasks (visual updates, input)*/
	{
		PositionViewModel();
		AnimationHandler();
		
	}

	protected override void OnFixedUpdate() /*consistent tick, not dependent on framerate, use with physics related tasks*/
	{
		base.OnFixedUpdate();

		if (Input.Down( "Primary Attack" ))
		{
			Attack();
		}
	}

	protected override void OnEnabled()
	{
		//Create the view model for the weapon when switching to it (enables the weapon when switched to)
		base.OnEnabled();
		CreateViewModel();
	}


	protected override void OnDisabled()
	{
		base.OnDisabled();
		viewmodel?.Destroy();
	}


	/*Ensures smooth transitions between weapon animation states.*/
	void AnimationHandler()
	{
		if ( !(viewmodel?.Components.TryGet<SkinnedModelRenderer>(out var vm) ?? false))
		{
			/*prevents NullReferenceException by ennsureing that we only proceed if viewmodel is not null.
			Components.TryGet<> attempts to get a component of type SkinnedModelRenderer from the viewmodel object’s 
			Components collection. If the component exists,it stores it in the vm variable and returns true; if not, it returns false.*/
			/*Ensure that an object or component exists before attempting to use it. 
			 * It prevents runtime errors by safely checking for null values or missing components.*/
			return;
		}

		var VSP = Components.Get <VastSwarmPlayer>( FindMode.InAncestors );
		/*Sets VSP to player component, using the FindMode.InAncestors, which searches for the Player on the current gameObject,
		 as well as any of it's parent objects as well, useful when the component is attached to a parent object.
		 We chose to search for the Player Object because the Player class already wraps the CharacterController and it’s more 
		 efficient.*/

		ironsights = ironsights.LerpTo( Input.Down( "Secondary" ) ? 1 : 0, Time.Delta * 5.0f );
		/*Smoothly transitions between aiming and not aiming, Time.Delta *5 controls the speed, the bigger the f, the faster. */

		vm.Set( "ironsights", ironsights );
		vm.Set( "move_bob", 1 );
		/*Sets paramaters on the SkinnedModelRenderer component, can be used in the animation system, animgraph*/


		var rotationDelta = Rotation.Difference( lastRot, Scene.Camera.WorldRotation );
		/*Detects how much the camera has rotated lince last frame, assigns to rotationDelta
		 Scene.Camera.Transform.Rotation gets the current rotation of the camera in the scene
		 Useful for having the animation reac to camera rotation, for realism.*/

		lastRot = Scene.Camera.WorldRotation;
		/*Update lastRot to the cameras current rotation. Ensures that the next frame it compares the new rotaion to the previous frame rotatilon*/

		var angles = rotationDelta.Angles();
		/*angles holds the pitch, yaw, and roll values that represent how much the camera's rotation has changed since the last frame.
		 struct Rotation.Angles() method converets a quaternion into Euler angles.*/

		vm.Set( "aim_pitch", angles.pitch );
		vm.Set("aim_yaw", angles.yaw );
		/*These lines work together to adjust the weapons rotation to match the camera movement. Pitch: Vertical and Yaw: Horizontas movements
		  Set() method sets a parameter on the SkinnedModelRenderer compoenent*/

		vm.Set( "aim_pitch_inertia", angles.pitch );
		vm.Set( "aim_yaw_inertia", angles.yaw );

		if ( BodyRenderer != null )
		{
			vm.Set( "jump", BodyRenderer.GetBool( "jump" ) );
			vm.Set( "move_groundspeed", BodyRenderer.GetFloat( "move_groundspeed" ) );
			vm.Set( "b_grounded", BodyRenderer.GetFloat( "b_grounded" ) );
			vm.Set( "move_x", BodyRenderer.GetFloat( "move_x" ) );
			vm.Set( "move_y", BodyRenderer.GetFloat( "move_y" ) );
			vm.Set( "move_z", BodyRenderer.GetFloat( "move_z" ) );
		}
		/*The viewmodel uses these parameters to adjust animations and visual effects, making the first-person weapon or object react realistically to 
		 * the player's movement and actions. This ensures that the weapon or object in view behaves naturally when the player is moving, jumping, or 
		 * interacting with the environment.*/
	}

	//Core Weapon Primary Firing Behavior

	void Attack()
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
			/*This spawn a clone of the prefab in the world and parented to the muzzle bone, spawning a muzzle flash at the muzzle bone in this case*/


			//Try to figure out how to print a log message if the muzzle.prefab is not found.
			/*if ( muzzle != null )
			{
			
				
			}*/

			Sound.Play( PrimaryFireSound, shotPosition );
			//plays the primary fire sound at shotPosition

			FireProjectile();
		}
	}

	//Core Weapon Projectile Behavior
	void FireProjectile()
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


		foreach ( var damageable in trace.GameObject.Components.GetAll<IDamageable>())
		{
			damageable.OnDamage( damage );
		}
		/*Finds all components on the hit object that can take damage, then calls the damage handling function applying damage to the object.*/

	}

	private void DrawDebugRay(Vector3 start, Vector3 end)
	{
		Gizmo.Draw.Line(start, end);
	}

	void CreateViewModel()
	{
		viewmodel = new GameObject( true, "viewmodel" );
		
		var modelRender = viewmodel.Components.Create<SkinnedModelRenderer>();
		/* Creates a new SkinnedModelRenderer component and attaches it to the viewmodel object.*/
		modelRender.Model = ViewModel;
		/*This sets the model that will be rendered by the SkinnedModelRenderer and assigns to the ViewModel Model*/
		modelRender.CreateBoneObjects = true;

		if ( GraphOverride != null )
			modelRender.AnimationGraph = GraphOverride; 
		/*Assigns GraphOverride to the AnimGraph Propery of the modelRender Component. Ensures the an animgraph is only applied if one has been 
		 * explicitly set, otherwise it leaves the default animation behavior unchanged.*/
		{
			var Arms = new GameObject();
			Arms.Parent = viewmodel;
			/*Creates a new GameObject called arms and makes its parent viewmodel, meaning any transformations applied to the viewmodel 
			 * will apply to the arms as well.*/

			var ArmsModel = Arms.Components.Create<SkinnedModelRenderer>();
			ArmsModel.Model = Model.Load( "models/first_person/first_person_arms.vmdl" );
			/*Creates and attaches a SkinnedModelRender component to the arms object. SkinnedModelRenderer handles rendering arm's model and any anims
			  Model.Load loads the model and assigns it to the 'Armsmodel' component's 'Model' property*/

			ArmsModel.BoneMergeTarget = modelRender;
			/*BoneMergeTarget property allows the bones of the Arns' model to merge with the bones of the viewmodel. Ensures that the animations or movement
			 on the viewmodel affect the Arms*/
		}
	}

	void PositionViewModel()
	{
		if ( viewmodel is null )
		{
			return;
		}

		var targetPos = Scene.Camera.Transform.World;
		targetPos.Position += targetPos.Rotation * ViewModelOffset;
		/*Scene.Camera.Transform.World retrieves the cameras world transform and stores it in TargetPos. 
		 * targetPos.Position += targetPos.Rotation * ViewModelOffset adjusts the viewmodels pos by adding an offset relative to the camera 
		   position and rotation. Ensures viewmodel stays aligned in front of the cmera.*/

		viewmodel.Transform.World = targetPos;

		//Add constraints to avoid exploits

		/*if (targetPos.Position.Length < 'MAX_ALLOWED_DISTANCE')
			{
				viewmodel.Transform.World = targetPos;
			}
		else
			{
				// Fallback to a default position if the target position is invalid
				viewmodel.Transform.World = fallbackPosition;
			}*/

	}
}
