using Sandbox;

public sealed class BarrierInteract : Component
{
	[Property] public Barrier Barrier;

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "use" ) ) // 'E' or interact button
		{
			if ( Barrier != null )
			{
				Barrier.ActivateBarrier();
			}
		}
	}
}

