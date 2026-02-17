using System.Linq;
using UnityEditor;
using UnityEngine;
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
        //

        [MenuItem("Tools/Satania/VRC Component Counter")]
        private static void Open()
        {
            GetWindow<VRCComponentCounterWindow>("VRC Component Counter");
        }

        private void OnEnable()
        {
            targetObject = null;
            ClearArrays();
        }

        private void ClearArrays()
        {
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
                EditorGUILayout.LabelField($"vrcAvatarDescriptors:{vrcAvatarDescriptors.Length}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"vrcAimConstraints:{vrcAimConstraints.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcLookAtConstraints:{vrcLookAtConstraints.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcParentConstraints:{vrcParentConstraints.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcPositionConstraints:{vrcPositionConstraints.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcRotationConstraints:{vrcRotationConstraints.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcScaleConstraints:{vrcScaleConstraints.Length}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"vrcContactReceivers:{vrcContactReceivers.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcContactSenders:{vrcContactSenders.Length}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"vrcHeadChops:{vrcHeadChops.Length}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"vrcPhysBones:{vrcPhysBones.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcPhysBoneColliders:{vrcPhysBoneColliders.Length}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField($"vrcPipelineManagers:{vrcPipelineManagers.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcSpatialAudioSources:{vrcSpatialAudioSources.Length}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"vrcStations:{vrcStations.Length}", EditorStyles.boldLabel);

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
    }
}