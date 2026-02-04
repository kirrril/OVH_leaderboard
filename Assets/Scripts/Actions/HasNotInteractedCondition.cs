// using UnityEngine;
// using Unity.Behavior;

// public class HasNotInteractedCondition : Condition
// {
//     public BlackboardVariable<GameObject> manAgent;

//     public override bool IsTrue()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return false;
//         return !manController.hasInteracted;
//     }
// }