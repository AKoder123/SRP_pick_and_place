using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Robotics;
using UnityEngine;
using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.PandaMoveit;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;

public class Moving : MonoBehaviour
{
    [SerializeField]
    GameObject m_Panda;

    private ArticulationBody[] articulationChain;
    const float k_JointAssignmentWait = 0.1f;
    private ArticulationBody rightFinger;
    private ArticulationBody leftFinger;

    const int k_NumRobotJoints = 9;

    public static readonly string[] LinkNames =
        { "panda_link0/panda_link1", "/panda_link2", "/panda_link3", "/panda_link4", "/panda_link5", "/panda_link6", "/panda_link7"};

    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/niryo_joints";

    // GameObject m_Target;
    // [SerializeField]
    // GameObject m_TargetPlacement;
    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);

    // Robot Joints
    UrdfJointRevolute[] m_JointArticulationBodies;

    // ROS Connector
    ROSConnection m_Ros;

    // Start is called before the first frame update
    void Start()
    {
        articulationChain = m_Panda.GetComponentsInChildren<ArticulationBody>();
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
        // StartCoroutine(SetPose());
        Debug.Log("after");

        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<PandaMoveitJointsMsg>(m_TopicName);

        

        
        
    }

    IEnumerator SetPose()
    {
        float[] jointPositions = new float[]{0.6016683664125085f, 0.2107767205405653f, -0.1522421624414099f, -2.5761598455864085f, 0.02502738476344136f,
        2.914097084124883f, 1.2369475778686148f};

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
        rightFingerDrive.target = 0.04016074910759926f;
        rightFinger.xDrive = rightFingerDrive;

        var leftFingerDrive = leftFinger.xDrive;
        leftFingerDrive.target = 0.04016074910759926f;
        leftFinger.xDrive = leftFingerDrive;

        // Wait for robot to achieve pose for all joint assignments
        yield return new WaitForSeconds(k_JointAssignmentWait);
    }


    public void Move_left(float gripper) 
    {
        float[] jointPositions = new float[]{ 0.7265509473733734f, 0.31427018733610185f, -0.2680263785437534f, -2.467356380931118f,
            0.024662607025776107f, 2.819748551766078f, 1.1758973142251432f, gripper, gripper};

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
        rightFingerDrive.target = gripper;
        rightFinger.xDrive = rightFingerDrive;

        var leftFingerDrive = leftFinger.xDrive;
        leftFingerDrive.target = gripper;
        leftFinger.xDrive = leftFingerDrive;

        var sourceDestinationMessage = new PandaMoveitJointsMsg();

        

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            sourceDestinationMessage.joints[i] = jointPositions[i];
        }
        m_Ros.Publish(m_TopicName, sourceDestinationMessage);
    }

    public void Move_right(float gripper)
    {
        
        float[] jointPositions = new float[]{0.2619991096153594f, 0.023486502526967056f, -0.7306691922891514f, -2.38161687707855f,
        0.040072656255846904f, 2.4436565056641895f, 0.3069542164476351f, gripper, gripper};

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
        rightFingerDrive.target = gripper;
        rightFinger.xDrive = rightFingerDrive;

        var leftFingerDrive = leftFinger.xDrive;
        leftFingerDrive.target = gripper;
        leftFinger.xDrive = leftFingerDrive;

        var sourceDestinationMessage = new PandaMoveitJointsMsg();
        
        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            sourceDestinationMessage.joints[i] = jointPositions[i];
        }
        m_Ros.Publish(m_TopicName, sourceDestinationMessage);
    }

    public void Ready(float gripper)
    {
        float[] jointPositions = new float[]{0.00019025905158115864f, -0.7850538262986299f, 2.8837701730560838e-05f, -2.356203691054924f,
        -0.00016141187679397914f, 1.5732667469142465f, 0.7851245770537191f, gripper, gripper};    

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
        rightFingerDrive.target = gripper;
        rightFinger.xDrive = rightFingerDrive;

        var leftFingerDrive = leftFinger.xDrive;
        leftFingerDrive.target = gripper;
        leftFinger.xDrive = leftFingerDrive;

        var sourceDestinationMessage = new PandaMoveitJointsMsg();
        
        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            sourceDestinationMessage.joints[i] = jointPositions[i];
        }
        m_Ros.Publish(m_TopicName, sourceDestinationMessage);
    }

    public void Pick_and_place()
    {
        float grip = 0.0195f;

        Ready(0.04f);

        Move_left(grip);

        Ready(grip);

        Move_left(0.04f);

        Ready(0.04f);

    }



    // Update is called once per frame
    void Update()
    {

    }
}
