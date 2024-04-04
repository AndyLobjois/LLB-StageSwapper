using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace LLB_StageSwapper
{
    [Serializable]
    public class StageSettings
    {
        public int ShadowDistance;
        public float CameraPositionY;
        [XmlArray("NormalObjects")]
        [XmlArrayItem("string")]
        public string[] normal;
        [XmlArray("EclipseObjects")]
        [XmlArrayItem("string")]
        public string[] eclipse;
        [XmlArray("IntroCamShots")]
        [XmlArrayItem("CamShot")]
        public CamShot[] shots;
    }

    [Serializable]
    public class CamShot
    {
        public CamTransform StartCam;
        public CamTransform EndCam;
        public float Duration;
        public bool Smooth;
    }

    [Serializable]
    public class CamTransform
    {
        public float x;
        public float y;
        public float z;
        public float rotX;
        public float rotY;
        public float rotZ;

        public override string ToString()
        {
            return String.Format("Pos({0},{1},{2}), Rot({3},{4},{5})",
                x, y, z, rotX, rotY, rotZ) ;
        }
    }
}
