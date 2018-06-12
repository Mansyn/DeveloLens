using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

public class ObjectCollectionManager : Singleton<ObjectCollectionManager>
{

    [Tooltip("A collection of square object prefabs to generate in the world.")]
    public List<GameObject> SquareObjPrefabs;

    [Tooltip("The desired size of square objects in the world.")]
    public Vector3 SquareObjSize = new Vector3(.5f, .5f, .5f);

    [Tooltip("A collection of Wide objects prefabs to generate in the world.")]
    public List<GameObject> WideObjPrefabs;

    [Tooltip("The desired size of wide objects in the world.")]
    public Vector3 WideObjSize = new Vector3(1.0f, .5f, .5f);

    [Tooltip("A collection of tall object prefabs to generate in the world.")]
    public List<GameObject> TallObjPrefabs;

    [Tooltip("The desired size of tall objects in the world.")]
    public Vector3 TallObjSize = new Vector3(.25f, .05f, .25f);

    [Tooltip("A collection of plant prefabs to generate in the world.")]
    public List<GameObject> PlantPrefabs;

    [Tooltip("The desired size of plants in the world.")]
    public Vector3 PlantSize = new Vector3(.25f, .5f, .25f);

    [Tooltip("Will be calculated at runtime if is not preset.")]
    public float ScaleFactor;

    public List<GameObject> ActiveHolograms = new List<GameObject>();

    public void CreateSquareObject(int number, Vector3 positionCenter, Quaternion rotation)
    {
        CreateObject(SquareObjPrefabs[number], positionCenter, rotation, SquareObjSize);
    }

    public void CreateTallObject(int number, Vector3 positionCenter, Quaternion rotation)
    {
        CreateObject(TallObjPrefabs[number], positionCenter, rotation, TallObjSize);
    }

    public void CreateWideObject(int number, Vector3 positionCenter, Quaternion rotation)
    {
        CreateObject(WideObjPrefabs[number], positionCenter, rotation, WideObjSize);
    }

    private void CreateObject(GameObject objectToCreate, Vector3 positionCenter, Quaternion rotation, Vector3 desiredSize)
    {
        // Stay center in the square but move down to the ground
        var position = positionCenter - new Vector3(0, desiredSize.y * .5f, 0);

        GameObject newObject = Instantiate(objectToCreate, position, rotation);

        if (newObject != null)
        {
            // Set the parent of the new object the GameObject it was placed on
            newObject.transform.parent = gameObject.transform;

            newObject.transform.localScale = RescaleToSameScaleFactor(objectToCreate);
            AddMeshColliderToAllChildren(newObject);
            ActiveHolograms.Add(newObject);
        }
    }

    public void CreateTree(int number, Vector3 positionCenter, Quaternion rotation)
    {
        // Stay center in the square but move down to the ground
        var position = positionCenter - new Vector3(0, PlantSize.y * .5f, 0);

        GameObject newObject = Instantiate(PlantPrefabs[number], position, rotation);

        if (newObject != null)
        {
            // Set the parent of the new object the GameObject it was placed on
            newObject.transform.parent = gameObject.transform;

            newObject.transform.localScale = RescaleToSameScaleFactor(PlantPrefabs[number]);
            newObject.AddComponent<MeshCollider>();
            ActiveHolograms.Add(newObject);
        }
    }

    private void AddMeshColliderToAllChildren(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
        }
    }

    private Vector3 RescaleToSameScaleFactor(GameObject objectToScale)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (ScaleFactor == 0f)
        {
            CalculateScaleFactor();
        }

        return objectToScale.transform.localScale * ScaleFactor;
    }

    private Vector3 StretchToFit(GameObject obj, Vector3 desiredSize)
    {
        var curBounds = GetBoundsForAllChildren(obj).size;

        return new Vector3(desiredSize.x / curBounds.x / 2, desiredSize.y, desiredSize.z / curBounds.z / 2);
    }

    private void CalculateScaleFactor()
    {
        float maxScale = float.MaxValue;

        var ratio = CalcScaleFactorHelper(WideObjPrefabs, WideObjSize);
        if (ratio < maxScale)
        {
            maxScale = ratio;
        }

        ScaleFactor = maxScale;
    }

    private float CalcScaleFactorHelper(List<GameObject> objects, Vector3 desiredSize)
    {
        float maxScale = float.MaxValue;

        foreach (var obj in objects)
        {
            var curBounds = GetBoundsForAllChildren(obj).size;
            var difference = curBounds - desiredSize;

            float ratio;

            if (difference.x > difference.y && difference.x > difference.z)
            {
                ratio = desiredSize.x / curBounds.x;
            }
            else if (difference.y > difference.x && difference.y > difference.z)
            {
                ratio = desiredSize.y / curBounds.y;
            }
            else
            {
                ratio = desiredSize.z / curBounds.z;
            }

            if (ratio < maxScale)
            {
                maxScale = ratio;
            }
        }

        return maxScale;
    }

    private Bounds GetBoundsForAllChildren(GameObject findMyBounds)
    {
        Bounds result = new Bounds(Vector3.zero, Vector3.zero);

        foreach (var curRenderer in findMyBounds.GetComponentsInChildren<Renderer>())
        {
            if (result.extents == Vector3.zero)
            {
                result = curRenderer.bounds;
            }
            else
            {
                result.Encapsulate(curRenderer.bounds);
            }
        }

        return result;
    }
}