using Characters.Base.BaseEnemy;
using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
    [Export] public PackedScene EnemyScene;
    [Export] public int EnemiesPerWave = 3;
    [Export] public float SpawnRadius = 200f;
    [Export] public float TimeBetweenSpawns = 1.0f;

    private Node2D _player;
    private Timer _spawnTimer;
    private List<BaseEnemy> _activeEnemies = new();

    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<Node2D>("PlayerWarrior");

        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = TimeBetweenSpawns;
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
        AddChild(_spawnTimer);

        _spawnTimer.Start();
    }

    private void OnSpawnTimerTimeout()
    {
        if (EnemyScene == null || _player == null)
            return;

        for (int i = 0; i < EnemiesPerWave; i++)
        {
            var enemy = EnemyScene.Instantiate<BaseEnemy>();
            enemy.Position = _player.Position + GetRandomOffset(SpawnRadius);
            enemy.Initialize(_player);
            GetParent().AddChild(enemy);
            _activeEnemies.Add(enemy);
        }
    }

    private Vector2 GetRandomOffset(float radius)
    {
        return new Vector2(
            (float)GD.RandRange(-radius, radius),
            (float)GD.RandRange(-radius, radius)
        );
    }

    // Optional: clean dead enemies from the list
    public override void _Process(double delta)
    {
        _activeEnemies.RemoveAll(e => !IsInstanceValid(e));
    }
}
