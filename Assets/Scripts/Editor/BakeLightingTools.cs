using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class BakeLightingTools
{
    private const string RootMenu = "Tools/Lighting/";
    private const float SmallGiMaxDimension = 0.75f;
    private const float MaxReasonableLightIntensity = 250000f;
    private const float MaxReasonableBounceIntensity = 10f;
    private const float MaxReasonableEmissionIntensity = 25f;
    private const float MaxReasonableWorldCoordinate = 250000f;
    private const float MinReasonableScaleMagnitude = 0.0001f;
    private const float MaxReasonableScaleMagnitude = 10000f;
    private const float MaxReasonableBoundsExtent = 250000f;
    private const float MaxReasonableVertexMagnitude = 250000f;
    private const float MinReasonableNormalMagnitudeSqr = 0.000001f;
    private const float MinReasonableTangentMagnitudeSqr = 0.000001f;
    private const float MaxDegenerateTriangleRatio = 0.35f;
    private const int MaxDetailedIssuesInDialog = 8;

    [MenuItem(RootMenu + "Analyze Active Scene For Bake")]
    public static void AnalyzeActiveSceneForBake()
    {
        BakeAudit audit = RunAudit(applyFixes: false);
        string report = BuildReport(audit);
        Debug.Log(report);
        EditorUtility.DisplayDialog("Bake Analysis", BuildDialogSummary(audit), "OK");
    }

    [MenuItem(RootMenu + "Auto-Setup GI For Active Scene")]
    public static void AutoSetupGiForActiveScene()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Auto-Setup GI",
            "This will mark likely-static renderers as Contribute GI in the active scene. Continue?",
            "Apply",
            "Cancel");

        if (!confirmed) return;

        BakeAudit audit = RunAudit(applyFixes: true);
        string report = BuildReport(audit);
        Debug.Log(report);
        EditorUtility.DisplayDialog("Auto-Setup Complete", BuildDialogSummary(audit), "OK");
    }

    [MenuItem(RootMenu + "Bake Active Scene Lighting")]
    public static void BakeActiveSceneLighting()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Lightmapping.BakeAsync();
        Debug.Log("[BakeLightingTools] Started Lightmapping.BakeAsync() for active scene.");
    }

    [MenuItem(RootMenu + "Prepare Scene For Low-Memory Bake")]
    public static void PrepareSceneForLowMemoryBake()
    {
        PrepareSceneForLowMemoryBakeInternal();
    }

    private static bool PrepareSceneForLowMemoryBakeInternal()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Low-Memory Bake Prep",
            "This will reduce bake quality settings and remove Contribute GI from very small renderers.\nContinue?",
            "Apply",
            "Cancel");

        if (!confirmed) return false;

        Scene scene = SceneManager.GetActiveScene();
        int removed = RemoveSmallGiContributors(scene, SmallGiMaxDimension);
        ApplyLowMemoryLightingPreset();
        Lightmapping.Clear();
        EditorSceneManager.MarkSceneDirty(scene);

        string message =
            $"Small GI contributors removed: {removed}\n" +
            "Lighting preset switched to low-memory mode.\n" +
            "Baked data cleared.";
        Debug.Log("[BakeLightingTools] " + message.Replace("\n", " | "));
        EditorUtility.DisplayDialog("Low-Memory Prep Complete", message, "OK");
        return true;
    }

    [MenuItem(RootMenu + "Bake Active Scene Lighting (Low Memory)")]
    public static void BakeActiveSceneLightingLowMemory()
    {
        if (!PrepareSceneForLowMemoryBakeInternal()) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Lightmapping.BakeAsync();
        Debug.Log("[BakeLightingTools] Started low-memory Lightmapping.BakeAsync() for active scene.");
    }

    [MenuItem(RootMenu + "Diagnose Non-Finite Bake Risks")]
    public static void DiagnoseNonFiniteBakeRisks()
    {
        Scene scene = SceneManager.GetActiveScene();
        NonFiniteAudit audit = RunNonFiniteAudit(scene, applyFixes: false);
        string report = BuildNonFiniteReport(audit);
        Debug.Log(report);
        EditorUtility.DisplayDialog("Non-Finite Bake Diagnostics", BuildNonFiniteDialogSummary(audit), "OK");
    }

    [MenuItem(RootMenu + "Sanitize Scene For Stable Bake")]
    public static void SanitizeSceneForStableBake()
    {
        SanitizeSceneForStableBakeInternal();
    }

    [MenuItem(RootMenu + "Bake Active Scene Lighting (Safe Mode)")]
    public static void BakeActiveSceneLightingSafeMode()
    {
        if (!SanitizeSceneForStableBakeInternal()) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Lightmapping.BakeAsync();
        Debug.Log("[BakeLightingTools] Started safe-mode Lightmapping.BakeAsync() for active scene.");
    }

    private static bool SanitizeSceneForStableBakeInternal()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Sanitize Scene For Stable Bake",
            "This will clamp/repair invalid light and material values, remove GI from invalid meshes/transforms, and switch to a safer bake preset.\nContinue?",
            "Apply",
            "Cancel");

        if (!confirmed) return false;

        Scene scene = SceneManager.GetActiveScene();
        NonFiniteAudit audit = RunNonFiniteAudit(scene, applyFixes: true);
        ApplySafeBakeLightingPreset();
        Lightmapping.Clear();
        EditorSceneManager.MarkSceneDirty(scene);

        string report = BuildNonFiniteReport(audit);
        Debug.Log(report);
        EditorUtility.DisplayDialog("Scene Sanitized", BuildNonFiniteDialogSummary(audit), "OK");
        return true;
    }

    private static BakeAudit RunAudit(bool applyFixes)
    {
        Scene scene = SceneManager.GetActiveScene();
        BakeAudit audit = new BakeAudit
        {
            sceneName = scene.name,
            hasSunSource = RenderSettings.sun != null,
            hasSkybox = RenderSettings.skybox != null,
            directionalLightCount = CountDirectionalLights(scene),
            volumeCount = CountVolumes(scene),
            dynamicExamples = new List<string>()
        };

        HashSet<GameObject> roots = GetTargetRoots(scene);
        foreach (GameObject root in roots)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.gameObject.scene != scene) continue;
                if (renderer is ParticleSystemRenderer || renderer is TrailRenderer || renderer is LineRenderer) continue;

                audit.totalRenderers++;
                StaticEditorFlags currentFlags = GameObjectUtility.GetStaticEditorFlags(renderer.gameObject);
                bool contributesGi = (currentFlags & StaticEditorFlags.ContributeGI) != 0;
                if (contributesGi) audit.alreadyContributeGi++;

                bool isDynamic = IsLikelyDynamic(renderer.gameObject);
                if (isDynamic)
                {
                    audit.skippedDynamic++;
                    if (audit.dynamicExamples.Count < 10)
                        audit.dynamicExamples.Add(GetHierarchyPath(renderer.transform));
                    continue;
                }

                audit.staticCandidates++;

                if (!applyFixes || contributesGi) continue;

                StaticEditorFlags updatedFlags = currentFlags | StaticEditorFlags.ContributeGI;
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, updatedFlags);
                if (renderer is MeshRenderer meshRenderer)
                    meshRenderer.receiveGI = ReceiveGI.Lightmaps;
                EditorUtility.SetDirty(renderer.gameObject);
                audit.markedContributeGi++;
            }
        }

        Terrain[] terrains = UnityEngine.Object.FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Terrain terrain in terrains)
        {
            if (terrain == null || terrain.gameObject.scene != scene) continue;
            if (IsLikelyDynamic(terrain.gameObject)) continue;
            audit.terrainCount++;

            if (!applyFixes) continue;
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(terrain.gameObject);
            if ((flags & StaticEditorFlags.ContributeGI) != 0) continue;
            GameObjectUtility.SetStaticEditorFlags(terrain.gameObject, flags | StaticEditorFlags.ContributeGI);
            EditorUtility.SetDirty(terrain.gameObject);
            audit.markedTerrainContributeGi++;
        }

        if (applyFixes)
        {
            EnsureSunSource(scene, ref audit);
            EditorSceneManager.MarkSceneDirty(scene);
        }

        return audit;
    }

    private static HashSet<GameObject> GetTargetRoots(Scene scene)
    {
        HashSet<GameObject> roots = new HashSet<GameObject>();
        GameObject[] selected = Selection.gameObjects;
        if (selected != null && selected.Length > 0)
        {
            foreach (GameObject go in selected)
            {
                if (go == null || go.scene != scene) continue;
                roots.Add(go.transform.root.gameObject);
            }
            return roots;
        }

        foreach (GameObject root in scene.GetRootGameObjects())
            roots.Add(root);
        return roots;
    }

    private static int CountDirectionalLights(Scene scene)
    {
        Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return lights.Count(l => l != null && l.gameObject.scene == scene && l.type == LightType.Directional && l.enabled);
    }

    private static int CountVolumes(Scene scene)
    {
        Volume[] volumes = UnityEngine.Object.FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return volumes.Count(v => v != null && v.gameObject.scene == scene && v.enabled);
    }

    private static void EnsureSunSource(Scene scene, ref BakeAudit audit)
    {
        if (RenderSettings.sun != null) return;
        Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Light directional = lights.FirstOrDefault(l => l != null && l.gameObject.scene == scene && l.type == LightType.Directional && l.enabled);
        if (directional == null) return;
        RenderSettings.sun = directional;
        audit.sunAssignedByTool = true;
    }

    private static bool IsLikelyDynamic(GameObject go)
    {
        if (go.CompareTag("Player") || go.CompareTag("MainCamera")) return true;
        if (go.GetComponentInParent<Animator>(true) != null) return true;
        if (go.GetComponentInParent<CharacterController>(true) != null) return true;
        if (go.GetComponentInParent<NavMeshAgent>(true) != null) return true;
        if (go.GetComponentInParent<Rigidbody>(true) != null) return true;
        return false;
    }

    private static string BuildReport(BakeAudit audit)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"[BakeLightingTools] Scene: {audit.sceneName}");
        sb.AppendLine($"- Renderers scanned: {audit.totalRenderers}");
        sb.AppendLine($"- Already Contribute GI: {audit.alreadyContributeGi}");
        sb.AppendLine($"- Static candidates: {audit.staticCandidates}");
        sb.AppendLine($"- Marked Contribute GI now: {audit.markedContributeGi}");
        sb.AppendLine($"- Dynamic skipped: {audit.skippedDynamic}");
        sb.AppendLine($"- Terrains found: {audit.terrainCount}");
        sb.AppendLine($"- Terrains marked now: {audit.markedTerrainContributeGi}");
        sb.AppendLine($"- Directional lights: {audit.directionalLightCount}");
        sb.AppendLine($"- Sun source assigned: {audit.hasSunSource || audit.sunAssignedByTool}");
        sb.AppendLine($"- Skybox assigned: {audit.hasSkybox}");
        sb.AppendLine($"- HDRP Volumes found: {audit.volumeCount}");
        if (audit.dynamicExamples.Count > 0)
        {
            sb.AppendLine("- Dynamic examples (not marked):");
            foreach (string path in audit.dynamicExamples)
                sb.AppendLine($"  - {path}");
        }
        return sb.ToString();
    }

    private static NonFiniteAudit RunNonFiniteAudit(Scene scene, bool applyFixes)
    {
        NonFiniteAudit audit = new NonFiniteAudit
        {
            sceneName = scene.name,
            issues = new List<string>()
        };

        AuditTransforms(scene, applyFixes, audit);
        AuditLights(scene, applyFixes, audit);
        AuditRenderersMaterialsAndMeshes(scene, applyFixes, audit);

        return audit;
    }

    private static void AuditTransforms(Scene scene, bool applyFixes, NonFiniteAudit audit)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform transform in transforms)
        {
            if (transform == null || transform.gameObject.scene != scene) continue;
            audit.transformsScanned++;

            Vector3 position = transform.position;
            Vector3 scale = transform.lossyScale;
            bool invalid =
                !IsFinite(position) ||
                !IsFinite(transform.rotation) ||
                !IsFinite(scale);

            bool extremePosition =
                Mathf.Abs(position.x) > MaxReasonableWorldCoordinate ||
                Mathf.Abs(position.y) > MaxReasonableWorldCoordinate ||
                Mathf.Abs(position.z) > MaxReasonableWorldCoordinate;

            bool extremeScale =
                Mathf.Abs(scale.x) < MinReasonableScaleMagnitude ||
                Mathf.Abs(scale.y) < MinReasonableScaleMagnitude ||
                Mathf.Abs(scale.z) < MinReasonableScaleMagnitude ||
                Mathf.Abs(scale.x) > MaxReasonableScaleMagnitude ||
                Mathf.Abs(scale.y) > MaxReasonableScaleMagnitude ||
                Mathf.Abs(scale.z) > MaxReasonableScaleMagnitude;

            if (!invalid && !extremePosition && !extremeScale) continue;

            if (invalid) audit.invalidTransforms++;
            if (extremePosition) audit.extremeTransforms++;
            if (extremeScale) audit.extremeTransforms++;

            string transformPath = GetHierarchyPath(transform);
            if (invalid)
                AddIssue(audit, $"Invalid transform values at: {transformPath}");
            if (extremePosition)
                AddIssue(audit, $"Extreme world position at: {transformPath} -> {position}");
            if (extremeScale)
                AddIssue(audit, $"Extreme/near-zero scale at: {transformPath} -> {scale}");

            if (!applyFixes) continue;

            int removed = RemoveGiFromHierarchy(transform.gameObject);
            audit.renderersRemovedFromGi += removed;
            if (removed > 0)
                AddIssue(audit, $"Removed Contribute GI from {removed} renderer(s) under: {transformPath}");
        }
    }

    private static void AuditLights(Scene scene, bool applyFixes, NonFiniteAudit audit)
    {
        Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Light light in lights)
        {
            if (light == null || light.gameObject.scene != scene) continue;
            audit.lightsScanned++;

            bool changed = false;
            bool hadInvalid = false;
            bool hadClamp = false;
            string lightPath = GetHierarchyPath(light.transform);

            if (!IsFinite(light.intensity) || light.intensity < 0f)
            {
                hadInvalid = true;
                if (applyFixes)
                {
                    light.intensity = 1f;
                    changed = true;
                }
            }
            else if (light.intensity > MaxReasonableLightIntensity)
            {
                hadClamp = true;
                if (applyFixes)
                {
                    light.intensity = MaxReasonableLightIntensity;
                    changed = true;
                }
            }

            if (!IsFinite(light.bounceIntensity) || light.bounceIntensity < 0f)
            {
                hadInvalid = true;
                if (applyFixes)
                {
                    light.bounceIntensity = 1f;
                    changed = true;
                }
            }
            else if (light.bounceIntensity > MaxReasonableBounceIntensity)
            {
                hadClamp = true;
                if (applyFixes)
                {
                    light.bounceIntensity = MaxReasonableBounceIntensity;
                    changed = true;
                }
            }

            if (light.type != LightType.Directional)
            {
                if (!IsFinite(light.range) || light.range <= 0f)
                {
                    hadInvalid = true;
                    if (applyFixes)
                    {
                        light.range = 10f;
                        changed = true;
                    }
                }
                else if (light.range > 10000f)
                {
                    hadClamp = true;
                    if (applyFixes)
                    {
                        light.range = 10000f;
                        changed = true;
                    }
                }
            }

            if (!IsFinite(light.color))
            {
                hadInvalid = true;
                if (applyFixes)
                {
                    light.color = Color.white;
                    changed = true;
                }
            }

            if (hadInvalid)
                AddIssue(audit, $"Invalid light values at: {lightPath}");
            if (hadClamp)
                AddIssue(audit, $"Extreme light values clamped at: {lightPath}");

            if (!changed) continue;

            if (hadInvalid) audit.fixedLights++;
            if (hadClamp) audit.clampedLights++;
            EditorUtility.SetDirty(light);
        }
    }

    private static void AuditRenderersMaterialsAndMeshes(Scene scene, bool applyFixes, NonFiniteAudit audit)
    {
        Renderer[] renderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        HashSet<Material> seenMaterials = new HashSet<Material>();
        Dictionary<Mesh, MeshAuditResult> meshValidity = new Dictionary<Mesh, MeshAuditResult>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || renderer.gameObject.scene != scene) continue;
            if (renderer is ParticleSystemRenderer || renderer is TrailRenderer || renderer is LineRenderer) continue;

            audit.renderersScanned++;

            Mesh mesh = TryGetSharedMesh(renderer);
            if (mesh != null)
            {
                if (!meshValidity.TryGetValue(mesh, out MeshAuditResult meshAudit))
                {
                    audit.meshesScanned++;
                    meshAudit = EvaluateMeshForBake(mesh);
                    meshValidity[mesh] = meshAudit;
                    if (meshAudit.hasRisk)
                    {
                        audit.meshesWithInvalidData++;
                        AddIssue(audit, $"Mesh bake risk '{mesh.name}': {meshAudit.reason}");
                    }
                }

                if (meshValidity[mesh].hasRisk && applyFixes && RemoveContributeGi(renderer.gameObject))
                {
                    audit.renderersRemovedFromGi++;
                }
            }

            Material[] materials = renderer.sharedMaterials;
            foreach (Material material in materials)
            {
                if (material == null || !seenMaterials.Add(material)) continue;

                audit.materialsScanned++;
                if (!ValidateMaterialForBake(material, applyFixes, out string issue, out bool changed)) continue;

                AddIssue(audit, $"Material '{material.name}': {issue}");
                if (!changed) continue;

                audit.materialsFixed++;
                EditorUtility.SetDirty(material);
            }
        }
    }

    private static bool ValidateMaterialForBake(Material material, bool applyFixes, out string issue, out bool changed)
    {
        changed = false;
        List<string> issues = new List<string>();

        Shader shader = material.shader;
        if (shader == null)
        {
            issue = "material has no shader";
            return true;
        }

        int propertyCount = ShaderUtil.GetPropertyCount(shader);
        for (int i = 0; i < propertyCount; i++)
        {
            string propertyName = ShaderUtil.GetPropertyName(shader, i);
            if (!material.HasProperty(propertyName)) continue;

            ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(shader, i);
            switch (propertyType)
            {
                case ShaderUtil.ShaderPropertyType.Float:
                case ShaderUtil.ShaderPropertyType.Range:
                {
                    float value = material.GetFloat(propertyName);
                    bool isEmissionRelated = propertyName.IndexOf("emiss", StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!IsFinite(value))
                    {
                        issues.Add($"{propertyName}=NaN/Inf");
                        if (applyFixes)
                        {
                            material.SetFloat(propertyName, 0f);
                            changed = true;
                        }
                        break;
                    }

                    if (isEmissionRelated && value > MaxReasonableEmissionIntensity)
                    {
                        issues.Add($"{propertyName} too high ({value:0.##})");
                        if (applyFixes)
                        {
                            material.SetFloat(propertyName, MaxReasonableEmissionIntensity);
                            changed = true;
                        }
                    }
                    break;
                }
                case ShaderUtil.ShaderPropertyType.Vector:
                {
                    Vector4 value = material.GetVector(propertyName);
                    if (IsFinite(value)) break;

                    issues.Add($"{propertyName}=NaN/Inf");
                    if (!applyFixes) break;
                    material.SetVector(propertyName, Vector4.zero);
                    changed = true;
                    break;
                }
                case ShaderUtil.ShaderPropertyType.Color:
                {
                    Color value = material.GetColor(propertyName);
                    bool isEmission = propertyName.IndexOf("emiss", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (IsFinite(value))
                    {
                        if (isEmission)
                        {
                            float maxComponent = Mathf.Max(value.r, value.g, value.b);
                            bool contributesEmissionGi = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != 0;
                            if (contributesEmissionGi && maxComponent > MaxReasonableEmissionIntensity)
                            {
                                issues.Add($"{propertyName} too high ({maxComponent:0.##})");
                                if (!applyFixes) break;
                                float scale = MaxReasonableEmissionIntensity / maxComponent;
                                material.SetColor(propertyName, new Color(value.r * scale, value.g * scale, value.b * scale, value.a));
                                changed = true;
                            }
                        }
                        break;
                    }

                    issues.Add($"{propertyName}=NaN/Inf");
                    if (!applyFixes) break;

                    if (isEmission)
                    {
                        material.SetColor(propertyName, Color.black);
                        material.globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    }
                    else
                    {
                        material.SetColor(propertyName, Color.white);
                    }
                    changed = true;
                    break;
                }
            }
        }

        issue = issues.Count == 0 ? string.Empty : string.Join(", ", issues.Take(3));
        return issues.Count > 0;
    }

    private static int RemoveGiFromHierarchy(GameObject root)
    {
        int removed = 0;
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (RemoveContributeGi(renderer.gameObject))
                removed++;
        }
        return removed;
    }

    private static bool RemoveContributeGi(GameObject go)
    {
        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(go);
        if ((flags & StaticEditorFlags.ContributeGI) == 0) return false;
        GameObjectUtility.SetStaticEditorFlags(go, flags & ~StaticEditorFlags.ContributeGI);
        EditorUtility.SetDirty(go);
        return true;
    }

    private static Mesh TryGetSharedMesh(Renderer renderer)
    {
        if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
            return skinnedMeshRenderer.sharedMesh;

        if (renderer is MeshRenderer meshRenderer)
        {
            MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
            if (meshFilter != null)
                return meshFilter.sharedMesh;
        }

        return null;
    }

    private static MeshAuditResult EvaluateMeshForBake(Mesh mesh)
    {
        if (mesh == null)
            return MeshAuditResult.Valid;

        Bounds bounds = mesh.bounds;
        if (!IsFinite(bounds.center) || !IsFinite(bounds.extents))
            return MeshAuditResult.Risk("non-finite mesh bounds");

        float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        if (maxExtent > MaxReasonableBoundsExtent)
            return MeshAuditResult.Risk($"mesh bounds are too large ({maxExtent:0.##})");

        Vector3[] vertices = mesh.vertices;
        if (vertices == null || vertices.Length == 0)
            return MeshAuditResult.Risk("mesh has no vertices");

        int extremeVertices = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            if (!IsFinite(vertex))
                return MeshAuditResult.Risk("contains non-finite vertex values");

            if (Mathf.Abs(vertex.x) > MaxReasonableVertexMagnitude ||
                Mathf.Abs(vertex.y) > MaxReasonableVertexMagnitude ||
                Mathf.Abs(vertex.z) > MaxReasonableVertexMagnitude)
            {
                extremeVertices++;
            }
        }

        if (extremeVertices > 0)
            return MeshAuditResult.Risk($"contains {extremeVertices} extreme vertex values");

        Vector3[] normals = mesh.normals;
        if (normals != null && normals.Length > 0)
        {
            int invalidNormals = 0;
            int tinyNormals = 0;
            int normalCount = Mathf.Min(normals.Length, vertices.Length);

            for (int i = 0; i < normalCount; i++)
            {
                Vector3 normal = normals[i];
                if (!IsFinite(normal))
                {
                    invalidNormals++;
                    continue;
                }

                if (normal.sqrMagnitude < MinReasonableNormalMagnitudeSqr)
                    tinyNormals++;
            }

            if (invalidNormals > 0)
                return MeshAuditResult.Risk($"contains {invalidNormals} non-finite normals");

            if (normalCount > 0 && (float)tinyNormals / normalCount > MaxDegenerateTriangleRatio)
                return MeshAuditResult.Risk($"has too many tiny normals ({tinyNormals}/{normalCount})");
        }

        Vector4[] tangents = mesh.tangents;
        if (tangents != null && tangents.Length > 0)
        {
            int invalidTangents = 0;
            int tinyTangents = 0;
            int tangentCount = Mathf.Min(tangents.Length, vertices.Length);

            for (int i = 0; i < tangentCount; i++)
            {
                Vector4 tangent = tangents[i];
                if (!IsFinite(tangent))
                {
                    invalidTangents++;
                    continue;
                }

                Vector3 tangentDir = new Vector3(tangent.x, tangent.y, tangent.z);
                if (tangentDir.sqrMagnitude < MinReasonableTangentMagnitudeSqr)
                    tinyTangents++;
            }

            if (invalidTangents > 0)
                return MeshAuditResult.Risk($"contains {invalidTangents} non-finite tangents");

            if (tangentCount > 0 && (float)tinyTangents / tangentCount > MaxDegenerateTriangleRatio)
                return MeshAuditResult.Risk($"has too many tiny tangents ({tinyTangents}/{tangentCount})");
        }

        int[] triangles = mesh.triangles;
        if (triangles != null && triangles.Length > 0)
        {
            if (triangles.Length % 3 != 0)
                return MeshAuditResult.Risk("triangle index count is not divisible by 3");

            int degenerateTriangles = 0;
            int triangleCount = triangles.Length / 3;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int indexA = triangles[i];
                int indexB = triangles[i + 1];
                int indexC = triangles[i + 2];

                if (indexA < 0 || indexB < 0 || indexC < 0 ||
                    indexA >= vertices.Length || indexB >= vertices.Length || indexC >= vertices.Length)
                {
                    return MeshAuditResult.Risk("triangle index out of range");
                }

                Vector3 a = vertices[indexA];
                Vector3 b = vertices[indexB];
                Vector3 c = vertices[indexC];
                float areaSqr = Vector3.Cross(b - a, c - a).sqrMagnitude;
                if (areaSqr < 0.0000000001f)
                    degenerateTriangles++;
            }

            if (triangleCount > 0 && (float)degenerateTriangles / triangleCount > MaxDegenerateTriangleRatio)
                return MeshAuditResult.Risk($"has too many degenerate triangles ({degenerateTriangles}/{triangleCount})");
        }

        Vector2[] uv = mesh.uv;
        for (int i = 0; i < uv.Length; i++)
        {
            if (!IsFinite(uv[i]))
                return MeshAuditResult.Risk("contains non-finite UV values");
        }

        return MeshAuditResult.Valid;
    }

    private static string BuildNonFiniteReport(NonFiniteAudit audit)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"[BakeLightingTools] Non-finite bake audit for scene: {audit.sceneName}");
        sb.AppendLine($"- Transforms scanned: {audit.transformsScanned}");
        sb.AppendLine($"- Invalid transforms: {audit.invalidTransforms}");
        sb.AppendLine($"- Extreme transforms: {audit.extremeTransforms}");
        sb.AppendLine($"- Lights scanned: {audit.lightsScanned}");
        sb.AppendLine($"- Lights fixed: {audit.fixedLights}");
        sb.AppendLine($"- Lights clamped: {audit.clampedLights}");
        sb.AppendLine($"- Renderers scanned: {audit.renderersScanned}");
        sb.AppendLine($"- Meshes scanned: {audit.meshesScanned}");
        sb.AppendLine($"- Meshes with invalid geometry data: {audit.meshesWithInvalidData}");
        sb.AppendLine($"- Materials scanned: {audit.materialsScanned}");
        sb.AppendLine($"- Materials fixed: {audit.materialsFixed}");
        sb.AppendLine($"- Renderers removed from GI: {audit.renderersRemovedFromGi}");
        sb.AppendLine($"- Total issues detected: {audit.issues.Count}");
        if (audit.issues.Count > 0)
        {
            sb.AppendLine("- Issues:");
            foreach (string issue in audit.issues)
                sb.AppendLine($"  - {issue}");
        }
        return sb.ToString();
    }

    private static string BuildNonFiniteDialogSummary(NonFiniteAudit audit)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Invalid transforms: {audit.invalidTransforms}");
        sb.AppendLine($"Extreme transforms: {audit.extremeTransforms}");
        sb.AppendLine($"Lights fixed/clamped: {audit.fixedLights}/{audit.clampedLights}");
        sb.AppendLine($"Meshes with invalid data: {audit.meshesWithInvalidData}");
        sb.AppendLine($"Materials fixed: {audit.materialsFixed}");
        sb.AppendLine($"GI removed from renderers: {audit.renderersRemovedFromGi}");
        sb.AppendLine($"Issues found: {audit.issues.Count}");
        if (audit.issues.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("First issues:");
            int count = Mathf.Min(MaxDetailedIssuesInDialog, audit.issues.Count);
            for (int i = 0; i < count; i++)
                sb.AppendLine($"- {audit.issues[i]}");
        }
        return sb.ToString();
    }

    private static void AddIssue(NonFiniteAudit audit, string issue)
    {
        if (string.IsNullOrWhiteSpace(issue)) return;
        if (audit.issues.Count >= 120) return;
        audit.issues.Add(issue);
    }

    private static int RemoveSmallGiContributors(Scene scene, float maxDimension)
    {
        int removed = 0;
        Renderer[] renderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || renderer.gameObject.scene != scene) continue;
            if (renderer is ParticleSystemRenderer || renderer is TrailRenderer || renderer is LineRenderer) continue;
            if (IsLikelyDynamic(renderer.gameObject)) continue;

            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(renderer.gameObject);
            bool contributesGi = (flags & StaticEditorFlags.ContributeGI) != 0;
            if (!contributesGi) continue;

            float dimension = Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
            if (dimension > maxDimension) continue;

            StaticEditorFlags updatedFlags = flags & ~StaticEditorFlags.ContributeGI;
            GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, updatedFlags);
            EditorUtility.SetDirty(renderer.gameObject);
            removed++;
        }
        return removed;
    }

    private static void ApplyLowMemoryLightingPreset()
    {
        LightingSettings settings = Lightmapping.lightingSettings;
        if (settings == null)
        {
            settings = new LightingSettings();
            Lightmapping.lightingSettings = settings;
        }

        SerializedObject so = new SerializedObject(settings);
        SetIntIfExists(so, "m_BakeBackend", 1);
        SetIntIfExists(so, "m_LightmapMaxSize", 512);
        SetFloatIfExists(so, "m_BakeResolution", 4f);
        SetIntIfExists(so, "m_Padding", 2);
        SetIntIfExists(so, "m_PVRDirectSampleCount", 16);
        SetIntIfExists(so, "m_PVRSampleCount", 64);
        SetIntIfExists(so, "m_PVREnvironmentSampleCount", 32);
        SetIntIfExists(so, "m_PVRBounces", 1);
        SetIntIfExists(so, "m_PVRMinBounces", 1);
        SetIntIfExists(so, "m_LightProbeSampleCountMultiplier", 2);
        SetIntIfExists(so, "m_FilterMode", 0);
        SetIntIfExists(so, "m_PVRFilteringMode", 0);
        SetIntIfExists(so, "m_AO", 0);
        SetIntIfExists(so, "m_EnableWorkerProcessBaking", 1);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(settings);
    }

    private static void ApplySafeBakeLightingPreset()
    {
        ApplyLowMemoryLightingPreset();

        LightingSettings settings = Lightmapping.lightingSettings;
        if (settings == null) return;

        SerializedObject so = new SerializedObject(settings);

        bool directionalityDisabled =
            SetEnumByTokenIfExists(so, "m_LightmapDirectionalMode", "Non", "Off", "None") ||
            SetEnumByTokenIfExists(so, "m_DirectionalityMode", "Non", "Off", "None");

        SetIntIfExists(so, "m_FilterMode", 0);
        SetIntIfExists(so, "m_PVRFilteringMode", 0);
        SetIntIfExists(so, "m_DenoiserTypeDirect", 0);
        SetIntIfExists(so, "m_DenoiserTypeIndirect", 0);
        SetIntIfExists(so, "m_DenoiserTypeAO", 0);

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(settings);

        bool reflectionDirectionalityDisabled = ForceNonDirectionalLightmapsIfPossible();
        bool forcedCpuLightmapper = ForceCpuLightmapperIfPossible();
        directionalityDisabled = directionalityDisabled || reflectionDirectionalityDisabled;

        if (directionalityDisabled)
            Debug.Log("[BakeLightingTools] Directionality mode switched to a non-directional option for bake stability.");
        else
            Debug.LogWarning("[BakeLightingTools] Could not force non-directional mode. If non-finite errors continue, switch Lighting > Directional Mode to Non-Directional manually.");

        if (forcedCpuLightmapper)
            Debug.Log("[BakeLightingTools] Lightmapper switched to Progressive CPU for extra stability.");
    }

    private static void SetIntIfExists(SerializedObject so, string property, int value)
    {
        SerializedProperty p = so.FindProperty(property);
        if (p == null) return;

        switch (p.propertyType)
        {
            case SerializedPropertyType.Integer:
                p.intValue = value;
                break;
            case SerializedPropertyType.Boolean:
                p.boolValue = value != 0;
                break;
            case SerializedPropertyType.Enum:
                int enumIndex = Mathf.Clamp(value, 0, Mathf.Max(0, p.enumDisplayNames.Length - 1));
                p.enumValueIndex = enumIndex;
                break;
            case SerializedPropertyType.Float:
                p.floatValue = value;
                break;
            default:
                Debug.LogWarning($"[BakeLightingTools] Property '{property}' is type '{p.propertyType}' and cannot be set as int directly.");
                break;
        }
    }

    private static void SetFloatIfExists(SerializedObject so, string property, float value)
    {
        SerializedProperty p = so.FindProperty(property);
        if (p == null) return;

        switch (p.propertyType)
        {
            case SerializedPropertyType.Float:
                p.floatValue = value;
                break;
            case SerializedPropertyType.Integer:
                p.intValue = Mathf.RoundToInt(value);
                break;
            case SerializedPropertyType.Boolean:
                p.boolValue = !Mathf.Approximately(value, 0f);
                break;
            default:
                Debug.LogWarning($"[BakeLightingTools] Property '{property}' is type '{p.propertyType}' and cannot be set as float directly.");
                break;
        }
    }

    private static bool SetEnumByTokenIfExists(SerializedObject so, string property, params string[] preferredTokens)
    {
        SerializedProperty p = so.FindProperty(property);
        if (p == null || p.propertyType != SerializedPropertyType.Enum) return false;

        string[] names = p.enumDisplayNames;
        if (names == null || names.Length == 0) return false;

        for (int i = 0; i < names.Length; i++)
        {
            string enumName = names[i];
            foreach (string token in preferredTokens)
            {
                if (string.IsNullOrWhiteSpace(token)) continue;
                if (enumName.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0) continue;
                p.enumValueIndex = i;
                return true;
            }
        }

        return false;
    }

    private static bool ForceNonDirectionalLightmapsIfPossible()
    {
        Type lightmapEditorSettingsType = typeof(Editor).Assembly.GetType("UnityEditor.LightmapEditorSettings");
        if (lightmapEditorSettingsType == null) return false;

        return TrySetEnumPropertyByNameContains(
            lightmapEditorSettingsType,
            propertyName: "lightmapsMode",
            preferredTokens: new[] { "NonDirectional", "Non-Directional", "Non" });
    }

    private static bool ForceCpuLightmapperIfPossible()
    {
        Type lightmapEditorSettingsType = typeof(Editor).Assembly.GetType("UnityEditor.LightmapEditorSettings");
        if (lightmapEditorSettingsType == null) return false;

        return TrySetEnumPropertyByNameContains(
            lightmapEditorSettingsType,
            propertyName: "lightmapper",
            preferredTokens: new[] { "ProgressiveCPU", "CPU" });
    }

    private static bool TrySetEnumPropertyByNameContains(Type ownerType, string propertyName, string[] preferredTokens)
    {
        if (ownerType == null || string.IsNullOrWhiteSpace(propertyName)) return false;

        System.Reflection.PropertyInfo property =
            ownerType.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (property == null) return false;

        Type enumType = property.PropertyType;
        if (!enumType.IsEnum) return false;

        Array values = Enum.GetValues(enumType);
        foreach (object value in values)
        {
            string name = Enum.GetName(enumType, value);
            if (string.IsNullOrWhiteSpace(name)) continue;

            foreach (string token in preferredTokens)
            {
                if (string.IsNullOrWhiteSpace(token)) continue;
                if (name.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0) continue;
                property.SetValue(null, value);
                return true;
            }
        }

        return false;
    }

    private static string BuildDialogSummary(BakeAudit audit)
    {
        return
            $"Renderers: {audit.totalRenderers}\n" +
            $"Marked GI now: {audit.markedContributeGi}\n" +
            $"Dynamic skipped: {audit.skippedDynamic}\n" +
            $"Sun assigned: {(audit.hasSunSource || audit.sunAssignedByTool)}\n" +
            $"Skybox assigned: {audit.hasSkybox}";
    }

    private static string GetHierarchyPath(Transform transform)
    {
        if (transform == null) return "<null>";
        StringBuilder sb = new StringBuilder(transform.name);
        Transform current = transform.parent;
        while (current != null)
        {
            sb.Insert(0, current.name + "/");
            current = current.parent;
        }
        return sb.ToString();
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private static bool IsFinite(Vector2 value)
    {
        return IsFinite(value.x) && IsFinite(value.y);
    }

    private static bool IsFinite(Vector3 value)
    {
        return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
    }

    private static bool IsFinite(Vector4 value)
    {
        return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z) && IsFinite(value.w);
    }

    private static bool IsFinite(Color value)
    {
        return IsFinite(value.r) && IsFinite(value.g) && IsFinite(value.b) && IsFinite(value.a);
    }

    private static bool IsFinite(Quaternion value)
    {
        return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z) && IsFinite(value.w);
    }

    private struct BakeAudit
    {
        public string sceneName;
        public int totalRenderers;
        public int alreadyContributeGi;
        public int staticCandidates;
        public int markedContributeGi;
        public int skippedDynamic;
        public int terrainCount;
        public int markedTerrainContributeGi;
        public int directionalLightCount;
        public int volumeCount;
        public bool hasSunSource;
        public bool sunAssignedByTool;
        public bool hasSkybox;
        public List<string> dynamicExamples;
    }

    private sealed class NonFiniteAudit
    {
        public string sceneName;
        public int transformsScanned;
        public int invalidTransforms;
        public int extremeTransforms;
        public int lightsScanned;
        public int fixedLights;
        public int clampedLights;
        public int renderersScanned;
        public int materialsScanned;
        public int materialsFixed;
        public int meshesScanned;
        public int meshesWithInvalidData;
        public int renderersRemovedFromGi;
        public List<string> issues;
    }

    private readonly struct MeshAuditResult
    {
        public readonly bool hasRisk;
        public readonly string reason;

        public MeshAuditResult(bool hasRisk, string reason)
        {
            this.hasRisk = hasRisk;
            this.reason = reason;
        }

        public static MeshAuditResult Valid => new MeshAuditResult(false, string.Empty);
        public static MeshAuditResult Risk(string reason) => new MeshAuditResult(true, reason ?? "unknown risk");
    }
}
