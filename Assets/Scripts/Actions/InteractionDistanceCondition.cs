// using UnityEngine;
// using Unity.Behavior;

// public class InteractionDistanceCondition : Condition
// {
//     public BlackboardVariable<GameObject> manAgent;

//     public override bool IsTrue()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return false;
//         var playerTransform = manController.playerController.gameObject.transform;

//         return Vector3.Distance(manController.transform.position, playerTransform.position) < 0.8f;
//     }
// }