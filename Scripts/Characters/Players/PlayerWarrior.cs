using System;
using Godot;
using Characters.Base;
using Characters.Entities.CharacterState;

namespace Characters.Players.PlayerWarrior;

public partial class PlayerWarrior : BaseCharacter
{
    protected bool isGaurd;
    private const float normalAttackCost = 10f;
    private const float hevayAttackCost = 20f;
    private ProgressBar staminaBar;
    private ProgressBar healthBar;

    public override void _Ready()
    {
        // Stamina initialize 
        base._Ready();
        maxStamina = 100f;
        stamina = maxStamina;
        staminaRestored = 20f;
        staminaBar = GetNode<ProgressBar>("UI/StaminaBar");
        
        // Health initialize
        maxHealth = 100f;
        health = maxHealth;
        healthBar = GetNode<ProgressBar>("UI/HealthBar");
        healthBar.MaxValue = maxHealth;
        healthBar.Value = health;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        staminaBar.Value = stamina;
    }
    
    
    
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

    protected override void HandleInput()
    {
        base.HandleInput();

        if (Input.IsActionJustPressed("AttackNormal"))
        {
            if (stamina >= normalAttackCost)
            {
                SpendStamina(normalAttackCost); 
                currentState = CharacterState.AttackNormal;
            }
        }
        else if (Input.IsActionJustPressed("AttackHeavy"))
        {
            if (stamina >= hevayAttackCost)
            {
                SpendStamina(hevayAttackCost);
                currentState = CharacterState.AttackHeavy;
            }
                
        }
    }
    
    protected override bool CanAttack(float cost)
    {
        return stamina >= cost;
    }

    protected override void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.Value = health;
        }
        
    }
}
