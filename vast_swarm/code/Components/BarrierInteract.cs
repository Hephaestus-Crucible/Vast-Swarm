using Sandbox;
using Sandbox.UI;

public sealed class BarrierInteract : Component, Component.ITriggerListener, Component.ICollisionListener
{
	[Property] public Barrier Barrier;

	private bool isPlayerInRange = false;
	//private BarrierUI barrierUI;


	protected override void OnStart()
	{
		/*barrierUI = Game.ActiveScene.GetAllComponents<BarrierUI>().FirstOrDefault();
		if ( barrierUI == null )
		{
			barrierUI = new BarrierUI();
			Game.ActiveScene.AddComponent<BarrierUI>();
		}*/

		var collider = GameObject.Components.Get<BoxCollider>();
		if ( collider != null )
		{
			collider.IsTrigger = true;
		}
		else
		{
			Log.Warning( "Barrier Interact requires a collider component set as a trigger" );
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
			//barrierUI.Message = "Press E to remove barrier";
			//barrierUI.IsVisible = true;
		}

	}

	public void OnTriggerExit( Collider Other )				//ensure that its a player
	{
		if( Other.GameObject.Tags.Has( "player" ) )
		{
			isPlayerInRange = false;
			//barrierUI.IsVisible = false;
		}
	}
}

