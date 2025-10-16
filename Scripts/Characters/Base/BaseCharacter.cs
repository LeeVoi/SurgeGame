using Godot;
using System;
using System.Reflection.Metadata;
using Characters.Base.Components.PlayerHelpers;
using Characters.Entities.CharacterState;
using Characters.Base.Components.CombatSystem.Interfaces;
using Characters.Base.Components.CombatSystem;

namespace Characters.Base;

public abstract partial class BaseCharacter : CharacterBody2D, IDamageable
{
	public PlayerMovement Movement { get; private set; }
	public PlayerAnimation Animation { get; private set; }

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


	// -------------- Combat ---------------
	protected HitBox _hitBox;
	protected HurtBox _hurtBox;

	[Export] public int AttackDamage { get; set; } = 20;


	public override void _Ready()
	{
		Animation = new PlayerAnimation(this);
		Movement = new PlayerMovement(this);

		// --- Set up hurtbox (for receiving hits)
		_hurtBox = GetNodeOrNull<HurtBox>("HurtBox");
		if (_hurtBox != null)
			_hurtBox.OwnerPath = GetPath();
		else
			GD.PrintErr($"{Name} has no HurtBox node!");

		// --- Set up hitbox (for dealing damage)
		_hitBox = GetNodeOrNull<HitBox>("AnimatedSprite2D/HitBox");
		if (_hitBox != null)
		{
			_hitBox.OwnerPath = GetPath();
			_hitBox.HitDetected += OnHitDetected;
			_hitBox.Deactivate(); // start disabled
		}
		else
			GD.PrintErr($"{Name} has no HitBox node!");
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

	// -------------- Combat logic -----------------
	protected virtual void OnHitDetected(HurtBox hurtbox)
	{
		if (hurtbox.Owner is IDamageable target)
			target.TakeDamage(AttackDamage, this);
	}

	// These are called by the AnimationPlayer during attack frames
	public void ActivateHitBox()
	{
		_hitBox?.Activate();
		GD.Print($"{Name} activated HitBox!");
	}

	public void DeactivateHitBox()
	{
		_hitBox?.Deactivate();
		GD.Print($"{Name} deactivated HitBox!");
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
	public virtual void TakeDamage(int amount, Node2D attacker)
	{
		health -= amount;
		health = Mathf.Max(health, 0);
		GD.Print($"{Name} hit by {attacker.Name}, health now {health}");
		UpdateHealthBar();

		if (health <= 0)
		{
			GD.Print($"{Name} died.");
			Die();
		}
	}

	// Add animations later to make the player die
	protected virtual void Die()
	{
		QueueFree();
	}

	// Handled by the Character class
	protected virtual void UpdateHealthBar() { }

	// Override to control ability 
	public virtual void HandleAbilityInput() { }
	public abstract void UseAbility();

	// Delegate to be able to override the input in each character class
	protected virtual void HandleInput()
	{
		Animation.UpdateInput();
	}

}
