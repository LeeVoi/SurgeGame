using System;
using Godot;

namespace Characters.Base.Components.CombatSystem;

public partial class HitBox : Area2D
{
    [Export] public NodePath OwnerPath;
    private Node2D _Owner;

    // Will be emitted when this hitbox overlaps a hurtbox.
    public event Action<HurtBox> HitDetected;

    public override void _Ready()
    {
        _Owner = GetNodeOrNull<Node2D>(OwnerPath) ?? GetParent<Node2D>();
        AreaEntered += OnAreaEntered;
        Monitoring = false;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is HurtBox hurtBox && area.GetParent() != _Owner)
            HitDetected?.Invoke(hurtBox);
    }

    public void Activate() => Monitoring = true;
    public void Deactivate() => Monitoring = false;
}