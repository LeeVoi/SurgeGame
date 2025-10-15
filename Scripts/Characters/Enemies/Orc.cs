using Godot;
using Characters.Base;

namespace Characters.Enemies;
public partial class Orc : BaseEnemy
{
    public override void _Ready()
    {
        base._Ready();
        Speed = 120f;
        Health = 20;
        AttackDamage = 5;
        AttackRange = 20f;
    }
}