using Godot;
using System;

namespace Characters.Base.BaseCharacter;

public partial class BaseCharacter : CharacterBody2D
{
	public const float Speed = 300.0f;
	public AnimatedSprite2D	animatedSprite;
	public float facingDirection = 1;
	

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite.AnimationFinished += OnAnimationFinished;
	}

	public void OnAnimationFinished()
	{
		if (animatedSprite.Animation == "AttackNormal" || animatedSprite.Animation == "AttackHeavy")
		{
			animatedSprite.Play("Idle");
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (Input.IsActionJustPressed("AttackNormal"))
		{
			animatedSprite.Play("AttackNormal");
			
		}

		if (Input.IsActionJustPressed("AttackHeavy"))
		{
			animatedSprite.Play("AttackHeavy");
			
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
			facingDirection = direction.X;
			
			
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			
		}

		if (animatedSprite.Animation != "AttackNormal" && animatedSprite.Animation != "AttackHeavy")
		{
			if (direction != Vector2.Zero)
			{
				animatedSprite.Play("Run");
				animatedSprite.FlipH = direction.X < 0;
			}
			else
			{
				animatedSprite.Play("Idle");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
