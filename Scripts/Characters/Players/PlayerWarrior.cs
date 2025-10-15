using System;
using Godot;
using Characters.Base;
using Characters.Entities.CharacterState;

namespace Characters.Players.PlayerWarrior;

public partial class PlayerWarrior : BaseCharacter
{
    private const float normalAttackCost = 10f;
    private const float hevayAttackCost = 20f;
    private bool isGaurdActive = false;
    private bool canUseGuard = true;
    private float guardCooldown = 10f;
    private float guardDuration = 2f;
   

    public override void _Ready()
    {
        // Stamina initialize 
        base._Ready();
        maxStamina = 100f;
        stamina = maxStamina;
        staminaRestored = 10f;
        staminaBar = GetNode<ProgressBar>("../UI/StaminaBar");
        
        // Health initialize
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
    
    
    protected override void UseAbility()
    {
        if (!canUseGuard) return;

        isGaurdActive = true;
        canUseGuard = false;
        animatedSprite.Play("Guard");
       
        // Timer for guard duration
        var timer = new Timer();
        timer.OneShot = true;
        timer.WaitTime = guardDuration;
        AddChild(timer);
        timer.Timeout += () =>
        {
            isGaurdActive = false;
            Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            currentState = direction != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle;
        };
        timer.Start();
        
        // Timer for guard cooldown 
        var cooldownTimer = new Timer();
        cooldownTimer.OneShot = true;
        cooldownTimer.WaitTime = guardCooldown;
        AddChild(cooldownTimer);
        cooldownTimer.Timeout += () =>
        {
            canUseGuard = true;
        };
        cooldownTimer.Start();
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

    // Check weather the player can use special ability 
    protected override void HandleAbilityInput()
    {
        if (Input.IsActionJustPressed("SpecialAbility"))
        {
            if (canUseGuard)
            {
                currentState = CharacterState.SpecialAbility;
            }
            else
            {
                GD.Print("Guard still cooling down!");
            }
        }
    }

    // Block incoming damage if worrier is using special ability 
    public override void TakeDamage(float amount)
    {
        if (isGaurdActive)
        {
            Console.WriteLine("Attack Blocked No damage taken");
            return;
        }
        base.TakeDamage(amount);
    }
    
}
