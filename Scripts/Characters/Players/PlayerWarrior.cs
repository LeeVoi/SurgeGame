using System;
using Godot;
using Characters.Base;
using Characters.Entities.CharacterState;

namespace Characters.Players.PlayerWarrior;

public partial class PlayerWarrior : BaseCharacter
{
    private bool isGuardActive = false;
    private bool canUseGuard = true;
    private float guardCooldown = 10f;
    private float guardDuration = 2f;

    public override void _Ready()
    {
        base._Ready();

        maxStamina = 100f;
        stamina = maxStamina;
        staminaRestored = 10f;
        staminaBar = GetNode<ProgressBar>("../UI/StaminaBar");

        maxHealth = 100f;
        health = maxHealth;
        healthBar = GetNode<ProgressBar>("../UI/HealthBar");
        healthBar.MaxValue = maxHealth;
        healthBar.Value = health;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        staminaBar.Value = stamina;
    }

    public override void UseAbility()
    {
        if (!canUseGuard) return;

        isGuardActive = true;
        canUseGuard = false;
        Animation.animatedSprite.Play("Guard");

        var timer = new Timer { OneShot = true, WaitTime = guardDuration };
        AddChild(timer);
        timer.Timeout += () =>
        {
            isGuardActive = false;
            timer.QueueFree();
            Vector2 dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            Animation.SetState(dir != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle);
        };
        timer.Start();

        var cooldown = new Timer { OneShot = true, WaitTime = guardCooldown };
        AddChild(cooldown);
        cooldown.Timeout += () => { canUseGuard = true; cooldown.QueueFree(); };
        cooldown.Start();
    }

    public override void HandleAbilityInput()
    {
        if (Input.IsActionJustPressed("SpecialAbility"))
        {
            if (canUseGuard)
                Animation.SetState(CharacterState.SpecialAbility);
            else
                GD.Print("Guard still cooling down!");
        }
    }

    public override void TakeDamage(int amount, Node2D attacker)
    {
        if (isGuardActive)
        {
            GD.Print("Attack Blocked! No damage taken.");
            return;
        }
        base.TakeDamage(amount, attacker);
    }

    protected override void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.Value = health;
    }

}
