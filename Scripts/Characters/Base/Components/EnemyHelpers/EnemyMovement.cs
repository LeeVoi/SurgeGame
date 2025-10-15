using System;
using System.Diagnostics;
using Godot;

namespace Characters.Base.Components.EnemyHelpers;

/// <summary>
/// Handles acceleration-based movement and sprite flipping for enemies.
/// Called every physics frame by BaseEnemy.
/// </summary>
public class EnemyMovement
{
    private readonly CharacterBody2D _body;
    private readonly AnimatedSprite2D _sprite;

    public float Speed = 100f;
    public float Acceleration = 400f;
    public float Deceleration = 600f;
    public Vector2 Direction = Vector2.Zero;

    public EnemyMovement(CharacterBody2D body)
    {
        _body = body;
        _sprite = body.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
    }

    /// <summary>
    /// Applies motion depending on the current AI state.
    /// </summary>
    public void Apply(EnemyState state, double delta)
    {
        switch (state)
        {
            case EnemyState.Idle:
                Stop(delta);
                break;

            case EnemyState.Chase:
                Move(delta);
                break;

            case EnemyState.Attack:
            case EnemyState.Dead:
                Stop(delta);
                break;
        }

        _body.MoveAndSlide();
        FlipSprite();
    }

    private void Move(double delta)
    {
        var targetVelocity = Direction * Speed;
        _body.Velocity = _body.Velocity.MoveToward(targetVelocity, Acceleration * (float)delta);
    }

    private void Stop(double delta)
    {
        _body.Velocity = _body.Velocity.MoveToward(Vector2.Zero, Deceleration * (float)delta);
    }

    private void FlipSprite()
    {
        if (_sprite == null) return;

        // Only flip when moving horizontally
        if (Math.Abs(Direction.X) > 0.1f)
            _sprite.FlipH = Direction.X < 0;
    }
}