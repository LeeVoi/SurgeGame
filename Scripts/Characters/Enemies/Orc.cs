using Godot;
using Characters.Base.BaseEnemy;

namespace Characters.Enemies;
public partial class Orc : BaseEnemy
{
    public override void _Ready()
    {
        Speed = 120f;
        Health = 20;
        DetectionRange = 250f;
        base._Ready();
    }
}