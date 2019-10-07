# VoxCake
VoxCake is the framework which provides easy and efficiency work with voxels.
## Features
- [X] Handling of voxel volumes;
- [X] Voxel meshing:
    - [X] Culling algorithm with ambient occlusion and texturing;
- [X] Editing the voxel volumes
    - [X] Command buffer;
    - [X] Tools:
        - [X] Voxel;
	- [X] Voxel picker;
        - [X] Voxel line;
        - [X] Voxel box;
        - [X] Voxel sphere;
        - [ ] Voxel ellipse;
        - [ ] Voxel filling; 
	- [ ] Voxel painting;
- [X] Physics:
    - [X] Raycast for voxel volumes;
    - [X] Collision for voxel volumes;
    - [X] Physic for voxel volumes;
- [X] Rendering:
    - [X] Chunk octree frustum culling; 
    - [X] Chunk frustum culling; (Glitched right now)
    - [X] Chunk loading near camera;
- [X] Saving and Loading of voxel volumes:
    - [X] .vcmap;
    - [X] .vxl;
    - [X] .vcmod;
    - [X] .kv6;
    - [X] .vox;
    
## Installing
1. Copy VoxCake folder to your Unity project.
2. You don`t need the second step, you`re ready to create great things!
3. You also don`t need the third step :D

## First steps
Well, i think that at first, you would see the fast results, dont you?
Okay, let`s create your first voxel volume in few lines of code! To make that you should:
1. Download some map. (You could find it here: http://aos.party/)
2. Put map into StreamingAssets folder.
3. Make GameObject and attach this script:
```csharp
using VoxCake;
using UnityEngine;

public class MapExample : Volume
{
    [SerializedField] private Camera camera;
    
    private void Start()
    {
        string path = Application.streamingAssetsPath + "/mapname.vxl"; // CHANGE "mapname" to name of your map file!
        uint innerColor = UColor.RGBAToUint(86, 93, 110, 100);
        Load(path, innerColor, camera, VolumeFormat.vxl, LoadMode.Near);
    }
    private void Update()
    {
        Render(Camera, RenderMode.Native);
    }
}
```
4. Attach needed components to script.
4. Set map width to 512, height to 64, depth to 512.
5. Press play button!
## Load model
To load model in format .vox(for example) you should:
1. Put model into StreamingAssets folder.
2. Make GameObject with MeshFiler and MeshRenderer components and attach this script:
```csharp
using VoxCake;
using UnityEngine;

public class ModelExample
{
    [SerializedField] private MeshFilter meshFilter;
    [SerializedField] private MeshFilter meshRenderer;

    private void Start()
    {
        MaterialManager.Init();
	string path = Application.streamingAssetsPath + "/modelname.vox"; // CHANGE "modelname" to name of your model file!
        meshFilter.mesh = ModelMesh.Get(path, 0);
	meshRenderer.material = MaterialManager.model;
    }
}
```
3. Attach needed components to script.
3. Press play button!
### Showcase
[![Watch the video](https://steamuserimages-a.akamaihd.net/ugc/976613425704858920/E913B74E84C2C07921E35FD83EBB375A1CA17F51/?imw=1024&imh=576&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=true)](https://www.youtube.com/watch?v=nwWKZDr22ts)
