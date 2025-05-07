using System.Collections;
using UnityEngine;

namespace Command
{
    public class AttackCommand : ICommand 
    {
        private Animator _animator;
        
        public IEnumerator Execute()
        {
            _animator.Play("Attack");
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        }

        public IEnumerator Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}