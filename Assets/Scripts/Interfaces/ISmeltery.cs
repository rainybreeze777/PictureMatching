using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISmeltery {

    // karma parameter is the total karma points that karma items give
    Weapon SmeltWeapon(List<int> spentEssence,int karma);

}
