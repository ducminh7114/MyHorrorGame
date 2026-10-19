using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public bool hasSave = false;

    public string sceneName = "";

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public float playerRotX;
    public float playerRotY;
    public float playerRotZ;

    public List<string> completedTriggers = new List<string>();
    public List<ObjectStateData> objectStates = new List<ObjectStateData>();

    public WeaponInventorySaveData weaponInventory = new WeaponInventorySaveData();
    public List<string> collectedWeaponPickupIDs = new List<string>();

}

[Serializable]
public class ObjectStateData
{
    public string id;
    public bool active;
}