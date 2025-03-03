using Sandbox;
using System.Threading.Tasks;
public sealed class Barrier : Component
{

	[Property] public float RaiseHeight { get; set; } = 200f;
	[Property] public float RaiseSpeed { get; set; } = 5f;
	[Property] public float DestroyDelay { get; set; } = 1f;

	private bool isActivated = false;
	private Vector3 startPos;
	private Vector3 targetPos;

	protected override void OnStart()
	{
		startPos = GameObject.WorldPosition;
		targetPos = startPos + Vector3.Up * RaiseHeight;

	}

	public void ActivateBarrier()
	{
		if ( isActivated ) return;

		isActivated = true;
		_ = RaiseAndDestroy();
	}

	private async Task RaiseAndDestroy()
	{
		while ( GameObject.WorldPosition.z < targetPos.z )
		{
			GameObject.WorldPosition += Vector3.Up * RaiseSpeed * Time.Delta;
			await Task.Yield();
		}

		await Task.DelaySeconds( DestroyDelay );
		GameObject.Destroy();
	}
}
