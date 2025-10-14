using System;
using Godot;
using Characters.Base.BaseCharacter;
using Characters.Entities.CharacterState;

namespace Characters.Players.PlayerWarrior;

public partial class PlayerWarrior : BaseCharacter
{
    protected bool isGaurd;
    protected override void UseAbility()
    {
        isGaurd = true;
        animatedSprite.Play("Guard");
        
        var timer = new Timer();
        timer.OneShot = true;
        timer.WaitTime = 0.2f;
        AddChild(timer);
        timer.Timeout += () =>
        {
            isGaurd = false;
            Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            currentState = direction != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle;
        };
        timer.Start();
    }
}
