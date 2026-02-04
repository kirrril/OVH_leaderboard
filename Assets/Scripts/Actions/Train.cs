// using UnityEngine;
// using Unity.Behavior;

// public class Train : Action
// {
//     public BlackboardVariable<GameObject> manAgent;
//     private float timer;
//     private float duration;

//     protected override Status OnStart()
//     {
//         timer = 0f;
//         var manController = manAgent.Value.GetComponent<ManController>();
//         if (manController == null) return Status.Failure;
//         duration = manController.duration;
//         manController.StartTraining();
//         return Status.Running;
//     }

//     protected override Status OnUpdate()
//     {
//         timer += Time.deltaTime;
//         if (timer >= duration) return Status.Success;
//         return Status.Running;
//     }

//     protected override void OnEnd()
//     {
//         var manController = manAgent.Value.GetComponent<ManController>();
//         manController.StopTraining();
//     }
// }