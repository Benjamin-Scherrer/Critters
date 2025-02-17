using System.Collections.Generic;
using UnityEngine;

public class ConglomerateManager : MonoBehaviour
{
    [Header("Movmeent")]

    //Main head gains extra force to pull the rest of the geobodies based on amount of geobodies in the conglomerate.
    [SerializeField] private AnimationCurve mainHeadMainMovmementMult = AnimationCurve.Linear(0, 1, 10, 10);
    [SerializeField] private AnimationCurve mainHeadFloatingMult = AnimationCurve.Linear(0, 1, 10, 10);

    //To be decided. There also needs to be some rule about the floating. 
    [SerializeField] private AnimationCurve sideHeadMainMovementMult = AnimationCurve.Linear(0, 0.5f, 10, 5);
    [SerializeField] private AnimationCurve sideHeadFloatingMult = AnimationCurve.Linear(0, 0.5f, 10, 5);

    [Header("Hierarchy")]
    [SerializeField] private int sideHeadInterval = 3;

    private  List<Geobody> Geobodies = new();
    
    //I need a systemt that decides on a main head.
    //from that main head, i will follow each connection and set side heads every few geobodies
    //all other geobodies will be limbs, besides the ends of the conglomerate, which also turn into side heads.
    //this processs needs to be repeated every time a geobody is added to the conglomerate.

    //PUBLIC
    public void CreateConglomerate(Geobody geobody)
    {
        if(Geobodies.Count != 0) throw new System.Exception("Conglomerate already exists");

        Geobodies.Add(geobody);
        //geobody.JointSnapped += OnJointSnapped;

        //Set this one geobody to head,
        //extra force multiplier is 1 because there are no other geobodies to pull.
        //geobody.SetToMainHead(1); 
        //-> No need for that, as it can be handled in the UpdateGeobodyHierarchy method.
        //~ unclear how it goes yet, especially efficiency wise.

        GoThroughConglomerateHierarchyStart();
    }

    public void DestroyConglemerate()
    {
        foreach(Geobody geobody in Geobodies)
        {
            //geobody.JointSnapped -= OnJointSnapped;
            geobody.SetToSeparate();
        }

        Geobodies.Clear();
    }

    public void OnJointSnapped(Geobody geobody)
    {
        if(Geobodies.IndexOf(geobody) != -1) return;

        Geobodies.Add(geobody);
        //geobody.JointSnapped += OnJointSnapped;

        //geobody.SetToLimb(); not really needed as the thing will go through the hierarchy anyway.

        Debug.Log("On joint snappped");
        GoThroughConglomerateHierarchyStart();
    }

    //PRIVATE

    private void GoThroughConglomerateHierarchyStart()
    {
        //Start at main head -> Which is at the start of the list
        if (Geobodies.Count == 0) throw new System.Exception("Conglomerate not created");

        List<Geobody> geobodiesAlreadyReached = new(Geobodies.Count);

        Geobody geobody = Geobodies[0];
        if(geobody == null) throw new System.Exception("Main head is null");
        geobody.SetToMainHead(this, mainHeadMainMovmementMult.Evaluate(Geobodies.Count), mainHeadFloatingMult.Evaluate(Geobodies.Count));
        geobodiesAlreadyReached.Add(geobody);


        Geobody[] tempGeobodies = geobody.GetSnappedGeobodies();
        Debug.Log("Starting to go through hierarchy. TempGeobodies.Lenght: " + tempGeobodies.Length);
        foreach (Geobody g in tempGeobodies)
        {
            Debug.Log("before hierarchy step 1");
            if(g == null) throw new System.Exception("Geobody is null");
            Debug.Log("before hierarchy step 2");
            if (g == geobody) 
            {
                Debug.Log("Geobody g is the same as the main head");
                continue;
            };   
            Debug.Log("before hierarchy step 3");
            GoThroughConglomerateHierarchyStep(0, 0, g, geobodiesAlreadyReached);
        }


        //End error checks
        //Commented for now: TODO: Uncomment when needed.
        //if(geobodiesAlreadyReached.Count < Geobodies.Count)
        //{
        //    foreach (Geobody g in Geobodies)
        //    {
        //        if (geobodiesAlreadyReached.IndexOf(g) == -1) DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.magenta);
        //        else 
        //            DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.green);
        //    }

        //    throw new System.Exception("Not all geobodies reached. Amount of Geobodies reached: " + geobodiesAlreadyReached.Count 
        //        + ", Geobodies needed:" + Geobodies.Count);

        //    //This case can be reached by having a geobody that just connected to the conglomerate,
        //    //but the conglomerate hasn't connected back yet,
        //    //causing a confusion to happen, as this search does not find the new geobody.

        //}

        //TODO: Make snapping robust enough to make this possible.
        ////this one here is important though because that would be looping which is very bad. //not yet ~because the snapping isn't limited yet.
        //if (geobodiesAlreadyReached.Count > Geobodies.Count)
        //{
        //    foreach (Geobody g in geobodiesAlreadyReached)
        //    {
        //        if (Geobodies.IndexOf(g) == -1) DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.magenta);
        //        else
        //            DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.green);
        //    }

        //    //Too many geobodies reached.
        //    throw new System.Exception("Too many geobodies reached. Amount of Geobodies reached: " + geobodiesAlreadyReached.Count
        //        + ", Geobodies needed:" + Geobodies.Count);
        //}

        Debug.Log("End");
    }

    private void GoThroughConglomerateHierarchyStep(int chainSegmentIndex, int chainDepth, Geobody geobody, List<Geobody> geobodiesAlreadyReached)
    {
        Debug.Log("Starting hierarchy step. chainSegmentIndex: " + chainSegmentIndex + ", chainDepth: " + chainDepth);
        if (chainSegmentIndex < 0 ) throw new System.Exception("Index out of bounds");
        if(chainSegmentIndex > sideHeadInterval) throw new System.Exception("Index out of bounds");
        if (chainDepth < 0) throw new System.Exception("Depth out of bounds");
        if(geobody == null) throw new System.Exception("Geobody is null");

        if (geobodiesAlreadyReached.IndexOf(geobody) != -1) //throw new System.Exception("Geobody already reached");
        {
            Debug.LogFormat("Geobody already reached. chainSegmentIndex: {0}, chainDepth {1}.", chainSegmentIndex, chainDepth);
            DebugUtilityBenjamin.Draw3DCross(geobody.transform.position + Vector3.one * 0.1f, 0.1f, Color.cyan);
            return;
        }
        geobodiesAlreadyReached.Add(geobody);

        chainSegmentIndex++;
        if (chainSegmentIndex < sideHeadInterval)
        {
            Debug.Log("Set to limb");
            geobody.SetToLimb(this);
        }
        else
        {
            Debug.Log("Set to Side Head");
            chainSegmentIndex = 0;
            chainDepth++;
            geobody.SetToSideHead(this, sideHeadMainMovementMult.Evaluate(chainDepth), sideHeadFloatingMult.Evaluate(chainDepth));
        }

        Geobody[] tempGeobodies = geobody.GetSnappedGeobodies();
        Debug.Log("Going through hierarchy step. TempGeobodies.Lenght: " + tempGeobodies.Length);
        foreach (Geobody g in tempGeobodies)
        {
            if (g == null) throw new System.Exception("Geobody is null");
            if (g == geobody) continue;
            GoThroughConglomerateHierarchyStep(chainSegmentIndex, chainDepth, g, geobodiesAlreadyReached);
        }
    }


    ////Depreciated
    //private void SetMainHeadBehaviour()
    //{
    //    //the firs geobody in the list is always the main head
    //    //it will get a movement behaviour based on other systems
    //    //and a specific amount of force to make sure it can pull the rest of the geobodies with it.

    //}

    //private void SetSideHeadBehaviour()
    //{
    //    //Side heads will be set every few geobodies down the chain 
    //    //they have their own movement behaviour based on the main head, adding some variaty into the movement
    //    //They have less force as they are not the driving force of the conglomerate..
    //}

    //private void SetLimbBehaviour()
    //{
    //    //limbs have no movement behaviour, they are just there to connect the side heads to the main head.
    //}
}
