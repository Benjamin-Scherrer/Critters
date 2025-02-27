using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CongomerateAllotmentScript: ScriptableObject
{
    public abstract void RunConglomerateAlotmentScript(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> conglomerateGeobodies, List<List<Geobody>> geobodyLayers);
}
