
using Godot;

namespace Characters.Base.Components.EnemyHelpers;

public class EnemyAimation
{
    private readonly AnimatedSprite2D _sprite;

    public EnemyAimation(BaseEnemy enemy)
    {
        _sprite = enemy.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public void PlayState(EnemyState state)
    {
        if (_sprite == null) return;

        string anim = state switch
        {
            EnemyState.Idle => "Idle",
            EnemyState.Attack => "Attack",
            EnemyState.Chase => "Walk",
            EnemyState.Dead => "Die",
            _ => "Idle"
        };

        if (_sprite.Animation != anim)
            _sprite.Play(anim);
    }
}