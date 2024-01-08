using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.PandaMoveit;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class SourceDestinationPublisher : MonoBehaviour
{
    const int k_NumRobotJoints = 7;

    public static readonly string[] LinkNames =
        { "panda_link0/panda_link1", "/panda_link2", "/panda_link3", "/panda_link4", "/panda_link5", "/panda_link6", "/panda_link7"};

    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/niryo_joints";

    [SerializeField]
    GameObject m_Panda;
    // GameObject m_Target;
    // [SerializeField]
    // GameObject m_TargetPlacement;
    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);

    // Robot Joints
    UrdfJointRevolute[] m_JointArticulationBodies;

    // ROS Connector
    ROSConnection m_Ros;

    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<PandaMoveitJointsMsg>(m_TopicName);

        m_JointArticulationBodies = new UrdfJointRevolute[k_NumRobotJoints];

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += LinkNames[i];
            m_JointArticulationBodies[i] = m_Panda.transform.Find(linkName).GetComponent<UrdfJointRevolute>();
        }
        
    }

    public void Publish()
    {
        var sourceDestinationMessage = new PandaMoveitJointsMsg();
        
        // for (var i = 0; i < k_NumRobotJoints; i++)
        // {
        //     Debug.Log(i);
        //     Debug.Log(m_JointArticulationBodies[i].GetPosition());
        // }
        
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            sourceDestinationMessage.joints[i] = m_JointArticulationBodies[i].GetPosition();
        }

        // Pick Pose
        // sourceDestinationMessage.pick_pose = new PoseMsg
        // {
        //     position = m_Target.transform.position.To<FLU>(),
        //     orientation = Quaternion.Euler(90, m_Target.transform.eulerAngles.y, 0).To<FLU>()
        // };

        // Place Pose
        // sourceDestinationMessage.place_pose = new PoseMsg
        // {
        //     position = m_TargetPlacement.transform.position.To<FLU>(),
        //     orientation = m_PickOrientation.To<FLU>()
        // };

        // Finally send the message to server_endpoint.py running in ROS
        m_Ros.Publish(m_TopicName, sourceDestinationMessage);
    }
}
