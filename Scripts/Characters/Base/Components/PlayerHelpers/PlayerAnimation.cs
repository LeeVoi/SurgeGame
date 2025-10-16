using Characters.Entities.CharacterState;
using Godot;

namespace Characters.Base.Components.PlayerHelpers;

public class PlayerAnimation
{
    // Defining Variables 
    private readonly BaseCharacter _player;
    public AnimatedSprite2D animatedSprite { get; private set; }
    public CharacterState CurrentState  {get; private set;} = CharacterState.Idle;
    private bool attackLocked = false;

    public PlayerAnimation(BaseCharacter player)
    {
        _player = player;
        animatedSprite = player.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.AnimationFinished += OnAnimationFinished;
    }

    public void UpdateAnimation()
    {
        switch (CurrentState)
        {
            case CharacterState.Idle:
                PlayIfNot("Idle");
                break;
            case CharacterState.Moving:
                PlayIfNot("Run");
                break;
            case CharacterState.AttackNormal:
                PlayIfNot("AttackNormal");
                break;
            case CharacterState.AttackHeavy:
                PlayIfNot("AttackHeavy");
                break;
            case CharacterState.SpecialAbility:
                _player.UseAbility();
                break;
        }
        
    }

    public void UpdateInput()
    {
        if (attackLocked)
            return;
        
        if (Input.IsActionPressed("AttackNormal") && _player.CanAttack(10f))
        {
            StartAttack(CharacterState.AttackNormal,10f);
            
        }
        else if (Input.IsActionPressed("AttackHeavy") && _player.CanAttack(20f))
        {
            StartAttack(CharacterState.AttackHeavy,20f);
        }
        else if (Input.IsActionPressed("SpecialAbility"))
        {
            // handled by subclass
            _player.HandleAbilityInput();
        }
        else
        {
           UpdateMovementState();
        }
    }
    

    private void OnAnimationFinished()
    {
        if (animatedSprite.Animation == "AttackNormal" || animatedSprite.Animation == "AttackHeavy")
        {
            ResetToIdleOrMove();
            attackLocked  = false;
        }
    }
    
// ´------------------Movement & State ---------------------
    private void ResetToIdleOrMove()
    {
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        CurrentState = direction != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle;
        PlayIfNot(CurrentState == CharacterState.Moving ? "Run" : "Idle");
    }
    
    
    // -------------- HelperS ----------------
    private void PlayIfNot(string animationName)
    {
        if (animatedSprite.Animation != animationName)
        {
            animatedSprite.Play(animationName);
        }
    }

    private void UpdateMovementState()
    {
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (CurrentState != CharacterState.AttackNormal &&
            CurrentState != CharacterState.AttackHeavy &&
            CurrentState != CharacterState.SpecialAbility)
        {
            CurrentState = direction != Vector2.Zero ? CharacterState.Moving : CharacterState.Idle;
        }
    }

    private void StartAttack(CharacterState attackType, float staminaCost)
    {
        _player.SpendStamina(staminaCost);
        CurrentState = attackType;
        attackLocked = true;
    }
    
    // Get and Set the state of the player
    public void SetState(CharacterState newState) => CurrentState = newState;

    public CharacterState GetState() => CurrentState;
}