using Sandbox;
using Sandbox.Citizen;

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
	[Range( 0f, 400f, 1f )]
	public float WalkSpeed { get; set; } = 120f;

	/// <summary>
	/// How fast you can run (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 800f, 1f )]
	public float RunSpeed { get; set; } = 250f;

	/// <summary>
	/// How high you will be able to jump (Units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 1000f, 10f )]
	public float JumpStrength { get; set; } = 400f;

	[Property]
	public Vector3 EyePosition { get; set; }

	public Vector3 EyeWorldPosition => Transform.Local.PointToWorld( EyePosition );
	public Angles EyeAngles { get; set; }

	Transform _initialCameraTransform;

	private void AdjustModelForFirstPerson()
	{
		if (Components.TryGet<SkinnedModelRenderer>(out var modelRenderer))
		{
			// Loop through the body groups and match the name
			foreach (var bodyGroup in modelRenderer.Model.BodyParts)
			{
				// Check if the body group name is "head_lod0"
				if (bodyGroup.Name == "head_lod0")
				{
					// Get the index of the body group
					// Need to continue working on this part of the code
					// For some reason BodyGroups does not seem to be accesible
					// Might be because it is defined in a different way
					// Or I am looking in the wrong direction all together
					// Overall the task here is keep the camera from clipping within the head of character
					//int headIndex = modelRenderer.Model.BodyParts.IndexOf( bodyGroup );

					// Set the body group to invisibile (0)
					//modelRenderer.SetBodyGroup( headIndex, 0 ); // Make the head invisible
					break;
				}
			}
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Camera == null ) return;

		UpdateEyeRotation();
		AdjustModelForFirstPerson();
		PerformRayTracingForCamera();
	}

	private void UpdateEyeRotation()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -20f, 20f ) );
		WorldRotation = Rotation.FromYaw( EyeAngles.yaw );
	}

	private void PerformRayTracingForCamera()
	{
		//var eyePosition = EyeWorldPosition;
		//var eyeRotation = EyeAngles;

		float traceDistance = 5f; // Defining a close range for first-person clipping detection
		Vector3 eyePosition = EyeWorldPosition; 

		// Perform ray-trace directly forward from eye position
		var forwardTrace = Scene.Trace.Ray( eyePosition, eyePosition + EyeAngles.Forward * traceDistance )
			.Size( 5f ) // Camera "size" for trace
			.IgnoreGameObjectHierarchy( GameObject ) // Ignore the player's own model
			.WithoutTags( "player" ) // Avoid collision with entities tagged as "player"
			.Run();

		if ( forwardTrace.Hit )
		{
			// Reposition camera slightly back from obstacle to prevent clipping
			Camera.WorldPosition = forwardTrace.EndPosition - EyeAngles.Forward * 0.1f; //Back off a small amount
		}
		else
		{
			// Place camera at the expected eye position if no obstacle
			Camera.WorldPosition = eyePosition;
		}

		DrawGizmoAtEyePosition( Camera.WorldPosition );
	}

	private void DrawGizmoAtEyePosition( Vector3 position )
	{
		Gizmo.Draw.LineSphere( position, 10f );
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		if ( Controller == null ) return;

		UpdateMovement();
		UpdateAnimation();
	}

	private void UpdateMovement()
	{
		var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
		var wishVelocity = Input.AnalogMove.Normal * wishSpeed * WorldRotation;

		Controller.Accelerate( wishVelocity );

		if ( Controller.IsOnGround )
		{
			Controller.Acceleration = 10f;
			Controller.ApplyFriction( 5f );
			HandleJump();
		}
		else
		{
			Controller.Acceleration = 5f;
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();
	}

	private void HandleJump()
	{
		if ( Input.Pressed( "Jump" ) )
		{
			Controller.Punch( Vector3.Up * JumpStrength );
			Animator?.TriggerJump();
		}
	}

	private void UpdateAnimation()
	{
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

