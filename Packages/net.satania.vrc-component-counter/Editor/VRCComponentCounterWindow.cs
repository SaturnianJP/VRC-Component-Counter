using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Core;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.Constraint.Components;
using VRC.SDK3.Dynamics.Contact.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace net.satania.VRCComponentCounter
{
    public static class GameObjectExtensions
    {
        public static T[] GetAllComponentsWithoutEditorOnly<T>(this GameObject target, bool includeInactive = true) where T : Component
        {
            return target.GetComponentsInChildren<T>(includeInactive)
                .Where(x => !x.gameObject.IsEditorOnly())
                .ToArray();
        }

        public static bool IsEditorOnly(this GameObject target)
        {
            if (target.CompareTag("EditorOnly"))
                return true;

            return _IsEditorOnlyRecursive(target.transform);

            bool _IsEditorOnlyRecursive(Transform target)
            {
                if (target.CompareTag("EditorOnly"))
                    return true;

                if (target.parent != null)
                    return _IsEditorOnlyRecursive(target.parent);

                return false;
            }
        }
    }

    public class VRCComponentCounterWindow : EditorWindow
    {
        private GameObject targetObject;

        //-------------------------------------------------
        //VRC Components
        VRCAvatarDescriptor[] vrcAvatarDescriptors;
        VRCAimConstraint[] vrcAimConstraints;
        VRCLookAtConstraint[] vrcLookAtConstraints;
        VRCParentConstraint[] vrcParentConstraints;
        VRCPositionConstraint[] vrcPositionConstraints;
        VRCRotationConstraint[] vrcRotationConstraints;
        VRCScaleConstraint[] vrcScaleConstraints;

        VRCContactReceiver[] vrcContactReceivers;
        VRCContactSender[] vrcContactSenders;

        VRCHeadChop[] vrcHeadChops;

        VRCPhysBone[] vrcPhysBones;
        VRCPhysBoneCollider[] vrcPhysBoneColliders;

        PipelineManager[] vrcPipelineManagers;
        VRCSpatialAudioSource[] vrcSpatialAudioSources;
        VRCStation[] vrcStations;

        //-------------------------------------------------
        //Unity Components
        AimConstraint[] aimConstraints;
        Animation[] animations;
        Animator[] animators;
        AudioSource[] audioSources;
        Camera[] cameras;
        Cloth[] cloths;
        Collider[] colliders;
        FlareLayer[] flareLayers;

        CharacterJoint[] characterJoints;
        ConfigurableJoint[] configurableJoints;
        FixedJoint[] fixedJoints;
        HingeJoint[] hingeJoints;
        SpringJoint[] springJoints;

        Light[] lights;
        LineRenderer[] lineRenderers;

        LookAtConstraint[] lookAtConstraints;

        MeshFilter[] meshFilters;
        MeshRenderer[] meshRenderers;

        ParentConstraint[] parentConstraints;
        ParticleSystemRenderer[] particleSystemRenderers;
        ParticleSystem[] particleSystems;

        PositionConstraint[] positionConstraints;

        Rigidbody[] rigidbodies;

        RotationConstraint[] rotationConstraints;
        ScaleConstraint[] scaleConstraints;

        SkinnedMeshRenderer[] skinnedMeshRenderers;
        TrailRenderer[] trailRenderers;
        Transform[] transforms;

        [MenuItem("Tools/Satania/VRC Component Counter")]
        private static void Open()
        {
            var w = GetWindow<VRCComponentCounterWindow>("VRC Component Counter");
            var r = new Rect(w.position.x, w.position.y, 937f, 407f);
            w.position = r;
        }

        private void OnEnable()
        {
            targetObject = null;
            ClearArrays();
        }

        private void ClearArrays()
        {
            //-------------------------------------------------
            //VRC Components
            vrcAvatarDescriptors = new VRCAvatarDescriptor[0];
            vrcAimConstraints = new VRCAimConstraint[0];
            vrcLookAtConstraints = new VRCLookAtConstraint[0];
            vrcParentConstraints = new VRCParentConstraint[0];
            vrcPositionConstraints = new VRCPositionConstraint[0];
            vrcRotationConstraints = new VRCRotationConstraint[0];
            vrcScaleConstraints = new VRCScaleConstraint[0];

            vrcContactReceivers = new VRCContactReceiver[0];
            vrcContactSenders = new VRCContactSender[0];

            vrcHeadChops = new VRCHeadChop[0];

            vrcPhysBones = new VRCPhysBone[0];
            vrcPhysBoneColliders = new VRCPhysBoneCollider[0];

            vrcPipelineManagers = new PipelineManager[0];
            vrcSpatialAudioSources = new VRCSpatialAudioSource[0];
            vrcStations = new VRCStation[0];

            //-------------------------------------------------
            //Unity Components
            transforms = new Transform[0];

            aimConstraints = new AimConstraint[0];
            lookAtConstraints = new LookAtConstraint[0];
            positionConstraints = new PositionConstraint[0];
            rotationConstraints = new RotationConstraint[0];
            parentConstraints = new ParentConstraint[0];
            scaleConstraints = new ScaleConstraint[0];

            animations = new Animation[0];
            animators = new Animator[0];
            audioSources = new AudioSource[0];
            cameras = new Camera[0];
            cloths = new Cloth[0];
            colliders = new Collider[0];
            flareLayers = new FlareLayer[0];

            characterJoints = new CharacterJoint[0];
            configurableJoints = new ConfigurableJoint[0];
            fixedJoints = new FixedJoint[0];
            hingeJoints = new HingeJoint[0];
            springJoints = new SpringJoint[0];

            lights = new Light[0];

            meshFilters = new MeshFilter[0];
            meshRenderers = new MeshRenderer[0];
            skinnedMeshRenderers = new SkinnedMeshRenderer[0];
            trailRenderers = new TrailRenderer[0];
            lineRenderers = new LineRenderer[0];

            particleSystemRenderers = new ParticleSystemRenderer[0];
            particleSystems = new ParticleSystem[0];

            rigidbodies = new Rigidbody[0];
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            targetObject = EditorGUILayout.ObjectField("Target", targetObject, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                if (!targetObject.scene.IsValid())
                {
                    EditorUtility.DisplayDialog(titleContent.text, "シーン上に存在しているオブジェクトを入れてください！", "OK");
                    targetObject = null;
                }
            }

            if (targetObject != null)
            {
                EditorGUILayout.Space(5);

                using (new GUILayout.HorizontalScope())
                {
                    using (new GUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField($"VRC AvatarDescriptor : {vrcAvatarDescriptors.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.LabelField($"VRC AimConstraint : {vrcAimConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC LookAtConstraint : {vrcLookAtConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC ParentConstraint : {vrcParentConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC PositionConstraint : {vrcPositionConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC RotationConstraint : {vrcRotationConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC ScaleConstraint : {vrcScaleConstraints.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.LabelField($"VRC ContactReceiver : {vrcContactReceivers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC ContactSender : {vrcContactSenders.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.LabelField($"VRC HeadChop : {vrcHeadChops.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.LabelField($"VRC PhysBone : {vrcPhysBones.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC PhysBoneCollider : {vrcPhysBoneColliders.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.LabelField($"VRC PipelineManager : {vrcPipelineManagers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC SpatialAudioSource : {vrcSpatialAudioSources.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"VRC Station : {vrcStations.Length}", EditorStyles.boldLabel);
                    }

                    using (new GUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField($"Transform : {transforms.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Animation : {animations.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Animator : {animators.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"AudioSource : {audioSources.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Camera : {cameras.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Cloth : {cloths.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Collider : {colliders.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Light : {lights.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"FlareLayer : {flareLayers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"Rigidbody : {rigidbodies.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.Space(5);

                        EditorGUILayout.LabelField($"AimConstraint : {aimConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"LookAtConstraint : {lookAtConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"PositionConstraint : {positionConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"RotationConstraint : {rotationConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"ParentConstraint : {parentConstraints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"ScaleConstraint : {scaleConstraints.Length}", EditorStyles.boldLabel);
                    }

                    using (new GUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField($"CharacterJoint : {characterJoints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"ConfigurableJoint : {configurableJoints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"FixedJoint : {fixedJoints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"HingeJoint : {hingeJoints.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"SpringJoint : {springJoints.Length}", EditorStyles.boldLabel);

                        EditorGUILayout.Space(5);

                        EditorGUILayout.LabelField($"MeshFilter : {meshFilters.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"MeshRenderer : {meshRenderers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"SkinnedMeshRenderer : {skinnedMeshRenderers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"TrailRenderer : {trailRenderers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"LineRenderer : {lineRenderers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"ParticleSystemRenderer : {particleSystemRenderers.Length}", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField($"ParticleSystem : {particleSystems.Length}", EditorStyles.boldLabel);
                    }
                }
                EditorGUI.indentLevel--;
                if (GUILayout.Button("Count", GUILayout.Height(25)))
                {
                    Count(targetObject);
                }
            }
        }

        private void Count(GameObject targetObject)
        {
            if (targetObject == null)
                return;

            //-------------------------------------------------
            //VRC Components
            {
                vrcAvatarDescriptors = targetObject.GetAllComponentsWithoutEditorOnly<VRCAvatarDescriptor>(true);

                vrcAimConstraints = targetObject.GetAllComponentsWithoutEditorOnly<VRCAimConstraint>(true);
                vrcLookAtConstraints = targetObject.GetAllComponentsWithoutEditorOnly<VRCLookAtConstraint>(true);
                vrcParentConstraints = targetObject.GetAllComponentsWithoutEditorOnly<VRCParentConstraint>(true);
                vrcPositionConstraints = targetObject.GetAllComponentsWithoutEditorOnly<VRCPositionConstraint>(true);
                vrcRotationConstraints = targetObject.GetAllComponentsWithoutEditorOnly<VRCRotationConstraint>(true);
                vrcScaleConstraints = targetObject.GetAllComponentsWithoutEditorOnly<VRCScaleConstraint>(true);

                vrcContactReceivers = targetObject.GetAllComponentsWithoutEditorOnly<VRCContactReceiver>(true);
                vrcContactSenders = targetObject.GetAllComponentsWithoutEditorOnly<VRCContactSender>(true);

                vrcHeadChops = targetObject.GetAllComponentsWithoutEditorOnly<VRCHeadChop>(true);

                vrcPhysBones = targetObject.GetAllComponentsWithoutEditorOnly<VRCPhysBone>(true);
                vrcPhysBoneColliders = targetObject.GetAllComponentsWithoutEditorOnly<VRCPhysBoneCollider>(true);

                vrcPipelineManagers = targetObject.GetAllComponentsWithoutEditorOnly<PipelineManager>(true);
                vrcSpatialAudioSources = targetObject.GetAllComponentsWithoutEditorOnly<VRCSpatialAudioSource>(true);
                vrcStations = targetObject.GetAllComponentsWithoutEditorOnly<VRCStation>(true);
            }

            //-------------------------------------------------
            //Unity Components
            {
                transforms = targetObject.GetAllComponentsWithoutEditorOnly<Transform>(true);

                aimConstraints = targetObject.GetAllComponentsWithoutEditorOnly<AimConstraint>(true);
                lookAtConstraints = targetObject.GetAllComponentsWithoutEditorOnly<LookAtConstraint>(true);
                positionConstraints = targetObject.GetAllComponentsWithoutEditorOnly<PositionConstraint>(true);
                rotationConstraints = targetObject.GetAllComponentsWithoutEditorOnly<RotationConstraint>(true);
                parentConstraints = targetObject.GetAllComponentsWithoutEditorOnly<ParentConstraint>(true);
                scaleConstraints = targetObject.GetAllComponentsWithoutEditorOnly<ScaleConstraint>(true);

                animations = targetObject.GetAllComponentsWithoutEditorOnly<Animation>(true);
                animators = targetObject.GetAllComponentsWithoutEditorOnly<Animator>(true);
                audioSources = targetObject.GetAllComponentsWithoutEditorOnly<AudioSource>(true);
                cameras = targetObject.GetAllComponentsWithoutEditorOnly<Camera>(true);
                cloths = targetObject.GetAllComponentsWithoutEditorOnly<Cloth>(true);
                colliders = targetObject.GetAllComponentsWithoutEditorOnly<Collider>(true);
                flareLayers = targetObject.GetAllComponentsWithoutEditorOnly<FlareLayer>(true);

                characterJoints = targetObject.GetAllComponentsWithoutEditorOnly<CharacterJoint>(true);
                configurableJoints = targetObject.GetAllComponentsWithoutEditorOnly<ConfigurableJoint>(true);
                fixedJoints = targetObject.GetAllComponentsWithoutEditorOnly<FixedJoint>(true);
                hingeJoints = targetObject.GetAllComponentsWithoutEditorOnly<HingeJoint>(true);
                springJoints = targetObject.GetAllComponentsWithoutEditorOnly<SpringJoint>(true);

                lights = targetObject.GetAllComponentsWithoutEditorOnly<Light>(true);

                meshFilters = targetObject.GetAllComponentsWithoutEditorOnly<MeshFilter>(true);
                meshRenderers = targetObject.GetAllComponentsWithoutEditorOnly<MeshRenderer>(true);
                skinnedMeshRenderers = targetObject.GetAllComponentsWithoutEditorOnly<SkinnedMeshRenderer>(true);
                trailRenderers = targetObject.GetAllComponentsWithoutEditorOnly<TrailRenderer>(true);
                lineRenderers = targetObject.GetAllComponentsWithoutEditorOnly<LineRenderer>(true);

                particleSystemRenderers = targetObject.GetAllComponentsWithoutEditorOnly<ParticleSystemRenderer>(true);
                particleSystems = targetObject.GetAllComponentsWithoutEditorOnly<ParticleSystem>(true);

                rigidbodies = targetObject.GetAllComponentsWithoutEditorOnly<Rigidbody>(true);
            }
        }
    }
}