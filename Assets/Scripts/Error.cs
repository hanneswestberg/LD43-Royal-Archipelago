using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Error {

    public string id;
    public string effect;
    public int cost;
    
    // Different kinds of errors are:
    // 0: Hull Damage: -1 speed
    // 1: Balloon Damage: sink speed +1
    // 2: Kitchen Damage: Increased Hunger speed
    // 3: Workshop Damage: Decresed production speed

}
