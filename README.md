# QRCode_tracking_in_HoloLens2
QR code tracking architecture developed for HoloLens 2. It requires the use of .NET Microsoft QR code libraries and Microsoft Mixed Reality Toolkit. Developed in Unity 2021.3.9f1. Compatible with the Holographic Remoting Player. 

## Instructions for using the QR code tracking Sample app
- Open the project using Unity 2021.3.9f1
- Using the MixedRealityFeatureTool (https://www.microsoft.com/en-us/download/details.aspx?id=102778) make sure you have installed the following packages:
   - Mixed Reality Toolkit Foundation - version 2.8.3
   - Mixed Reality Toolkit Standard Assets - version 2.8.3
   - Mixed Reality Toolkit Tools - version 2.8.3
   - Mixed Reality OpenXR Plugin - version 1.9.0
<img width="600" alt="Captura de pantalla 2023-10-17 122327" src="https://github.com/garcalo/QR-code-tracking-architecture-for-HoloLens-2/assets/133862204/a216ac5b-424d-413a-b16b-473251028767">
<img width="600" alt="platform support" src="https://github.com/garcalo/QR-code-tracking-architecture-for-HoloLens-2/assets/133862204/a0e48438-1fed-4089-af00-1b48a3f24471">


  
- Install NuGet for Unity following the repository instructions: https://github.com/GlitchEnzo/NuGetForUnity. I recommend the install via .unitypackage file
    - Inside Unity, use NuGet to install the Microsoft.MixedReality.QR. For this, click on the NuGet tab, then on Manage NuGet Packages. This way you will obtain a window where you can search for the package. This can be seen in the image below:
      
<img width="500" alt="install microsoft mixed reality qr from nuget" src="https://github.com/garcalo/QR-code-tracking-architecture-for-HoloLens-2/assets/133862204/32a02041-5721-413d-ad5f-6551f89761f2">

Once this is done, the QR code tracking app sample is ready to use. For testing it, the user can either deploy the app into the HoloLens 2 device building the project in Unity and deploying via Visual Studio, or use the Holographic Remoting tab in Unity to connect the device using an ip address and run the app in the computer and screencast the functionality into the HoloLens 2 device.

## QR code tracking architecture description
### Class Summaries

1. **Singleton.cs**
   - A template class in Unity for creating classes that can have only one instance at a time.
   - Manages a single instance of the class across scenes or creates one if none exists.
   - Simplifies Unity's singleton management and ensures only one instance is tracking QR codes.
   - Provides global access to other classes and efficient resource management.

2. **SpatialGraphNode.cs**
   - Handles spatial nodes in Windows Mixed Reality scenarios.
   - Creates instances based on unique node IDs.
   - Contains a 'TryLocate' method to identify the real-world location and orientation of spatial nodes within Unity's coordinate system.

3. **SpatialGraphNodeTracker.cs**
   - Records and synchronizes the pose of a specific spatial node in a Windows Mixed Reality environment.
   - Detects changes in the node's ID, locates its pose, and applies transformations to related Unity GameObjects.
   - Compatible with Microsoft's Mixed Reality OpenXR, making it compatible with Holographic Remoting Player.

4. **QRCodesVisualizer.cs**
   - Manages the visualization of QR codes in the Unity scene.
   - Instantiates and deletes QR code objects in response to QR code events received from the 'QRCodesManager' script.

5. **QRCodesManager.cs**
   - Handles QR code tracking in Unity and is defined as a singleton.
   - Initiates and manages QR code tracking events.
   - Maintains a sorted list of tracked QR codes and manages access permissions.
   - Provides methods for retrieving QR code information and can start tracking automatically.

6. **QRCodesSetup.cs**
   - Responsible for setting up and configuring QR code scanning in the Unity project.
   - Offers options for automatically starting QR code tracking and visualizing detected QR codes in 3D space.
   - Uses the 'QRCodesManager' class to access the QR code manager, initiate tracking, and add a 'QRCodesVisualizer' component if needed upon awakening.
  

## QR Code Tracking in Augmented Reality


![qrCodeScriptsDiagram-v2](https://github.com/garcalo/QR-code-tracking-architecture-for-HoloLens-2/assets/133862204/dbbbdf01-9f89-403e-8d29-56ab13935901)

As depicted in the diagram above, the QR code tracking process in this AR application is orchestrated as follows:

1. **Initialization**: Upon app launch, the AR application triggers the **QRCodesSetup** script. This script configures the QR code scanning system and decides whether to start tracking automatically.

2. **Manager Control**: The **QRCodesManager** class manages the QR code tracking process. It communicates with the device's hardware and initializes the tracking system, if supported.

3. **Visualizing Detected QR Codes**: Optionally, the **QRCodesSetup** script can add the **QRCodesVisualizer** component, responsible for displaying detected QR codes in 3D space.

4. **Spatial Node Tracking**: The **SpatialGraphNodeTracker** class continuously checks the pose of a specific spatial node representing a real-world QR code. It utilizes the OpenXR framework for accurate positioning and rotation, ensuring compatibility with Holographic Remoting.

5. **Simplified Spatial Node Interaction**: The **SpatialGraphNode** class simplifies interactions with spatial nodes, transforming their positions and rotations into Unity's coordinate system.

This integrated architecture enables the recognition and tracking of QR codes, facilitating the overlay of digital 3D models onto the real-world environment within an augmented reality application.


