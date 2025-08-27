namespace CreditFeeManagement.Models
{
    public enum StudyType
    {
        HocLan1 = 1,
        HocLan2 = 2,
        HocCaiThien = 3
    }
    
    public static class StudyTypeExtensions
    {
        public static string GetDisplayName(this StudyType studyType)
        {
            return studyType switch
            {
                StudyType.HocLan1 => "Học lần 1",
                StudyType.HocLan2 => "Học lần 2", 
                StudyType.HocCaiThien => "Học cải thiện",
                _ => studyType.ToString()
            };
        }
        
        public static string GetShortName(this StudyType studyType)
        {
            return studyType switch
            {
                StudyType.HocLan1 => "L1",
                StudyType.HocLan2 => "L2",
                StudyType.HocCaiThien => "CT",
                _ => studyType.ToString()
            };
        }
    }
}