using Godot;
using Characters.Base.Components.EnemyHelpers;
using System;

namespace Characters.Base;

/// <summary>
/// Base class for all enemy characters.
/// Handles chasing and attacking the player.
/// Other enemy types should inherit from this.
/// </summary>
public partial class BaseEnemy : CharacterBody2D
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemyAimation _animation;

    private Node2D _player;
    private bool _allowChasing = true;
    private EnemyState _state = EnemyState.Idle;

    // --- Stats ---
    [Export]
    public float Speed
    {
        get => _movement.Speed;
        set => _movement.Speed = value;
    }
    [Export]
    public float AttackRange
    {
        get => _combat.AttackRange;
        set => _combat.AttackRange = value;
    }

    [Export]
    public int AttackDamage
    {
        get => _combat.Damage;
        set => _combat.Damage = value;
    }

    [Export] public int Health { get; set; } = 10;

    public override void _Ready()
    {
        _movement = new EnemyMovement(this);
        _combat = new EnemyCombat(this);
        _animation = new EnemyAimation(this);

        _state = EnemyState.Idle;
    }

    public override void _PhysicsProcess(double delta)
    {
        _combat.Update(delta);
        UpdateAi(delta);
        _movement.Apply(_state, delta);
        _animation.PlayState(_state);
    }

    private void UpdateAi(double delta)
    {
        // Dead enemies skip logic
        if (_state == EnemyState.Dead) return;

        // If no player or not allowed to chase → stand idle
        if (_player == null || !_allowChasing)
        {
            _state = EnemyState.Idle;
            _movement.Direction = Vector2.Zero;
            return;
        }

        // Always chase the player
        _movement.Direction = (_player.GlobalPosition - GlobalPosition).Normalized();

        float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

        // Attack if close enough
        if (distance <= _combat.AttackRange)
        {
            _state = EnemyState.Attack;
            _combat.TryAttack();
        }
        else
        {
            _state = EnemyState.Chase;
        }
    }

    /// <summary>
    /// Called by the AnimationPlayer during the attack animation
    /// at the exact frame when the hit should connect.
    /// </summary>
    public void AttackPlayer()
    {
        if (_state == EnemyState.Dead || _player == null)
            return;

        _combat.DealDamage(_player);
    }

    public void TakeDamage(int amount)
    {
        if (_state == EnemyState.Dead)
            return;

        Health -= amount;

        if (Health <= 0)
            Die();
    }

    private async void Die()
    {
        _state = EnemyState.Dead;
        _animation.PlayState(_state);

        var animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer != null)
            await ToSignal(animPlayer, "animation_finished");

        QueueFree();
    }

    public void Initialize(Node2D player, bool allowChasing = true)
    {
        _player = player;
        _allowChasing = allowChasing;
    }
    public override void _Draw()
    {
        // Draws a semi-transparent red circle showing attack reach
        if (_combat != null)
            DrawCircle(Vector2.Zero, _combat.AttackRange, new Color(1, 0, 0, 0.25f));
    }
}

