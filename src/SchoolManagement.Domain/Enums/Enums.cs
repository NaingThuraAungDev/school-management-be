namespace SchoolManagement.Domain.Enums;

public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2
}

public enum GuardianRelationship
{
    Father = 0,
    Mother = 1,
    Guardian = 2,
    Other = 3
}

public enum DocumentType
{
    BirthCertificate = 0,
    PreviousRecords = 1,
    TransferCertificate = 2,
    Photo = 3,
    Other = 4
}

public enum StaffType
{
    Teacher = 0,
    Admin = 1,
    Support = 2
}

public enum StaffRoleType
{
    ClassTeacher = 0,
    HOD = 1,
    Admin = 2,
    SubjectTeacher = 3,
    Principal = 4
}

public enum UserType
{
    Student = 0,
    Staff = 1,
    Admin = 2,
    Parent = 3
}

public enum DayOfWeekEnum
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public enum ExamTermType
{
    MidTerm = 0,
    Final = 1,
    Quarterly = 2,
    HalfYearly = 3
}
