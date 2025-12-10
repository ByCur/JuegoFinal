using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Controlador))]
public class PrometeoEditor : Editor
{
    private SerializedObject SO;

    // ➤ MULTIJUGADOR
    private SerializedProperty isPlayer2;
    private SerializedProperty keyForward;
    private SerializedProperty keyReverse;
    private SerializedProperty keyLeft;
    private SerializedProperty keyRight;
    private SerializedProperty keyHandbrake;

    // CAR SETUP
    private SerializedProperty maxSpeed;
    private SerializedProperty maxReverseSpeed;
    private SerializedProperty accelerationMultiplier;
    private SerializedProperty maxSteeringAngle;
    private SerializedProperty steeringSpeed;
    private SerializedProperty brakeForce;
    private SerializedProperty decelerationMultiplier;
    private SerializedProperty handbrakeDriftMultiplier;
    private SerializedProperty bodyMassCenter;

    // WHEELS
    private SerializedProperty frontLeftMesh;
    private SerializedProperty frontLeftCollider;
    private SerializedProperty frontRightMesh;
    private SerializedProperty frontRightCollider;
    private SerializedProperty rearLeftMesh;
    private SerializedProperty rearLeftCollider;
    private SerializedProperty rearRightMesh;
    private SerializedProperty rearRightCollider;

    // EFFECTS
    private SerializedProperty useEffects;
    private SerializedProperty RLWParticleSystem;
    private SerializedProperty RRWParticleSystem;
    private SerializedProperty RLWTireSkid;
    private SerializedProperty RRWTireSkid;

    // UI
    private SerializedProperty useUI;
    private SerializedProperty carSpeedText;

    // SOUNDS
    private SerializedProperty useSounds;
    private SerializedProperty carEngineSound;
    private SerializedProperty tireScreechSound;

    private void OnEnable()
    {
        SO = new SerializedObject(target);

        // ➤ MULTIJUGADOR
        isPlayer2 = SO.FindProperty("isPlayer2");
        keyForward = SO.FindProperty("keyForward");
        keyReverse = SO.FindProperty("keyReverse");
        keyLeft = SO.FindProperty("keyLeft");
        keyRight = SO.FindProperty("keyRight");
        keyHandbrake = SO.FindProperty("keyHandbrake");

        // CAR SETUP
        maxSpeed = SO.FindProperty("maxSpeed");
        maxReverseSpeed = SO.FindProperty("maxReverseSpeed");
        accelerationMultiplier = SO.FindProperty("accelerationMultiplier");
        maxSteeringAngle = SO.FindProperty("maxSteeringAngle");
        steeringSpeed = SO.FindProperty("steeringSpeed");
        brakeForce = SO.FindProperty("brakeForce");
        decelerationMultiplier = SO.FindProperty("decelerationMultiplier");
        handbrakeDriftMultiplier = SO.FindProperty("handbrakeDriftMultiplier");
        bodyMassCenter = SO.FindProperty("bodyMassCenter");

        // WHEELS
        frontLeftMesh = SO.FindProperty("frontLeftMesh");
        frontLeftCollider = SO.FindProperty("frontLeftCollider");
        frontRightMesh = SO.FindProperty("frontRightMesh");
        frontRightCollider = SO.FindProperty("frontRightCollider");
        rearLeftMesh = SO.FindProperty("rearLeftMesh");
        rearLeftCollider = SO.FindProperty("rearLeftCollider");
        rearRightMesh = SO.FindProperty("rearRightMesh");
        rearRightCollider = SO.FindProperty("rearRightCollider");

        // EFFECTS
        useEffects = SO.FindProperty("useEffects");
        RLWParticleSystem = SO.FindProperty("RLWParticleSystem");
        RRWParticleSystem = SO.FindProperty("RRWParticleSystem");
        RLWTireSkid = SO.FindProperty("RLWTireSkid");
        RRWTireSkid = SO.FindProperty("RRWTireSkid");

        // UI
        useUI = SO.FindProperty("useUI");
        carSpeedText = SO.FindProperty("carSpeedText");

        // SOUNDS
        useSounds = SO.FindProperty("useSounds");
        carEngineSound = SO.FindProperty("carEngineSound");
        tireScreechSound = SO.FindProperty("tireScreechSound");
    }

    public override void OnInspectorGUI()
    {
        SO.Update();

        //
        // PLAYER CONTROLS
        //
        GUILayout.Space(20);
        GUILayout.Label("PLAYER CONTROLS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isPlayer2);
        GUILayout.Space(8);
        EditorGUILayout.PropertyField(keyForward);
        EditorGUILayout.PropertyField(keyReverse);
        EditorGUILayout.PropertyField(keyLeft);
        EditorGUILayout.PropertyField(keyRight);
        EditorGUILayout.PropertyField(keyHandbrake);

        //
        // CAR SETUP
        //
        GUILayout.Space(20);
        GUILayout.Label("CAR SETUP", EditorStyles.boldLabel);
        EditorGUILayout.IntSlider(maxSpeed, 20, 190, new GUIContent("Max Speed"));
        EditorGUILayout.IntSlider(maxReverseSpeed, 10, 120, new GUIContent("Max Reverse Speed"));
        EditorGUILayout.IntSlider(accelerationMultiplier, 1, 10, new GUIContent("Acceleration Multiplier"));
        EditorGUILayout.IntSlider(maxSteeringAngle, 10, 45, new GUIContent("Max Steering Angle"));
        EditorGUILayout.Slider(steeringSpeed, 0.1f, 1f, new GUIContent("Steering Speed"));
        EditorGUILayout.IntSlider(brakeForce, 100, 600, new GUIContent("Brake Force"));
        EditorGUILayout.IntSlider(decelerationMultiplier, 1, 10, new GUIContent("Deceleration Multiplier"));
        EditorGUILayout.IntSlider(handbrakeDriftMultiplier, 1, 10, new GUIContent("Drift Multiplier"));
        EditorGUILayout.PropertyField(bodyMassCenter);

        //
        // WHEELS
        //
        GUILayout.Space(20);
        GUILayout.Label("WHEELS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(frontLeftMesh);
        EditorGUILayout.PropertyField(frontLeftCollider);
        EditorGUILayout.PropertyField(frontRightMesh);
        EditorGUILayout.PropertyField(frontRightCollider);
        EditorGUILayout.PropertyField(rearLeftMesh);
        EditorGUILayout.PropertyField(rearLeftCollider);
        EditorGUILayout.PropertyField(rearRightMesh);
        EditorGUILayout.PropertyField(rearRightCollider);

        //
        // EFFECTS
        //
        GUILayout.Space(20);
        GUILayout.Label("EFFECTS", EditorStyles.boldLabel);
        useEffects.boolValue = EditorGUILayout.BeginToggleGroup("Use Effects?", useEffects.boolValue);
        EditorGUILayout.PropertyField(RLWParticleSystem);
        EditorGUILayout.PropertyField(RRWParticleSystem);
        EditorGUILayout.PropertyField(RLWTireSkid);
        EditorGUILayout.PropertyField(RRWTireSkid);
        EditorGUILayout.EndToggleGroup();

        //
        // UI
        //
        GUILayout.Space(20);
        GUILayout.Label("UI", EditorStyles.boldLabel);
        useUI.boolValue = EditorGUILayout.BeginToggleGroup("Use UI?", useUI.boolValue);
        EditorGUILayout.PropertyField(carSpeedText);
        EditorGUILayout.EndToggleGroup();

        //
        // SOUNDS
        //
        GUILayout.Space(20);
        GUILayout.Label("SOUNDS", EditorStyles.boldLabel);
        useSounds.boolValue = EditorGUILayout.BeginToggleGroup("Use Sounds?", useSounds.boolValue);
        EditorGUILayout.PropertyField(carEngineSound);
        EditorGUILayout.PropertyField(tireScreechSound);
        EditorGUILayout.EndToggleGroup();

        SO.ApplyModifiedProperties();
    }
}
