using Godot;


namespace Characters.Base.Components.PlayerHelpers;

public class PlayerMovement
{
    public const float Speed = 300.0f;
    public float FacingDirection { get; private set; } = 1f;

    private readonly BaseCharacter _player;
    public Vector2 CurrentDirection;

    // Cached sprite base scale to prevent stretching when flipping
    private Vector2 _baseScale;
    private bool _scaleInitialized = false;

    public PlayerMovement(BaseCharacter player)
    {
        _player = player;

        // Try to cache base scale if animation is already initialized
        if (_player.Animation?.animatedSprite != null)
        {
            _baseScale = _player.Animation.animatedSprite.Scale;
            _scaleInitialized = true;
        }
        else
        {
            // Fallback: set to 1,1 in case PlayerAnimation isn’t ready yet
            _baseScale = Vector2.One;
        }
    }

    public void Move()
    {
        CurrentDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // Lazily initialize base scale if not yet set (e.g., on first frame)
        if (!_scaleInitialized && _player.Animation?.animatedSprite != null)
        {
            _baseScale = _player.Animation.animatedSprite.Scale;
            _scaleInitialized = true;
        }

        // Handle flipping — this mirrors both the sprite and HitBox (child)
        if (CurrentDirection.X != 0 && _scaleInitialized)
        {
            FacingDirection = CurrentDirection.X;

            // Always mirror relative to the base scale, not current scale
            var s = _baseScale;
            s.X *= FacingDirection < 0 ? -1 : 1;

            _player.Animation.animatedSprite.Scale = s;
        }

        var state = _player.Animation.GetState();

        bool canMove = state == Entities.CharacterState.CharacterState.Idle ||
                       state == Entities.CharacterState.CharacterState.Moving;

        if (canMove)
        {
            _player.Velocity = CurrentDirection * Speed;
            _player.MoveAndSlide();
        }
    }

}