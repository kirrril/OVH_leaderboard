// using UnityEngine;
// using Unity.Behavior;

// public class Move : Action
// {
//     public BlackboardVariable<GameObject> manAgent;

//     protected override Status OnStart()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return Status.Failure;
//         manController.MoveToTarget();
//         return Status.Success;
//     }
// }