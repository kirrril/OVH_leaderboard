// using UnityEngine;
// using Unity.Behavior;

// public class Insult : Action
// {
//     public BlackboardVariable<GameObject> manAgent;
//     private float timer;

//     protected override Status OnStart()
//     {
//         timer = 0f;
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return Status.Failure;
//         manController.DoInsult();
//         return Status.Running;
//     }

//     protected override Status OnUpdate()
//     {
//         timer += Time.deltaTime;
//         if (timer >= 0.1f) return Status.Success;
//         return Status.Running;
//     }
// }
