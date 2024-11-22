using Sandbox;
using Sandbox.Citizen;
using System.Security.Cryptography.X509Certificates;

public sealed class VastSwarmPlayer : Component
{
	[Property]
	[Category( "Components" )]
	public GameObject Camera { get; set; }

	[Property]
	[Category( "Components" )]
	public CharacterController Controller { get; set; }

	[Property]
	[Category( "Components" )]
	public CitizenAnimationHelper Animator { get; set; }

	/// <summary>
	/// How fast you can walk (Units per second) 
	/// </summary>
	[Property]
	[Category( "Stats" )]
	//Creating a slider to create limits for attributes
	[Range( 0f, 400f, 1f )]
	public float WalkSpeed { get; set; } = 120f;

	/// <summary>
	/// How fast you can run (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	//Creating a slider to create limits for attributes
	[Range( 0f, 800f, 1f )]
	public float RunSpeed { get; set; } = 250f;

	/// <summary>
	/// How high you will be able to jump (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	//Creating a slider to create limits for attributes
	[Range( 0f, 1000f, 10f )]
	public float JumpStrength { get; set; } = 400f;

	/// <summary>
	/// Where the camera rotates around and the aim originates from
	/// </summary>
	[Property]
	public Vector3 EyePosition { get; set; }

	//EyeAngles is used to keep track of what the character is currently looking at
	public Angles EyeAngles { get; set; }
	Transform _initialCameraTransform;

	protected override void DrawGizmos()
	{
		Gizmo.Draw.LineSphere( EyePosition, 10f );
	}

	protected override void OnUpdate()
	{
		//We add AnalogLook to eye angles to determine how far the mouse has moved since the last frame
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -45f, 45f ) );
		//Transform.Rotation will now update the player view depending on where the EyeAngles.yaw has been moved to (left to right)
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		if ( Camera != null )
			Camera.Transform.Local = _initialCameraTransform.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if ( Controller == null ) return;

		//Determines whether the player is pressing the run key, in this instance "Shift", and changes the WalkSpeed to run speed. 
		//Otherwise, WalkSpeed is default
		var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
		//wishVelocity takes the player's inputs and multiplies it with the wishSpeed
		//This will allow functionality with WASD along with any controller, essentially creating 
		//universal inputs. Transform.Rotation is multiplied to change velocity vector with player's rotation
		//Normal vector is used to keep the diagonal movement speed the same as vertical/horizontal
		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * Transform.Rotation;

		//Accelerating controller based off of the wishVelocity
		Controller.Accelerate( wishVelocity );

		//This if statement will keep player from sliding after they have stopped inputting movement by applying friction
		if ( Controller.IsOnGround )
		{
			//Default controller acceleration
			Controller.Acceleration = 10f;
			Controller.ApplyFriction( 5f );


			//Check if "Jump" is pressed, in this case Space, which is only determined in the frame the input is pressed
			//The punch method is used to push the player up, determined by jump strength, which is applied to the velocity vector instantaneously. 
			//Useful since the controller likes to stick the player to the ground
			if ( Input.Pressed( "Jump" ) )
			{
				Controller.Punch( Vector3.Up * JumpStrength );

				//If jump is used, then the jump animation is triggered.
				if ( Animator != null )
					Animator.TriggerJump();
			}

		}
		//If controller is not on the ground, this will apply gravity to bring character back down
		else
		{
			//Acceleration is cut down to half, to prevent strafing when midair
			Controller.Acceleration = 5f;
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}
		//After determining what we want the controller to do, Controller.Move() is used to apply the complicated stuff
		Controller.Move();

		//This helps the controller understand when the character is back on the ground
		//Creating smooth transition of animation
		if ( Animator != null )
		{
			Animator.IsGrounded = Controller.IsOnGround;
			Animator.WithVelocity( Controller.Velocity );
		}

	}

	protected override void OnStart()
	{
		if ( Camera != null )
			_initialCameraTransform = Camera.Transform.Local;

		if ( Components.TryGet<SkinnedModelRenderer>( out var model ) )
		{
			var clothing = ClothingContainer.CreateFromLocalUser();
			clothing.Apply( model );
		}
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

}
