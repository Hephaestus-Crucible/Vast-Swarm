using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

public sealed class Enemy : Component
{
	[Property] public float Health { get; set; } = 100.0f; //Enemy Health

	public void takeDamage(float damage)
	{
		Health -= damage;
		if ( Health <= 0 ) Die();
	}

	public void Die()
	{
		GameObject.Destroy(); //Removing enemy from the world
	}
	
}
