// using UnityEngine;
// using Unity.Behavior;

// public class StopChasingCondition : Condition
// {
//     public BlackboardVariable<GameObject> manAgent;

//     public override bool IsTrue()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return false;
//         var player = manController.playerController.transform;
//         return Vector3.Distance(manController.transform.position, player.position) > 3f;
//     }
// }