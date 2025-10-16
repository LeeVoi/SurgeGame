using Godot;

namespace Characters.Base.Components.CombatSystem.Interfaces;

public interface IDamageable
{
    void TakeDamage(int damage, Node2D attacker);
}