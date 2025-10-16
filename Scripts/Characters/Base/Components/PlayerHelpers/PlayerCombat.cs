using Godot;
using Characters.Base.Components.CombatSystem;
using System;
using Characters.Base.Components.CombatSystem.Interfaces;

namespace Characters.Base.Components.PlayerHelpers;

/// <summary>
/// Handles the Player's combat logic - attack input, stamina cost, and hit detection.
/// </summary>
public class PlayerCombat
{
    private readonly BaseCharacter _player;
    private readonly AnimationPlayer _anim;
    private HitBox _hitBox;

    public int Damage { get; set; } = 20;
    public float NormalAttackCost { get; set; } = 10f;
    public float HevyAttackCost { get; set; } = 20f;


    public PlayerCombat(BaseCharacter player)
    {
        _player = player;
        _anim = player.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _hitBox = player.GetNodeOrNull<HitBox>("AnimatedSprite2D/HitBox");

        if (_hitBox == null)
            GD.PrintErr($"{player.Name} has no HitBox node!");
        else
        {
            _hitBox.OwnerPath = player.GetPath();
            _hitBox.HitDetected += OnHitDetected;
            _hitBox.Deactivate();
        }
    }

    /// <summary>
    /// Handles input related to attack (normal/heavy)
    /// Called by BaseCharacter.cs during HandleInput
    /// </summary>
    /// <param name="box"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void HandleAttackInput()
    {
        if (Input.IsActionJustPressed("AttackNormal") && _player.CanAttack(NormalAttackCost))
        {
            _player.SpendStamina(NormalAttackCost);
            _anim?.Play("AttackNormal");
        }
        else if (Input.IsActionJustPressed("AttackHeavy") && _player.CanAttack(HevyAttackCost))
        {
            _player.SpendStamina(HevyAttackCost);
            _anim?.Play("AttackHeavy");
        }
    }

    private void OnHitDetected(HurtBox hurtbox)
    {
        if (hurtbox.Owner is IDamageable target)
            target.TakeDamage(Damage, _player);
    }

    public void ActivateHitBox() => _hitBox?.Activate();
    public void DeactivateHitBox() => _hitBox?.Deactivate();
}