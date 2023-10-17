// GAA

using UnityEngine;

namespace Microsoft.MixedReality.SampleQRCodes.WindowsXR
{
    internal class SpatialGraphNode
    {
        public System.Guid Id { get; private set; }
#if WINDOWS_UWP && UNITY_XR_WINDOWSMR
        private Windows.Perception.Spatial.SpatialCoordinateSystem CoordinateSystem = null; // Reference to the Windows Perceptions SpatialCoordinateSystem
#endif

        public static SpatialGraphNode FromStaticNodeId(System.Guid id)
        {
#if WINDOWS_UWP && UNITY_XR_WINDOWSMR
            var coordinateSystem = Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(id); // Create a coordinate system for the specified node
            return coordinateSystem == null ? null :
                new SpatialGraphNode()
                {
                    Id = id,
                    CoordinateSystem = coordinateSystem
                };
#else
            return null;
#endif
        }


        public bool TryLocate(out Pose pose)
        {
            pose = Pose.identity; // Initialize pose to identity

#if WINDOWS_UWP && UNITY_XR_WINDOWSMR
            Quaternion rotation = Quaternion.identity;
            Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);

            System.IntPtr rootCoordnateSystemPtr = UnityEngine.XR.WindowsMR.WindowsMREnvironment.OriginSpatialCoordinateSystem; // Get the root spatial coordinate system
            Windows.Perception.Spatial.SpatialCoordinateSystem rootSpatialCoordinateSystem =
            Windows.Perception.Spatial.SpatialCoordinateSystem rootSpatialCoordinateSystem =
                (Windows.Perception.Spatial.SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(rootCoordnateSystemPtr); // Convert IntPtr to SpatialCoordinateSystem || before IntPtr object representing a handle to a spatial coordinate system, thanks to the conversion it now holds a reference to a SpatialCoordinateSystem object that can be used within the code

            // Get the relative transform from the unity origin
            System.Numerics.Matrix4x4? relativePose = CoordinateSystem.TryGetTransformTo(rootSpatialCoordinateSystem);  // Get the relative pose of the node

            if (relativePose != null)
            {
                System.Numerics.Vector3 scale;
                System.Numerics.Quaternion rotation1;
                System.Numerics.Vector3 translation1;

                System.Numerics.Matrix4x4 newMatrix = relativePose.Value; // Get the relative pose matrix

                // Platform coordinates are all right handed and unity uses left handed matrices. so we convert the matrix
                // from rhs-rhs to lhs-lhs 
                // Convert from right to left coordinate system
                newMatrix.M13 = -newMatrix.M13;
                newMatrix.M23 = -newMatrix.M23;
                newMatrix.M43 = -newMatrix.M43;

                newMatrix.M31 = -newMatrix.M31;
                newMatrix.M32 = -newMatrix.M32;
                newMatrix.M34 = -newMatrix.M34;

                System.Numerics.Matrix4x4.Decompose(newMatrix, out scale, out rotation1, out translation1); // Decompose the matrix into scale, rotation, and translation
                translation = new Vector3(translation1.X, translation1.Y, translation1.Z);  // Convert translation to Unity Vector3
                rotation = new Quaternion(rotation1.X, rotation1.Y, rotation1.Z, rotation1.W); // Convert rotation to Unity Quaternion || Convert rotation to Unity Quaternion
                pose = new Pose(translation, rotation); // Create a new Pose from translation and rotation
                return true;
            }
#endif
            return false;
        }
    }
}