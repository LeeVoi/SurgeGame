using Godot;
using Characters.Base.Components.CombatSystem.Interfaces;

namespace Characters.Base.Components.CombatSystem;

public partial class HurtBox : Area2D
{
    [Export] public NodePath OwnerPath;
    public Node2D _Owner { get; set; }

    public override void _Ready()
    {
        _Owner = GetNodeOrNull<Node2D>(OwnerPath) ?? GetParent<Node2D>();
    }

    public void RecivedHit(int damage, Node2D attacker)
    {
        if (_Owner is IDamageable damagable)
            damagable.TakeDamage(damage, attacker);
    }
}