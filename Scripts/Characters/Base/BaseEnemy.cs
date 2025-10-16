using Godot;
using Characters.Base.Components.EnemyHelpers;
using System;
using Characters.Base.Components.CombatSystem;
using Characters.Base.Components.CombatSystem.Interfaces;

namespace Characters.Base;

/// <summary>
/// Base class for all enemy characters.
/// Handles chasing and attacking the player.
/// Other enemy types should inherit from this.
/// </summary>
public partial class BaseEnemy : CharacterBody2D, IDamageable
{
    private EnemyMovement _movement;
    private EnemyCombat _combat;
    private EnemyAimation _animation;
    private HurtBox _hurtBox;

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

        _hurtBox = GetNodeOrNull<HurtBox>("HurtBox");
        if (_hurtBox != null)
            _hurtBox.OwnerPath = GetPath();

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
    /// This method applies damage to the target.
    /// </summary>
    /// <param name="amount"> amount of damage to apply to the target</param>
    /// <param name="attacker"> Right now not used for anything</param>
    public void TakeDamage(int amount, Node2D attacker)
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

        // Disable collisions safely after physics query.
        var collider = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (collider != null)
            collider.SetDeferred("disabled", true);

        // Stop physics immediately.
        SetPhysicsProcess(false);
        Velocity = Vector2.Zero;

        // wait for death animation.
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

    // Called from AnimationPlayer
    public void ActivateHitbox()
    {
        _combat?.ActivateHitbox();
    }

    // Called from AnimationPlayer
    public void DeactivateHitbox()
    {
        _combat?.DeactivateHitbox();
    }
    public override void _Draw()
    {
        // Draws a semi-transparent red circle showing attack reach
        if (_combat != null)
            DrawCircle(Vector2.Zero, _combat.AttackRange, new Color(1, 0, 0, 0.25f));
    }
}

