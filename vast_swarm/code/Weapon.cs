using Sandbox;

public sealed class Weapon : Component
{
	[Property, Group("View Model")]
	public Model ViewModel { get; set; }


	[Property, Group("View Model Offset")]
	public Vector3 ViewModelOffset { get; set; }



	protected override void OnUpdate()
	{

	}
}
