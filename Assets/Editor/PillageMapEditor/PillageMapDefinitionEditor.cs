#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PillageMapDefinition))]
public class PillageMapDefinitionEditor : Editor
{
    private SerializedProperty regionsProperty;
    private SerializedProperty objectsProperty;
    private string typeFilter = string.Empty;
    private int selectedIndex = -1;

    private void OnEnable()
    {
        regionsProperty = serializedObject.FindProperty("regions");
        objectsProperty = serializedObject.FindProperty("objects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Pillage Map Definition", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(regionsProperty, true);

        EditorGUILayout.Space(8f);
        EditorGUILayout.BeginHorizontal();
        typeFilter = EditorGUILayout.TextField("Object Filter", typeFilter);
        if (GUILayout.Button("Clear", GUILayout.Width(58f)))
        {
            typeFilter = string.Empty;
        }
        EditorGUILayout.EndHorizontal();

        DrawObjectList();

        if (GUILayout.Button("Rebuild Lookup"))
        {
            ((PillageMapDefinition)target).RebuildLookup();
            EditorUtility.SetDirty(target);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawObjectList()
    {
        EditorGUILayout.LabelField("Objects", EditorStyles.boldLabel);

        for (int i = 0; i < objectsProperty.arraySize; i++)
        {
            SerializedProperty element = objectsProperty.GetArrayElementAtIndex(i);
            SerializedProperty id = element.FindPropertyRelative("id");
            SerializedProperty type = element.FindPropertyRelative("type");

            if (!string.IsNullOrWhiteSpace(typeFilter)
                && !id.stringValue.ToLowerInvariant().Contains(typeFilter.ToLowerInvariant())
                && !type.enumDisplayNames[type.enumValueIndex].ToLowerInvariant().Contains(typeFilter.ToLowerInvariant()))
            {
                continue;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            bool isSelected = selectedIndex == i;
            if (GUILayout.Toggle(isSelected, isSelected ? "Selected" : "Select", "Button", GUILayout.Width(76f)) != isSelected)
            {
                selectedIndex = isSelected ? -1 : i;
                SceneView.RepaintAll();
            }

            EditorGUILayout.LabelField($"{id.stringValue} ({type.enumDisplayNames[type.enumValueIndex]})", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (selectedIndex == i)
            {
                EditorGUILayout.PropertyField(element, true);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element.FindPropertyRelative("enabled"), GUIContent.none, GUILayout.Width(24f));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("position"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void OnSceneGUI()
    {
        PillageMapDefinition definition = (PillageMapDefinition)target;
        if (definition == null || selectedIndex < 0 || selectedIndex >= definition.Objects.Count)
        {
            return;
        }

        MapObjectPlacement placement = definition.Objects[selectedIndex];
        if (placement == null)
        {
            return;
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newPosition = Handles.PositionHandle(placement.Position, Quaternion.identity);
        float handleSize = HandleUtility.GetHandleSize(newPosition) * 0.8f;
        Vector3 newScale = Handles.ScaleHandle(placement.Scale, newPosition + Vector3.up * handleSize, Quaternion.identity, handleSize);

        Handles.Label(newPosition + Vector3.up * handleSize * 1.45f, placement.Id);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(definition, "Edit Map Object Placement");
            placement.Position = newPosition;
            placement.Scale = newScale;
            definition.RebuildLookup();
            EditorUtility.SetDirty(definition);
        }
    }

    [MenuItem("Pillage/Map Editor/Generate Default Map Definition")]
    private static void GenerateDefaultMapDefinition()
    {
        const string directory = "Assets/map asset/MapDefinitions";
        const string assetPath = directory + "/DefaultPillageMap.asset";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        PillageMapDefinition definition = AssetDatabase.LoadAssetAtPath<PillageMapDefinition>(assetPath);
        if (definition == null)
        {
            definition = CreateInstance<PillageMapDefinition>();
            AssetDatabase.CreateAsset(definition, assetPath);
        }

        AddRegion(definition, "Village", Vector3.zero, new Vector2(70f, 48f));
        AddRegion(definition, "Forest", new Vector3(70f, 0f, 0f), new Vector2(70f, 48f));
        AddRegion(definition, "DeepForest", new Vector3(122.5f, 0f, 0f), new Vector2(35f, 48f));
        AddRegion(definition, "FourthForest", new Vector3(175f, 0f, 0f), new Vector2(70f, 48f));

        AddObject(definition, "GeneratedOutsideHouseLineDrawing", MapObjectType.House, "Village", new Vector3(0f, 2f, 0f), Vector3.one, 20, 0f, 0);
        AddObject(definition, "GeneratedOutsideHouseDoor", MapObjectType.Door, "Village", new Vector3(0f, 1.35f, 0f), Vector3.one, 20, 1.6f, 0);
        AddObject(definition, "GeneratedNeighborHouseWithMailbox", MapObjectType.NeighborHouse, "Village", new Vector3(18.4f, 3.1f, 0f), Vector3.one, 20, 0f, 0);
        AddObject(definition, "GeneratedNeighborMailbox", MapObjectType.Mailbox, "Village", new Vector3(20.55f, 0.85f, 0f), new Vector3(0.52f, 0.52f, 1f), 24, 0f, 0, "Outside/mailbox_thick");
        AddObject(definition, "GeneratedQuestGiverNpcLineDrawing", MapObjectType.QuestNpc, "Village", new Vector3(14.83f, -0.06f, 0f), Vector3.one, 25, 2.3f, 70);
        AddObject(definition, "GeneratedForestEntranceLineDrawing", MapObjectType.ForestEntrance, "Village", new Vector3(21.5f, -14.2f, 0f), Vector3.one, 25, 0f, 0);
        AddObject(definition, "GeneratedPondLineDrawing", MapObjectType.Pond, "Village", new Vector3(-23.5f, -13.6f, 0f), Vector3.one, 22, 0f, 0);
        AddObject(definition, "GeneratedPondCollider", MapObjectType.BoundaryCollider, "Village", new Vector3(-23.5f, -13.6f, 0f), Vector3.one, 22, 0f, 0);
        AddObject(definition, "GeneratedFishingRodLineDrawing", MapObjectType.FishingRod, "Village", new Vector3(-18.95f, -12.55f, 0f), Vector3.one, 25, 2.35f, 70);
        AddObject(definition, "GeneratedLetterFragmentEvent_WindTrace", MapObjectType.LetterFragment, "Village", new Vector3(-13.4f, 12.2f, 0f), Vector3.one, 26, 1.45f, 72);
        AddObject(definition, "GeneratedLetterFragmentEvent_GrassPatch", MapObjectType.LetterFragment, "Village", new Vector3(8.8f, -7.8f, 0f), Vector3.one, 26, 1.45f, 72);
        AddObject(definition, "GeneratedLetterFragmentEvent_PondEdge", MapObjectType.LetterFragment, "Village", new Vector3(-14.7f, -19.2f, 0f), Vector3.one, 26, 1.45f, 72);
        AddObject(definition, "GeneratedForestWellLineDrawing", MapObjectType.Well, "Forest", new Vector3(80.6f, -7.8f, 0f), Vector3.one, 24, 1.9f, 68);
        AddObject(definition, "GeneratedForestSmallBirdLineDrawing", MapObjectType.Bird, "Forest", new Vector3(70f, 0f, 0f), Vector3.one, 27, 2.15f, 72);
        AddObject(definition, "GeneratedForestFallenBirdLineDrawing", MapObjectType.FallenBird, "Forest", new Vector3(42.2f, -17.6f, 0f), Vector3.one, 27, 2.15f, 72);
        AddObject(definition, "GeneratedDeepForestSquirrelLineDrawing", MapObjectType.Squirrel, "DeepForest", new Vector3(110.9f, 0.35f, 0f), Vector3.one, 28, 1.85f, 73);
        AddObject(definition, "GeneratedDeepForestHungrySquirrelLineDrawing", MapObjectType.HungrySquirrel, "DeepForest", new Vector3(130.3f, -5.8f, 0f), Vector3.one, 29, 1.85f, 74);
        AddObject(definition, "GeneratedFourthForestBigTreeLineDrawing", MapObjectType.BigTree, "FourthForest", new Vector3(175f, -3.2f, 0f), Vector3.one, 23, 6.2f, 77);
        AddObject(definition, "GeneratedFourthForestRiver", MapObjectType.River, "FourthForest", new Vector3(201.6f, -15.2f, 0f), Vector3.one, 9, 3.6f, 54);
        AddObject(definition, "GeneratedFourthForestIslandDoor", MapObjectType.IslandDoor, "FourthForest", new Vector3(201.6f, -15.2f, 0f), Vector3.one, 16, 1.9f, 61);

        definition.RebuildLookup();
        EditorUtility.SetDirty(definition);
        AssetDatabase.SaveAssets();
        Selection.activeObject = definition;
        Debug.Log($"Generated map definition at {assetPath}. Random decorations will be recorded into this asset the next time the outside map is generated in Play Mode.");
    }

    private static void AddRegion(PillageMapDefinition definition, string id, Vector3 offset, Vector2 size)
    {
        for (int i = 0; i < definition.Regions.Count; i++)
        {
            if (definition.Regions[i].Id == id)
            {
                definition.Regions[i].Offset = offset;
                definition.Regions[i].Size = size;
                return;
            }
        }

        definition.Regions.Add(new MapRegionDefinition
        {
            Id = id,
            Offset = offset,
            Size = size
        });
    }

    private static void AddObject(
        PillageMapDefinition definition,
        string id,
        MapObjectType type,
        string region,
        Vector3 position,
        Vector3 scale,
        int sortingOrder,
        float promptDistance,
        int promptSortingOrder,
        string resourcePath = null)
    {
        MapObjectPlacement placement = definition.GetOrAddObject(id, type);
        placement.Region = region;
        placement.Position = position;
        placement.Scale = scale;
        placement.SortingOrder = sortingOrder;
        placement.PromptDistance = promptDistance;
        placement.PromptSortingOrder = promptSortingOrder;
        placement.LineWidthMultiplier = Mathf.Approximately(placement.LineWidthMultiplier, 0f) ? 1f : placement.LineWidthMultiplier;
        placement.ResourcePath = resourcePath;
    }
}
#endif
