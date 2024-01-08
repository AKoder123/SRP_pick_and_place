using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Robotics;
using UnityEngine;

public class PoseSetter : MonoBehaviour
{
    private ArticulationBody[] articulationChain;
    const float k_JointAssignmentWait = 0.1f;
    private ArticulationBody rightFinger;
    private ArticulationBody leftFinger;

    // Start is called before the first frame update
    void Start()
    {
        articulationChain = this.GetComponentsInChildren<ArticulationBody>();
        int defDyanmicVal = 10;
        foreach (ArticulationBody joint in articulationChain)
        {
            joint.gameObject.AddComponent<JointControl>();
            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;
            ArticulationDrive currentDrive = joint.xDrive;
            currentDrive.forceLimit = 10000;
            joint.xDrive = currentDrive;
        }
        leftFinger = articulationChain[11];
        rightFinger = articulationChain[12];
        Debug.Log("before");
        StartCoroutine(SetPose());
        Debug.Log("after");
    }

    IEnumerator SetPose()
    {
        float[] jointPositions = new float[]{0.00019025905158115864f, -0.7850538262986299f, 2.8837701730560838e-05f, -2.356203691054924f,
        -0.00016141187679397914f, 1.5732667469142465f, 0.7851245770537191f};

        var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

        // Set the joint values for every joint
        for (var joint = 1; joint < 8; joint++)
        {
            // Debug.Log(articulationChain[joint].name);
            var joint1XDrive = articulationChain[joint].xDrive;
            joint1XDrive.target = result[joint - 1];
            articulationChain[joint].xDrive = joint1XDrive;
        }
        
        var rightFingerDrive = rightFinger.xDrive;
        rightFingerDrive.target = 0.04f;
        rightFinger.xDrive = rightFingerDrive;

        var leftFingerDrive = leftFinger.xDrive;
        leftFingerDrive.target = 0.04f;
        leftFinger.xDrive = leftFingerDrive;

        // Wait for robot to achieve pose for all joint assignments
        yield return new WaitForSeconds(k_JointAssignmentWait);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
