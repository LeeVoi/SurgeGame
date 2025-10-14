using Godot;
/// <summary>
/// A bridge class to connect AnimationPlayer signals to C# events.
/// This allows for cleaner signal handling in C#.
/// </summary>
public partial class AnimationEnemyBridge : AnimationPlayer
{
    [Signal]
    public delegate void AttackHitEventHandler();
    
    /// <summary>
    /// purpose: To be called from an animation event to emit the AttackHit signal.
    /// This allows the enemy to signal when an attack should register a hit.
    /// Will allow for better timing of attack hit detection.
    /// Currently only partially used, signal is being emitted from the animation.
    /// But nothing is done to time the damage application to a specific frame.
    /// </summary>
    public void EmitAttackHit()
    {
        EmitSignal(SignalName.AttackHit);
    }
}