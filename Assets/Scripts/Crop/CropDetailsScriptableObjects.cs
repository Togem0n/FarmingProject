using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "ScriptableObjects/Crop/Crop Details List")]
public class CropDetailsScriptableObjects : ScriptableObject
{
    [SerializeField] public List<CropDetails> cropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return cropDetails.Find(x => x.seedItemCode == seedItemCode);
    }
}
