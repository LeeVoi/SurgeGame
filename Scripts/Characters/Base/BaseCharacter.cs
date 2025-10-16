using Godot;
using System;
using System.Reflection.Metadata;
using Characters.Base.Components.PlayerHelpers;
using Characters.Entities.CharacterState;

namespace Characters.Base;

public abstract partial class BaseCharacter : CharacterBody2D
{
	public PlayerMovement Movement { get; private set; }
	public PlayerAnimation Animation  { get; private set; }
	
	// --------------- Stamina System ---------------
	protected float stamina;
	protected float maxStamina;
	protected float staminaRestored;
	// ------------- Health system ---------------
	protected float maxHealth; 
	protected float health;
	// -------------- UI ---------------
	protected ProgressBar staminaBar;
	protected ProgressBar healthBar;
	
	
	
	public override void _Ready()
	{
		Movement = new PlayerMovement(this);
		Animation = new PlayerAnimation(this);
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		//Handle input and animation state
		HandleInput();
		// Handle movement & animation rendering
		Movement.Move();
		Animation.UpdateAnimation();
		// Restore stamina
		stamina = Mathf.Min(maxStamina, stamina + staminaRestored * (float)delta);
	}
	
	
	// -------------- Stamina System -----------------
	public bool SpendStamina(float amount)
	{
		if (stamina >= amount)
		{
			stamina -= amount;
			return true;
		}
		return false;
	}
	
	public virtual bool CanAttack(float cost)
	{
		return true;
	}
	
	// ------------------- Health System ------------------
	public virtual void TakeDamage(float amount)
	{
		health -= amount;
		health = Mathf.Max(health, 0);
		UpdateHealthBar();

		if (health <= 0)
		{
			Console.WriteLine("You Died");
			Die();
		}
	}

	// Add animations later to make the player die
	protected virtual void Die()
	{
		QueueFree();
	}

	// Handled by the Character class
	protected virtual void UpdateHealthBar(){ }
	
	// Override to control ability 
	public virtual void HandleAbilityInput() { }
	public abstract void UseAbility();

	// Delegate to be able to override the input in each character class
	protected virtual void HandleInput()
	{
		Animation.UpdateInput();
	}
	
}
