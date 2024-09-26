using Sandbox;
using Sandbox.UI;

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
	protected override void OnUpdate() /*Called every frame, use for real time tasks (visual updates, input)*/
	{
		PositionViewModel();
		AnimationHandler();
		
	}

	protected override void OnFixedUpdate() /*consistent tick, not dependent on framerate, use with physics related tasks*/
	{
		base.OnFixedUpdate();

		
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


		var rotationDelta = Rotation.Difference( lastRot, Scene.Camera.Transform.Rotation );
		/*Detects how much the camera has rotated lince last frame, assigns to rotationDelta
		 Scene.Camera.Transform.Rotation gets the current rotation of the camera in the scene
		 Useful for having the animation reac to camera rotation, for realism.*/

		lastRot = Scene.Camera.Transform.Rotation;
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

	}

	//Core Weapon Projectile Behavior
	void FireProjectile()
	{

	}

	void CreateViewModel()
	{
		viewmodel = new GameObject( true, "viewmodel" );
		
		var modelRender = viewmodel.Components.Create<SkinnedModelRenderer>();
		/* Creates a new SkinnedModelRenderer component and attaches it to the viewmodel object.*/
		modelRender.Model = ViewModel;
		/*This sets the model that will be rendered by the SkinnedModelRenderer and assogms to the ViewModel Model*/
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
		if ( viewmodel != null )
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

		/*if (targetPos.Position.Length < MAX_ALLOWED_DISTANCE)
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
