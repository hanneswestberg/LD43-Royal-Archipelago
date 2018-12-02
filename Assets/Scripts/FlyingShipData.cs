using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShipData {
    // All the data is stored here. No logic

    // Resources:
    [HideInInspector] public int supplies;
    [HideInInspector] public int food;
    [HideInInspector] public int materials;
    [HideInInspector] public int waste;
    [HideInInspector] public List<Worker> workers = new List<Worker>();

    // Status:
    [HideInInspector] public float altitude;
    [HideInInspector] public float distance;
    [HideInInspector] public float speed;
    [HideInInspector] public float currentWeight;
    [HideInInspector] public float maximumWeight;
    [HideInInspector] public float flightTime;

    // Personell:
    [HideInInspector] public List<Worker> assignedKitchen = new List<Worker>();
    [HideInInspector] public List<Worker> assignedWorkshop = new List<Worker>();
    [HideInInspector] public List<Worker> assignedEngines = new List<Worker>();


}
