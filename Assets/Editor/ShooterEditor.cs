using System;
using BehaviorScripts;
using ManagerScripts;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(ShooterBehavior), true)]
    public class ShooterEditor : UnityEditor.Editor
    {
        private SerializedProperty _damageMultiplier;
        private SerializedProperty _attackSpeedMultiplier;
        private SerializedProperty _additionalBullets;
        
        private SerializedProperty _sightMultiplier;
        
        //Array of all properties shared by both enemies and the player
        private SerializedProperty[] _sharedProperties;// = new SerializedProperty[6];


        //Is this an enemy object?
        private void OnEnable()
        {

            try
            {
                _sightMultiplier = serializedObject.FindProperty("sightMultiplier");

                _damageMultiplier = serializedObject.FindProperty("damageMultiplier");
                _attackSpeedMultiplier = serializedObject.FindProperty("attackSpeedMultiplier");
                _additionalBullets = serializedObject.FindProperty("additionalBulletsFired");

            
                _sharedProperties = new []
                {
                    serializedObject.FindProperty("lockingRangeMultiplier"),
                    serializedObject.FindProperty("bulletSpeedMultiplier"),
                    serializedObject.FindProperty("noiseMultiplier"),
                    
                };
            }
            catch (Exception)
            {
                //Nearly every line above raises a SerializedObjectNotCreateableException, whose type I can't access
                //(which is why a general Exception is being caught instead)
                //I can't find any information on it, but suppressing it like this doesn't cause any noticeable issues.
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var shooter = serializedObject.targetObject as ShooterBehavior;
            if (shooter.transform.parent)
            {
                var owner = shooter.transform.parent.GetComponent<Entity>();
                if (owner is EnemyScript e)
                {
                    EditorGUILayout.PropertyField(_sightMultiplier);
                    if (e.isElite)
                    {
                        EditorGUILayout.PropertyField(_attackSpeedMultiplier);
                        EditorGUILayout.PropertyField(_damageMultiplier);
                        EditorGUILayout.PropertyField(_additionalBullets);
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Attack Speed Multiplier {_attackSpeedMultiplier.floatValue}");
                        EditorGUILayout.LabelField($"Damage Multiplier {_damageMultiplier.floatValue}");
                        EditorGUILayout.LabelField($"Additional Bullets {_additionalBullets.intValue}");
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(_attackSpeedMultiplier);
                    EditorGUILayout.PropertyField(_damageMultiplier);
                    EditorGUILayout.PropertyField(_additionalBullets);
                }
                
                EditorGUILayout.LabelField("Damage Per Second", $"{shooter.uiDamagePerSecond}");
                EditorGUILayout.LabelField("Attacks Per Second", $"{shooter.uiAttacksPerSecond}");
            }
            else
            {
                EditorGUILayout.PropertyField(_attackSpeedMultiplier);
                EditorGUILayout.PropertyField(_damageMultiplier);
                EditorGUILayout.PropertyField(_additionalBullets);
                
            }

            foreach (var p in _sharedProperties)
            {
                EditorGUILayout.PropertyField(p);
            }
            
            
            serializedObject.ApplyModifiedProperties();
        }
    }

}