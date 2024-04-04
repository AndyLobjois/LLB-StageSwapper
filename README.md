Swap stages with custom Unity scenes in Lethal League Blaze.<br>
Example with the Hive Underground: [https://github.com/AndyLobjois/Custom-Stages/tree/main/Hive Underground](https://github.com/AndyLobjois/Custom-Stages/tree/main/Hive%20Underground)

```
StageSwapper is originally made by Amanda and she gave me permission for taking care of it. Thanks a lot ! ♥
```

# How to install a custom stage ?

- Launch the game once, **Stage Swapper** folder will be created automatically.
- Find it inside your Lethal League Blaze installation folder : `.../LLBlaze/ModdingFolder/Stage Swapper`
- Inside Stage Swapper folder, place the **Custom** **Stage File*** and **the XML file** !**
- Stage file’s name indicate which stage to replace (see Bundles/Stages folder for the names)

(*) Custom Stage files are “AssetBundles” used by Unity and have no formats.<br>
(**) .xml file gather specific informations for the stage (shadow distance, camera introductions, …)

# How to create a custom stage ?

### <ins>Setup</ins>

- Install [Unity 2018.4.3](https://unity.com/releases/editor/whats-new/2018.4.3).
- Create a **new project.**
- Drag & drop [Tools.unitypackage](https://github.com/AndyLobjois/LLB-BlenderReadyModels/blob/master/Characters/(Tools)/Tools.unitypackage) inside Unity.

### <ins>Create</ins>

- In the main scene, create these **3 GameObjects (case sensitive) :**
    - StageSwapperAlways
    - StageSwapperNormal
    - StageSwapperEclipse
- Import your assets into Unity
	- There is too much to cover about importing assets into Unity, check youtube tutorials :)
- Put every objects inside **StageSwapperAlways** that is not related to **Eclipse Mode.**
    - Like the ground, walls, environments, props, etc …
- Put objects inside **StageSwapperNormal / StageSwapperEclipse** only if they are related to **Eclipse Mode.**
    - StageSwapperNormal is the normal state of the stage when the ball is under Eclipse Mode speed.
    - StageSwapperEclipse is the “eclipse” state of the stage when the ball is above Eclipse Mode speed.
    - The game toggle their visibility depending on the ball speed, triggered by Eclipse Mode.
    - I recommend to put lights in these instead of StageSwapperAlways and avoid overlapping lights.
- For specific stages, you’ll need to create a custom ReceiveShadowsPlane, name that object `CustomReceiveShadowsPlane`
    - Place that object in the scene to the correct position/rotation/scale
    - Make sure this gameObject is active

### <ins>Build</ins>

- In the **Project Viewport**, find and select your scene : **NameOfYourScene.unity**
- Check the bottom of the **Inspector Viewport** and assign an **AssetBundle Tag** to your scene with one of the stage name :
    - *construction, factory, junktown, outskirts, pool, room21, sewers, stadium, streets, subway*
- In the Project viewport, **create a folder named `Bundle`** to the root (Assets/Bundle)
- Build your bundle with **File > Build AssetBundles**
    - Your fresh bundle will be in Assets/Bundle folder
- Configure your custom .xml file for controlling specific features of the stage :
    
    ```xml
    <StageSettings>
    	<ShadowDistance>58</ShadowDistance>
    	<CameraPositionY>2</CameraPositionY>
    	<NormalObjects>
    		<string>StageSwapperNormal</string>
    	</NormalObjects>
    	<EclipseObjects>
    		<string>StageSwapperEclipse</string>
    	</EclipseObjects>
    	<IntroCamShots>
    		<CamShot>
    			<StartCam>
    				<x>3.77</x>
    				<y>1.147</y>
    				<z>-0.21965</z>
    				<rotX>-0.136</rotX>
    				<rotY>35.408</rotY>
    				<rotZ>0</rotZ>
    			</StartCam>
    			<EndCam>
    				<x>2.607</x>
    				<y>1.147</y>
    				<z>0.583</z>
    				<rotX>-0.136</rotX>
    				<rotY>35.408</rotY>
    				<rotZ>0</rotZ>
    			</EndCam>
    			<Duration>1.8</Duration>
    			<Smooth>false</Smooth>
    		</CamShot>
    	</IntroCamShots>
    </StageSettings>
    ```
    
    - In <NormalObjects> and <EclipseObjects> tags, you can rename the <string> tags → `<string>CustomName</string>`
    - You can add several <string> tag inside <NormalObjects> and <EclipseObjects> if you want.
    - You can add several <CamShot> if you want.

### <ins>Tips</ins>

- Delete anything outside the Always/Normal/Eclipse gameobjects (except CustomReceiveShadowsPlane). Don’t bloat your scene.
- Remove any camera or audio listener from the scene, it’ll highjack the game.
- I recommend to use Directional Light instead of other types for perfomance reasons.
    - You can use Dynamic Shadows too for easier lighting.
    - You can bake your lights if you know how to manage that.
- I recommend to use only 1 light at a time for performance reasons.
    - *Example : one in StageSwapperNormal and another one in StageSwapperEclipse, they will be toggle by Eclipse Mode*
- Easy process for getting Camera Intro Shots :
    - Create a camera
    - Put the camera at the start of your “shot”
    - Copy/Paste the position and rotation inside your custom XML file
    - Repeat the process for the end of your “shot”
    - Don’t forget to delete the camera before building