// using UnityEngine;
// using Unity.Behavior;

// public class Chase : Action
// {
//     public BlackboardVariable<GameObject> manAgent;

//     protected override Status OnUpdate()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return Status.Failure;
//         manController.Chase();
//         return Status.Running;
//     }
// }