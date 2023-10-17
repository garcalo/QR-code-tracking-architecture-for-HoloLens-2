// GAA

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

#if MIXED_REALITY_OPENXR
using Microsoft.MixedReality.OpenXR;
#else
using SpatialGraphNode = Microsoft.MixedReality.SampleQRCodes.WindowsXR.SpatialGraphNode; // Alias for SpatialGraphNode from WindowsXR namespace
#endif

namespace Microsoft.MixedReality.SampleQRCodes
{
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private SpatialGraphNode node; // Reference to a SpatialGraphNode object

        public System.Guid Id { get; set; } // Property to get/set the Id | GAA: method from Microsoft

        void Update()
        {
            if (node == null || node.Id != Id) // Check if the node is null or if the node's Id is different from the current Id
            {
                node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;  // If the Id is not empty, create a new SpatialGraphNode using the Id; otherwise, set the node to null
                Debug.Log("Initialize SpatialGraphNode Id= " + Id);
            }

            if (node != null) // Check if the node is not null
            {
#if MIXED_REALITY_OPENXR
                if (node.TryLocate(FrameTime.OnUpdate, out Pose pose)) // Try to locate the node's pose using the OpenXR framework | GAA: necesitamos esto para Holographic Remoting
#else
                if (node.TryLocate(out Pose pose)) // Try to locate the node's pose using the default framework (WindowsXR)
#endif
                {
                    // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                    // to these objects so apply the inverse
                    if (CameraCache.Main.transform.parent != null) // If there is a parent to the camera, it means teleportation is being used, so transform the pose accordingly
                    {
                        pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
                    }

                    gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation); // Set the position and rotation of the game object's transform based on the pose
                    Debug.Log("Id= " + Id + " QRPose = " + pose.position.ToString("F7") + " QRRot = " + pose.rotation.ToString("F7"));
                }
                else
                {
                    Debug.LogWarning("Cannot locate " + Id);
                }
            }
        }
    }
}