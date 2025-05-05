using UnityEngine;
using System.Collections;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    private int currentAnimation = 0;
    private bool isTransitioning = false;
    
    [Header("Animation Settings")]
  
    public float extraWaitTime = 0.5f;
    
    private readonly string[] animationNames = { "Idle", "Dancing", "Greet" };
    private readonly string animStateParam = "AnimationState";
    
    void Start()
    {
        animator = GetComponent<Animator>();
     
        currentAnimation = 0;
        animator.SetInteger(animStateParam, currentAnimation);
        
      
        StartCoroutine(AnimationSequenceController());
    }
    
    IEnumerator AnimationSequenceController()
    {
        while (true)
        {
         
            if (!isTransitioning)
            {
             
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                
              
                if (stateInfo.normalizedTime >= 0.95f)
                {
                    
                    yield return new WaitForSeconds(extraWaitTime);
                    
                
                    PlayNextAnimation();
                }
            }
            
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void PlayNextAnimation()
    {
        isTransitioning = true;
        
       
        currentAnimation = (currentAnimation + 1) % animationNames.Length;
        
        
        animator.SetInteger(animStateParam, currentAnimation);
        
        
        StartCoroutine(ResetTransitionFlag());
    }
    
    IEnumerator ResetTransitionFlag()
    {
       
        yield return null;
        isTransitioning = false;
    }
    
   
    public void SetAnimation(int animationIndex)
    {
        if (animationIndex >= 0 && animationIndex < animationNames.Length)
        {
            isTransitioning = true;
            currentAnimation = animationIndex;
            animator.SetInteger(animStateParam, currentAnimation);
            StartCoroutine(ResetTransitionFlag());
        }
    }
}