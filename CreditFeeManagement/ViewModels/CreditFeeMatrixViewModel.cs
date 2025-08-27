using CreditFeeManagement.Models;

namespace CreditFeeManagement.ViewModels
{
    public class CreditFeeMatrixViewModel
    {
        public List<StudentType> StudentTypes { get; set; } = new();
        public List<ModulesType> ModulesTypes { get; set; } = new();
        public Array<StudyType> StudyTypes { get; set; } = Enum.GetValues<StudyType>();
        
        // Dictionary để lưu trữ dữ liệu matrix: [StudentTypeId][ModulesTypeId][StudyType] = FeePerCredit
        public Dictionary<int, Dictionary<int, Dictionary<StudyType, decimal>>> MatrixData { get; set; } = new();
        
        // Properties cho UI
        public int SelectedStudentTypeId { get; set; }
        public bool IsLoading { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        
        // Batch operation properties
        public decimal? FillValue { get; set; }
        public string SelectedRange { get; set; } = string.Empty;
        public List<CellSelection> SelectedCells { get; set; } = new();
        
        public void InitializeMatrix()
        {
            MatrixData.Clear();
            
            foreach (var studentType in StudentTypes)
            {
                MatrixData[studentType.Id] = new Dictionary<int, Dictionary<StudyType, decimal>>();
                
                foreach (var moduleType in ModulesTypes)
                {
                    MatrixData[studentType.Id][moduleType.Id] = new Dictionary<StudyType, decimal>();
                    
                    foreach (StudyType studyType in StudyTypes)
                    {
                        MatrixData[studentType.Id][moduleType.Id][studyType] = 0;
                    }
                }
            }
        }
        
        public decimal GetFee(int studentTypeId, int moduleTypeId, StudyType studyType)
        {
            if (MatrixData.ContainsKey(studentTypeId) &&
                MatrixData[studentTypeId].ContainsKey(moduleTypeId) &&
                MatrixData[studentTypeId][moduleTypeId].ContainsKey(studyType))
            {
                return MatrixData[studentTypeId][moduleTypeId][studyType];
            }
            return 0;
        }
        
        public void SetFee(int studentTypeId, int moduleTypeId, StudyType studyType, decimal fee)
        {
            if (!MatrixData.ContainsKey(studentTypeId))
                MatrixData[studentTypeId] = new Dictionary<int, Dictionary<StudyType, decimal>>();
            
            if (!MatrixData[studentTypeId].ContainsKey(moduleTypeId))
                MatrixData[studentTypeId][moduleTypeId] = new Dictionary<StudyType, decimal>();
            
            MatrixData[studentTypeId][moduleTypeId][studyType] = fee;
        }
        
        public List<CreditFee> ToEntityList()
        {
            var result = new List<CreditFee>();
            
            foreach (var studentTypeKvp in MatrixData)
            {
                foreach (var moduleTypeKvp in studentTypeKvp.Value)
                {
                    foreach (var studyTypeKvp in moduleTypeKvp.Value)
                    {
                        if (studyTypeKvp.Value > 0) // Chỉ lưu những giá trị > 0
                        {
                            result.Add(new CreditFee
                            {
                                StudentTypeId = studentTypeKvp.Key,
                                ModulesTypeId = moduleTypeKvp.Key,
                                StudyType = studyTypeKvp.Key,
                                FeePerCredit = studyTypeKvp.Value
                            });
                        }
                    }
                }
            }
            
            return result;
        }
    }
    
    public class CellSelection
    {
        public int StudentTypeId { get; set; }
        public int ModulesTypeId { get; set; }
        public StudyType StudyType { get; set; }
        public decimal Value { get; set; }
    }
}