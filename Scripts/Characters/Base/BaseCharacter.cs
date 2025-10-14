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
	}

	protected virtual void HandleInput()
	{
		if (Input.IsActionPressed("AttackNormal"))
		{
			currentState = CharacterState.AttackNormal;
		}
		else if (Input.IsActionPressed("AttackHeavy"))
		{
			currentState = CharacterState.AttackHeavy;
		}
		else if (Input.IsActionPressed("SpecialAbility"))
		{
			currentState = CharacterState.SpecialAbility;
		}
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
	
}
