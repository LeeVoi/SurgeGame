using Godot;
using System;

namespace Characters.Base;

/// <summary>
/// Base class for all enemy characters.
/// Handles basic AI behaviors like Idle, Patrol, Chase and Facing direction.
/// Also manages a basic health / damage logic. Other enemy types should inherit from this.
/// </summary>
public partial class BaseEnemy : CharacterBody2D
{
    // ---------------- Movement & Behavior Settings ----------------
    [Export] public float Speed { get; set; } = 100f;
    [Export] public float DetectionRange { get; set; } = 300f;

    // ---------------- Combat Settings ----------------
    [Export] public int Health { get; set; } = 3;
    [Export] public float AttackRange = 40f;
    [Export] public float AttackCooldown = 1.2f;
    [Export] public int AttackDamage = 1;
    private float _attackCooldownTimer = 0f;

    // ---------------- Internal State ----------------
    // Cached references
    protected float facingDirection = 1f;
    private AnimatedSprite2D _animatedSprite;

    // Movement and state variables
    private Node2D _player;
    private Vector2 _direction = Vector2.Zero;
    private float _idleTimer = 0f;
    private float _moveDuration = 1.5f;
    private float _idleDuration = 1f;
    private bool _isMoving = false;
    private bool _isChasing = false;
    private bool _allowChasing = true;

    // ---------------- Initialization ----------------
    /// <summary>
    /// Called when the node is added to the scene.
    /// Initializes components and starts random idle movement.
    /// </summary>
    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite.Play("Idle");
        SetRandomMovement();
    }

    // ---------------- Main Update Loop ----------------
    /// <summary>
    /// Called every physics frame. Handles movement, chasing logic, and animation updates.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame.</param>
    public override void _PhysicsProcess(double delta)
    {
        // Handle attack cooldown timer
        if (_attackCooldownTimer > 0f)
            _attackCooldownTimer -= (float)delta;

        HandleAi(delta);
    }

    // ---------------- Ai Logic ----------------
    /// <summary>
    /// Core AI logic for the enemy.
    /// </summary>
    private void HandleAi(double delta)
    {
        if (!_allowChasing || _player == null)
        {
            HandleIdle(delta);
            return;
        }

        float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);
        _isChasing = distanceToPlayer <= DetectionRange;

        if (_isChasing)
        {
            _direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            // If close enough to attack
            if (distanceToPlayer <= AttackRange)
            {
                Velocity = Vector2.Zero;
                TryAttack();
            }
            else
            {
                Velocity = _direction * Speed;
                MoveAndSlide();
                _animatedSprite.Play("Walk");
            }

            MoveAndSlide();
            FlipSprite();
        }
        else
        {
            HandleIdle(delta);
        }
    }

    // ---------------- Combat ----------------
    private void TryAttack()
    {
        if (_attackCooldownTimer > 0f)
            return;

        _attackCooldownTimer = AttackCooldown;
        // get the animation player once
        var animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");   // Uses engine to cook a signal call to the AttackPlayer()
        // play the attack animation
        animPlayer.Play("Attack");
    }

    /// <summary>
    /// This method is hooked to an engine event call on a specific frame.
    /// When method is called damage is applied.
    /// </summary>
    private void AttackPlayer()
    {

        // apply the damage at that exact point
        if (_player is BaseCharacter playerCharacter)
        {
            playerCharacter.TakeDamage(AttackDamage);
            GD.Print($"Attack landed! Damage: {AttackDamage}");
        }
        else if (_player != null)
        {
            GD.Print($"_player exists but is not a BaseCharacter (type: {_player.GetType().Name})");
        }
        else
        {
            GD.Print("No player reference — cannot attack.");
        }
    }


    // ---------------- Idle / Wandering ----------------
    private void HandleIdle(double delta)
    {
        if (_isMoving)
        {
            Velocity = _direction * Speed;
            MoveAndSlide();
        }
        else
        {
            Velocity = Vector2.Zero;
        }
        _idleTimer -= (float)delta;
        if (_idleTimer <= 0f)
            SetRandomMovement();
    }

    /// <summary>
    /// Toggles between idle and moving states.
    /// Randomizes a movement direction when switching to moving state.
    /// </summary>
    private void SetRandomMovement()
    {
        _isMoving = !_isMoving;
        _idleTimer = _isMoving ? _moveDuration : _idleDuration;

        if (_isMoving)
        {
            // Random unit vector for wandering direction.
            _direction = new Vector2(
                (float)GD.RandRange(-1, 1),
                (float)GD.RandRange(-1, 1)
            ).Normalized();
            _animatedSprite.Play("Walk");
        }
        else
        {
            _animatedSprite.Play("Idle");
        }
    }

    // ---------------- Utility Methods ----------------
    private void FlipSprite()
    {
        if (Math.Abs(_direction.X) > 0.1f)
        {
            facingDirection = _direction.X > 0 ? 1 : -1;
            _animatedSprite.FlipH = facingDirection < 0;
        }
    }

    /// <summary>
    /// Assigns the player target and whether this enemy can chase.
    /// Called automatically by the spawner during instantiation.
    /// </summary>
    /// <param name="player">Reference to the player node to chase.</param>
    /// <param name="canChase">Determines if the enemy is allowed to chase the player.</param>
    public void Initialize(Node2D player, bool canChase = true)
    {
        _player = player;
        _allowChasing = canChase;
    }


    // ---------------- Health & Damage ----------------
    /// <summary>
    /// Reduces the enemy's health by the specified amount.
    /// Destroys the enemy if health drops to zero or below.
    /// </summary>
    /// <param name="amount">Amount of damage to inflict.</param>
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
            Die();
    }
    /// <summary>
    /// Handles the enemy's death and cleanup.
    /// Currently removes the enemy from the scene.
    /// </summary>
    private void Die()
    {
        QueueFree();
    }
}
