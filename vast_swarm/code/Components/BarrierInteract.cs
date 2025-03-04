using Sandbox;
using Sandbox.UI;

public sealed class BarrierInteract : Component, Component.ITriggerListener, Component.ICollisionListener
{
	[Property] public Barrier Barrier;
	[Property] public BarrierUI BarrierUI;

	private bool isPlayerInRange = false;
	


	protected override void OnStart()
	{
		
		var collider = GameObject.Components.Get<BoxCollider>();
		if ( collider != null )
		{
			collider.IsTrigger = true;
		}
		else
		{
			Log.Warning( "Barrier Interact requires a collider component set as a trigger" );
		}

		if ( BarrierUI == null )
		{
			BarrierUI = Game.ActiveScene.GetAllComponents<BarrierUI>().FirstOrDefault();
			if ( BarrierUI == null )
			{
				Log.Error( "No BarrierUI found in the scene" );
			}
			else
			{
				Log.Info( $"BarrierUI found in Play Mode: {BarrierUI.GameObject.Name}" );
			}
		}
	}

	protected override void OnUpdate()
	{
		if ( !isPlayerInRange )
		{
			return;
		}
		if ( Input.Pressed( "use" ) ) // 'E' or interact button
		{
			if ( Barrier != null )
			{
				Barrier.ActivateBarrier();
				//barrierUI.IsVisible = false;
			}
		}
	}

	public void OnTriggerEnter(Collider Other )
	{
		if ( Other.GameObject.Tags.Has( "player" ))				//ensure that its a player
		{
			isPlayerInRange = true;
			if ( BarrierUI != null )
			{
				BarrierUI.IsVisible = true;
			}
		}

	}

	public void OnTriggerExit( Collider Other )				//ensure that its a player
	{
		if( Other.GameObject.Tags.Has( "player" ) )
		{
			isPlayerInRange = false;
			if ( BarrierUI != null )
			{
				BarrierUI.IsVisible = false;
			}
		}
	}
}

