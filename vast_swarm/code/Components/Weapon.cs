using Sandbox;

public sealed class Weapon : Component
{
	[Property, Group("View Model")]
	public Model ViewModel { get; set; }
	/*Property holds the 3d model for the weapons view model, can set in the editor to change the appearance.*/


	[Property, Group("View Model Offset")]
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
	public GameObject PrimaryFireVFXPrefab { get; set; }
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

	protected override void OnUpdate() /*Called every frame, use for real time tasks (visual updates, input)*/
	{
		//position viewmodel()
		//call animationhandler()
		

	}

	protected override void OnFixedUpdate() /*consistent tick, not dependent on framerate, use with physics related tasks*/
	{
		base.OnFixedUpdate();

		
	}

	protected override void OnEnabled()
	{
		//Create the view model for the weapon when switching to it (enables the weapon when switched to)
	}


	protected override void OnDisabled()
	{
		//Destroy the viewmodel when switching off of the weapon
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
		/*Sets paramaters in the weapons animation system, animgraph*/
	
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
		//Create and configure first person view model of the weapon
	}

	void PositionViewModel()
	{
		//Positions viewmodel in front of the camera.
	}
}
