// using UnityEngine;
// using Unity.Behavior;

// public class Interact : Action
// {
//     public BlackboardVariable<GameObject> manAgent;

//     protected override Status OnStart()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         var playerController = manController.playerController;
//         if (manController.hasInteracted) return Status.Failure;
//         manController.hasInteracted = true;
//         manController.agent.ResetPath();
//         return Status.Success;
//     }
// }