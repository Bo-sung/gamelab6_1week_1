using UnityEngine;

public enum AugmentType
{
    //스탯 관련
    SpeedUp,
    AccelerationUp,
    BrakeUp,
    CorneringUp,
    WeightUp,
    LoseWheel,
    ThrowBreak,
    SpringBumper
}


[CreateAssetMenu(fileName = "Augment", menuName = "Scriptable Objects/Augment")]
public class Augment : ScriptableObject
{
    public string augmentName;
    public string description;
    //TODO : 증강 최대 한계를 게임매니저에서 관리할 것인지 증강매니저에서 관리할 것인지 등  
    public int limit;
    public AugmentType type;
    public float value;
}   

