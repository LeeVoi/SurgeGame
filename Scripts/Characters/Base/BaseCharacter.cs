using Godot;
using System;
using Characters.Entities.CharacterState;

namespace Characters.Base.BaseCharacter;

public abstract partial class BaseCharacter : CharacterBody2D
{
	public const float Speed = 300.0f;
	protected AnimatedSprite2D	animatedSprite;
	protected float facingDirection = 1;
	protected CharacterState currentState = CharacterState.Idle;
	protected float stamina;
	protected float maxStamina;
	protected float staminaRestored;
	protected float maxHealth; 
	protected float health;
	
	
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite.AnimationFinished += OnAnimationFinished;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		HandleMovement();
		HandleAnimation();
		stamina = Mathf.Min(maxStamina, stamina + staminaRestored * (float)delta);
	}

	protected virtual void HandleInput()
	{
		if (Input.IsActionPressed("AttackNormal") && CanAttack(10f))
		{
			currentState = CharacterState.AttackNormal;
		}
		else if (Input.IsActionPressed("AttackHeavy") && CanAttack(20f))
		{
			currentState = CharacterState.AttackHeavy;
		}
		else if (Input.IsActionPressed("SpecialAbility"))
		{
			currentState = CharacterState.SpecialAbility;
		}
		//Temp 
		else if (Input.IsActionJustReleased("TakeDamage"))
		{
			TakeDamage(20f);
		}
		//Temp
		else
		{
			Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

			
			if (currentState != CharacterState.AttackNormal &&
			    currentState != CharacterState.AttackHeavy &&
			    currentState != CharacterState.SpecialAbility)
			{
				currentState = direction != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle;
			}
		}
	}

	protected virtual void HandleMovement()
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction.X != 0)
		{
			facingDirection = direction.X;
			animatedSprite.FlipH = facingDirection < 0;
		}
		
		Vector2 velocity = direction * Speed;
		Velocity = velocity;
		MoveAndSlide();
	}

	protected virtual void HandleAnimation()
	{
		switch (currentState)
		{
			case CharacterState.Idle:
				animatedSprite.Play("Idle");
				break;
			case CharacterState.Moving:
				animatedSprite.Play("Run");
				break;
			case CharacterState.AttackNormal:
				animatedSprite.Play("AttackNormal");
				break;
			case CharacterState.AttackHeavy:
				animatedSprite.Play("AttackHeavy");
				break;
			case CharacterState.SpecialAbility:
					UseAbility();
				break;
		}
	}

	
	protected virtual void OnAnimationFinished()
	{
		if (animatedSprite.Animation == "AttackNormal" || animatedSprite.Animation == "AttackHeavy")
		{
			Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			currentState = direction != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle;
		}
	}
	protected abstract void UseAbility();
	
	
	// -------------- Stamina System -----------------
	protected bool SpendStamina(float amount)
	{
		if (stamina >= amount)
		{
			stamina -= amount;
			return true;
		}
		return false;
	}
	
	protected virtual bool CanAttack(float cost)
	{
		return true;
	}
	
	// ------------------- Health System ------------------

	protected virtual void TakeDamage(float amount)
	{
		health -= amount;
		health = Mathf.Max(health, 0);
		UpdateHealthBar();

		if (health <= 0)
		{
			Console.WriteLine("You Died");
			//Die();
		}
	}

	// Add animations later to make the player die
	protected virtual void Die()
	{
		QueueFree();
	}

	// Handled by the Character class
	protected virtual void UpdateHealthBar(){ }
	
}
