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


	protected override void OnUpdate() /*Called every frame, use for real time tasks (visual updates, input)*/
	{
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
