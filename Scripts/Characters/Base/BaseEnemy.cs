using Godot;
using System;

namespace Characters.Base.BaseEnemy;
public partial class BaseEnemy : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 100f;
    [Export] public int Health { get; set; } = 3;
    [Export] public float DetectionRange { get; set; } = 300f;

    protected float facingDirection = 1f;
    private AnimatedSprite2D _animatedSprite;
    private Node2D _player;
    private Vector2 _direction = Vector2.Zero;
    private float _idleTimer = 0f;
    private float _moveDuration = 1.5f;
    private float _idleDuration = 1f;
    private bool _isMoving = false;
    private bool _isChasing = false;
    private bool _allowChasing = true;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite.Play("Idle");
        SetRandomMovement();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_allowChasing && _player != null)
        {
            float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);
            _isChasing = distanceToPlayer < DetectionRange;

            if (_isChasing)
            {
                _direction = (_player.GlobalPosition - GlobalPosition).Normalized();
                _animatedSprite.Play("Walk");
            }
        }

        if (_isMoving || _isChasing)
        {
            Velocity = _direction * Speed;
            MoveAndSlide();

            if (Math.Abs(_direction.X) > 0.1f)
            {
                facingDirection = _direction.X > 0 ? 1 : -1;
                _animatedSprite.FlipH = facingDirection < 0;
            }
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        if (!_isChasing)
        {
            _idleTimer -= (float)delta;
            if (_idleTimer <= 0)
                SetRandomMovement();
        }
    }

    private void SetRandomMovement()
    {
        _isMoving = !_isMoving;
        _idleTimer = _isMoving ? _moveDuration : _idleDuration;

        if (_isMoving)
        {
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

    public void Initialize(Node2D player, bool canChase = true)
    {
        _player = player;
        _allowChasing = canChase;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
            Die();
    }

    private void Die()
    {
        QueueFree();
    }
}
