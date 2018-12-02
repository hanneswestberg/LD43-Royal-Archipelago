using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Difficulty Setting")]
public class DifficultySetting : ScriptableObject {

    [Header("Start Values:")]
    [SerializeField] private int supplies;
    [SerializeField] private int food;
    [SerializeField] private int materials;
    [SerializeField] private int workers;

    [Header("Difficulty Parameters:")]
    [SerializeField] private float sinkSpeed;
    [SerializeField] private float foodProcessTimer;
    [SerializeField] private float materialProcessTimer;
    [SerializeField] private float foodConsumptionTimer;
    [SerializeField] private float errorInterval;
    [SerializeField] private int wasteBoost;
    [SerializeField] private int winDistance;

    public int Supplies { get { return supplies; } }
    public int Food { get { return food; } }
    public int Materials { get { return materials; } }
    public int Workers { get { return workers; } }
    public float SinkSpeed { get { return sinkSpeed; } }
    public float FoodProcessTimer { get { return foodProcessTimer; } }
    public float MaterialProcessTimer { get { return materialProcessTimer; } }
    public float FoodConsumptionTimer { get { return foodConsumptionTimer; } }
    public float ErrorInterval { get { return errorInterval; } }
    public int WasteBoost { get { return wasteBoost; } }
    public int WinDistance { get { return winDistance; } }
}
