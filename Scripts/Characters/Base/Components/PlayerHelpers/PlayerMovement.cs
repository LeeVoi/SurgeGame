using Godot;


namespace Characters.Base.Components.PlayerHelpers;

public class PlayerMovement
{
    public const float Speed = 300.0f;
    public float facingDirection { get; private set; } = 1f;
    private readonly BaseCharacter _player;
    public Vector2 CurrentDirection  { get; private set; }


    public PlayerMovement(BaseCharacter player)
    {
        this._player = player;
    }

    public void Move()
    {
        CurrentDirection = Input.GetVector("move_left","move_right", "move_up", "move_down");

        if (CurrentDirection.X != 0)
        {
            facingDirection = CurrentDirection.X;
            _player.Animation.animatedSprite.FlipH = facingDirection < 0;
        }

        var state = _player.Animation.GetState();

        bool canMove = state != Entities.CharacterState.CharacterState.Idle &&
                       state != Entities.CharacterState.CharacterState.Dead;

        if (canMove)
        {
            _player.Velocity = CurrentDirection * Speed;
            _player.MoveAndSlide();
        }


    }

}