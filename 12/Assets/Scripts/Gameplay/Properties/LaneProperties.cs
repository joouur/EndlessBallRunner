using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallRunner.Manager
{
    [System.Serializable]
    public class BaseLane
    {
        public string name;
        public int ID;
        public Vector3 position;
        public Vector3 rotation;
        public int size;
        public Transform Prefab;
        public LaneType laneType;
        //public bool isOn;

        public enum LaneType
        {
            DEBUG_LANE,
            Base_Lane,
            Solo_Lane,
            End_Lane,
            Speed_Lane,
            Stop_Lane
        }

        public BaseLane(Vector3 pos, Vector3 rot, int s, Transform pre, LaneType type)
        {
            position = pos;
            rotation = rot;
            size = s;
            Prefab = pre;
            laneType = type;
            //isOn = false;
        }
    }
}