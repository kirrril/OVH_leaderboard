// using UnityEngine;
// using Unity.Behavior;

// public class HighScoreCondition : Condition
// {
//     public BlackboardVariable<GameObject> manAgent;

//     public override bool IsTrue()
//     {
//         var controller = manAgent.Value.GetComponent<ManController>();
//         if (controller == null) return false;
//         return controller.playerController.score > 50;
//     }
// }