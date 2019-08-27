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
- [X] Physics:
    - [X] Raycast for voxel volumes;
    - [X] Collision for voxel volumes;
    - [X] Physic for voxel volumes;
- [X] Rendering:
    - [X] Chunk octa-frustum culling; (Glitched right now)
    - [X] Chunk frustum culling;
    - [X] Chunk loading near camera;
- [X] Saving and Loading of voxel volumes:
    - [X] .vcmap;
    - [X] .vcmod;
	- [X] .kv6;
	- [X] .vox;
    
## Installing
1. Copy VoxCake folder to your Unity project.
2. You don`t need the second step, youre ready to create great things!
3. You also don`t need the third step :D

## First steps
Well, i think that at first, you would see the fast results, dont you?
Okay, let`s create your first voxel volume in few lines of code! To make that you should:
1. Put map into StreammingAssets folder
2. Make GameObject and attach this script
```csharp
using VoxCake;
using UnityEngine;

public class MapExample : Volume
{
    private void Start()
    {
        LoadNearCamera("pathToYourMap", UColor.RGBAToUint(86, 93, 110, 100), Camera.main);
    }
    private void Update()
    {
        Render();
    }
}
```

## Load model
To load model in format .vox(for example) you should:
1. Put model into StreammingAssets/Models folder
2. Make GameObject with MeshFiler and MeshRenderer components and attach this script
```csharp
using VoxCake;
using UnityEngine;

public class ModelExample : Volume
{
    private void Start()
    {
		ResourceManager.Init();
        MaterialManager.Init();
        GetComponent<MeshFilter>().mesh = ResourceManager.GetModel("pathToYourModel", PlayerTeam.Green);
		GetComponent<MeshRenderer>().material = MaterialManager.model;
    }
}
```
### Showcase
[![Watch the video](https://steamuserimages-a.akamaihd.net/ugc/976613425704858920/E913B74E84C2C07921E35FD83EBB375A1CA17F51/?imw=1024&imh=576&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=true)](https://www.youtube.com/watch?v=nwWKZDr22ts)
