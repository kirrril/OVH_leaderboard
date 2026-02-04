// using UnityEngine;
// using Unity.Behavior;

// public class PlayerFreeToAttackCondition : Condition
// {
//     public BlackboardVariable<GameObject> manAgent;

//     public override bool IsTrue()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return false;
//         return !manController.playerController.isBeingAttacked;
//     }
// }