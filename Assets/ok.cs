using Unity.VisualScripting;
using UnityEngine;

public class ok : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(this);
    }
}
