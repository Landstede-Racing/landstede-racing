using System.Collections.Generic;

public enum Locationa 
{
    MGU_K,
    MGU_H,
    ICE,
    EnergyStore,
    Gearbox,
    TurboCharger,
    ControlElectronics,
    FrontLeftWing,
    FrontRightWing,
    RearWing,
    Underbody,
    FrontLeftWheel,
    FrontLeftBrake,
    FrontRightWheel,
    FrontRightBrake,
    RearLeftWheel,
    RearLeftBrake,
    RearRightWheel,
    RearRightBrake,
    LeftSidepod,
    RightSidepod,
    Body,
}

public class Location
{
    public string name;
}

public class Locations
{
    public static Location MGU_K = new() { name = "MGU_K"};
    public static Location MGU_H = new() { name = "MGU_H"};
    public static Location ICE = new() { name = "ICE"};
    public static Location EnergyStore = new() { name = "EnergyStore"};
    public static Location Gearbox = new() { name = "Gearbox"};
    public static Location TurboCharger = new() { name = "TurboCharger"};
    public static Location ControlElectronics = new() { name = "ControlElectronics"};
    public static Location FrontLeftWing = new() { name = "FrontLeftWing"};
    public static Location FrontRightWing = new() { name = "FrontRightWing"};
    public static Location RearWing = new() { name = "RearWing"};
    public static Location Underbody = new() { name = "Underbody"};
    public static Location FrontLeftWheel = new() { name = "FrontLeftWheel"};
    public static Location FrontLeftBrake = new() { name = "FrontLeftBrake"};
    public static Location FrontRightWheel = new() { name = "FrontRightWheel"};
    public static Location FrontRightBrake = new() { name = "FrontRightBrake"};
    public static Location RearLeftWheel = new() { name = "RearLeftWheel"};
    public static Location RearLeftBrake = new() { name = "RearLeftBrake"};
    public static Location RearRightWheel = new() { name = "RearRightWheel"};
    public static Location RearRightBrake = new() { name = "RearRightBrake"};
    public static Location LeftSidepod = new() { name = "LeftSidepod"};
    public static Location RightSidepod = new() { name = "RightSidepod"};
    public static Location Body = new() { name = "Body"};

    public static IEnumerable<Location> Values
    {
        get
        {
            yield return MGU_K;
            yield return MGU_H;
            yield return ICE;
            yield return EnergyStore;
            yield return Gearbox;
            yield return TurboCharger;
            yield return ControlElectronics;
            yield return FrontLeftWing;
            yield return FrontRightWing;
            yield return RearWing;
            yield return Underbody;
            yield return FrontLeftWheel;
            yield return FrontLeftBrake;
            yield return FrontRightWheel;
            yield return FrontRightBrake;
            yield return RearLeftWheel;
            yield return RearLeftBrake;
            yield return RearRightWheel;
            yield return RearRightBrake;
            yield return LeftSidepod;
            yield return RightSidepod;
            yield return Body;
        }
    }
}