using System.Collections.Generic;

public static class PlayerSchool {
    private static string schoolName = "Priscus' School";
    private static List<UnitObject> roster = new List<UnitObject>();

    public static string SchoolName {
        get {
            return schoolName;
        }
        set {
            schoolName = value;
        }
    }

    public static List<UnitObject> Roster {
        get {
            return roster;
        }
        set {
            roster = value;
        }
    }
}