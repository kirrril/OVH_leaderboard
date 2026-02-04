// using UnityEngine;
// using Unity.Behavior;

// public class Attack : Action
// {
//     public BlackboardVariable<GameObject> manAgent;

//     protected override Status OnStart()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return Status.Failure;
//         manController.DoAttack();
//         return Status.Success;
//     }
// }