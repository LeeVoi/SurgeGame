
using Godot;

namespace Characters.Base.Components.EnemyHelpers;

/// <summary>
/// Handles an enemy's offensive behavior — attack timing, cooldowns,
/// animation triggers, and applying damage to targets.
/// Designed to be modular so that other combat types (ranged, area, etc.)
/// can inherit and override its methods.
/// </summary>
public class EnemyCombat
{
    // --- References ---
    private readonly BaseEnemy _enemy;             // Reference to the enemy
    private readonly AnimationPlayer _anim;        // Animation player controlling attack animations

    // --- Attack parameters ---
    /// <summary>
    /// Maximum distance at which this enemy can perform an attack.
    /// </summary>
    public float AttackRange = 40f;

    /// <summary>
    /// Time (in seconds) between consecutive attacks.
    /// </summary>
    public float Cooldown = 1.2f;

    /// <summary>
    /// Amount of damage dealt to the target per successful hit.
    /// </summary>
    public int Damage = 5;

    // --- Internal state ---
    private float _cooldownTimer = 0f;

    /// <summary>
    /// Returns true if the enemy can currently perform an attack.
    /// </summary>
    public bool CanAttack => _cooldownTimer <= 0f;

    /// <summary>
    /// Constructs a new EnemyCombat component.
    /// </summary>
    /// <param name="enemy">The parent enemy using this combat component.</param>
    public EnemyCombat(BaseEnemy enemy)
    {
        _enemy = enemy;
        _anim = enemy.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
    }

    /// <summary>
    /// Updates the internal cooldown timer each frame.
    /// Should be called once per physics frame by the owning enemy.
    /// </summary>
    /// <param name="delta">Time elapsed since last frame.</param>
    public void Update(double delta)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= (float)delta;
    }

    /// <summary>
    /// Attempts to start an attack if the cooldown has expired.
    /// Triggers the "Attack" animation on the associated AnimationPlayer.
    /// </summary>
    public virtual void TryAttack()
    {
        if (!CanAttack)
            return;

        _cooldownTimer = Cooldown;
        _anim?.Play("Attack");
    }

    /// <summary>
    /// Applies damage to the given target.
    /// Typically called from the Attack animation at the impact frame.
    /// </summary>
    /// <param name="target">The Node2D being damaged (usually the player).</param>
    public virtual void DealDamage(Node2D target)
    {
        if (target is BaseCharacter character)
            character.TakeDamage(Damage);
    }
}