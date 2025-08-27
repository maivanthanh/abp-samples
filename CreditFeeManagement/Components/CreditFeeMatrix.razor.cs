using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using CreditFeeManagement.Models;
using CreditFeeManagement.ViewModels;

namespace CreditFeeManagement.Components
{
    public partial class CreditFeeMatrix : ComponentBase
    {
        [Parameter] public CreditFeeMatrixViewModel ViewModel { get; set; } = new();
        [Parameter] public EventCallback<List<CreditFee>> OnSave { get; set; }
        [Parameter] public EventCallback<CreditFeeMatrixViewModel> OnExport { get; set; }
        [Parameter] public EventCallback<CreditFeeMatrixViewModel> OnImport { get; set; }

        private bool IsLoading { get; set; } = false;
        private List<CellSelection> SelectedCells { get; set; } = new();
        private decimal? CopiedValue { get; set; }
        private bool IsCtrlPressed { get; set; } = false;
        private bool IsShiftPressed { get; set; } = false;
        private CellSelection? LastSelectedCell { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (ViewModel.StudentTypes.Any() && ViewModel.SelectedStudentTypeId == 0)
            {
                ViewModel.SelectedStudentTypeId = ViewModel.StudentTypes.First().Id;
            }
            
            ViewModel.InitializeMatrix();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("addKeyboardListeners", DotNetObjectReference.Create(this));
            }
        }

        private void SelectStudentType(int studentTypeId)
        {
            ViewModel.SelectedStudentTypeId = studentTypeId;
            ClearSelection();
            StateHasChanged();
        }

        private void UpdateFee(int studentTypeId, int moduleTypeId, StudyType studyType, ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out decimal value))
            {
                ViewModel.SetFee(studentTypeId, moduleTypeId, studyType, value);
            }
            else
            {
                ViewModel.SetFee(studentTypeId, moduleTypeId, studyType, 0);
            }
        }

        private void SelectCell(int studentTypeId, int moduleTypeId, StudyType studyType, MouseEventArgs e)
        {
            var cellSelection = new CellSelection
            {
                StudentTypeId = studentTypeId,
                ModulesTypeId = moduleTypeId,
                StudyType = studyType,
                Value = ViewModel.GetFee(studentTypeId, moduleTypeId, studyType)
            };

            if (e.CtrlKey)
            {
                // Ctrl + Click: Toggle selection
                var existing = SelectedCells.FirstOrDefault(c => 
                    c.StudentTypeId == studentTypeId && 
                    c.ModulesTypeId == moduleTypeId && 
                    c.StudyType == studyType);
                
                if (existing != null)
                {
                    SelectedCells.Remove(existing);
                }
                else
                {
                    SelectedCells.Add(cellSelection);
                }
            }
            else if (e.ShiftKey && LastSelectedCell != null)
            {
                // Shift + Click: Select range
                SelectRange(LastSelectedCell, cellSelection);
            }
            else
            {
                // Normal click: Single selection
                SelectedCells.Clear();
                SelectedCells.Add(cellSelection);
            }

            LastSelectedCell = cellSelection;
            StateHasChanged();
        }

        private void SelectRange(CellSelection start, CellSelection end)
        {
            SelectedCells.Clear();
            
            var modules = ViewModel.ModulesTypes.ToList();
            var studyTypes = ViewModel.StudyTypes.ToArray();
            
            var startModuleIndex = modules.FindIndex(m => m.Id == start.ModulesTypeId);
            var endModuleIndex = modules.FindIndex(m => m.Id == end.ModulesTypeId);
            var startStudyIndex = Array.IndexOf(studyTypes, start.StudyType);
            var endStudyIndex = Array.IndexOf(studyTypes, end.StudyType);
            
            var minModuleIndex = Math.Min(startModuleIndex, endModuleIndex);
            var maxModuleIndex = Math.Max(startModuleIndex, endModuleIndex);
            var minStudyIndex = Math.Min(startStudyIndex, endStudyIndex);
            var maxStudyIndex = Math.Max(startStudyIndex, endStudyIndex);
            
            for (int i = minModuleIndex; i <= maxModuleIndex; i++)
            {
                for (int j = minStudyIndex; j <= maxStudyIndex; j++)
                {
                    SelectedCells.Add(new CellSelection
                    {
                        StudentTypeId = ViewModel.SelectedStudentTypeId,
                        ModulesTypeId = modules[i].Id,
                        StudyType = studyTypes[j],
                        Value = ViewModel.GetFee(ViewModel.SelectedStudentTypeId, modules[i].Id, studyTypes[j])
                    });
                }
            }
        }

        private bool IsCellSelected(int studentTypeId, int moduleTypeId, StudyType studyType)
        {
            return SelectedCells.Any(c => 
                c.StudentTypeId == studentTypeId && 
                c.ModulesTypeId == moduleTypeId && 
                c.StudyType == studyType);
        }

        private void ClearSelection()
        {
            SelectedCells.Clear();
            LastSelectedCell = null;
            StateHasChanged();
        }

        private async Task HandleKeyDown(KeyboardEventArgs e, int studentTypeId, int moduleTypeId, StudyType studyType)
        {
            if (e.Key == "Tab" || e.Key == "Enter")
            {
                await MoveToNextCell(studentTypeId, moduleTypeId, studyType, e.Key == "Tab" ? !e.ShiftKey : true);
            }
            else if (e.Key == "Delete")
            {
                ViewModel.SetFee(studentTypeId, moduleTypeId, studyType, 0);
                StateHasChanged();
            }
            else if (e.CtrlKey && e.Key == "c")
            {
                CopySelected();
            }
            else if (e.CtrlKey && e.Key == "v")
            {
                PasteToSelected();
            }
        }

        private async Task MoveToNextCell(int currentStudentTypeId, int currentModuleTypeId, StudyType currentStudyType, bool forward)
        {
            var modules = ViewModel.ModulesTypes.ToList();
            var studyTypes = ViewModel.StudyTypes.ToArray();
            
            var moduleIndex = modules.FindIndex(m => m.Id == currentModuleTypeId);
            var studyIndex = Array.IndexOf(studyTypes, currentStudyType);
            
            if (forward)
            {
                studyIndex++;
                if (studyIndex >= studyTypes.Length)
                {
                    studyIndex = 0;
                    moduleIndex++;
                    if (moduleIndex >= modules.Count)
                    {
                        moduleIndex = 0;
                    }
                }
            }
            else
            {
                studyIndex--;
                if (studyIndex < 0)
                {
                    studyIndex = studyTypes.Length - 1;
                    moduleIndex--;
                    if (moduleIndex < 0)
                    {
                        moduleIndex = modules.Count - 1;
                    }
                }
            }

            var nextCellId = $"cell_{currentStudentTypeId}_{modules[moduleIndex].Id}_{(int)studyTypes[studyIndex]}";
            await JSRuntime.InvokeVoidAsync("focusElement", nextCellId);
        }

        // Batch Operations
        private void FillSelectedCells()
        {
            if (ViewModel.FillValue.HasValue)
            {
                foreach (var cell in SelectedCells)
                {
                    ViewModel.SetFee(cell.StudentTypeId, cell.ModulesTypeId, cell.StudyType, ViewModel.FillValue.Value);
                }
                ViewModel.StatusMessage = $"Đã điền {SelectedCells.Count} ô với giá trị {ViewModel.FillValue:N0}";
                StateHasChanged();
            }
        }

        private void FillAllCurrentTab()
        {
            if (ViewModel.FillValue.HasValue)
            {
                int count = 0;
                foreach (var module in ViewModel.ModulesTypes)
                {
                    foreach (StudyType studyType in ViewModel.StudyTypes)
                    {
                        ViewModel.SetFee(ViewModel.SelectedStudentTypeId, module.Id, studyType, ViewModel.FillValue.Value);
                        count++;
                    }
                }
                ViewModel.StatusMessage = $"Đã điền tất cả {count} ô với giá trị {ViewModel.FillValue:N0}";
                StateHasChanged();
            }
        }

        private void FillRow(int moduleTypeId)
        {
            if (ViewModel.FillValue.HasValue)
            {
                foreach (StudyType studyType in ViewModel.StudyTypes)
                {
                    ViewModel.SetFee(ViewModel.SelectedStudentTypeId, moduleTypeId, studyType, ViewModel.FillValue.Value);
                }
                var moduleName = ViewModel.ModulesTypes.First(m => m.Id == moduleTypeId).Name;
                ViewModel.StatusMessage = $"Đã điền hàng '{moduleName}' với giá trị {ViewModel.FillValue:N0}";
                StateHasChanged();
            }
        }

        private void FillColumn(StudyType studyType)
        {
            if (ViewModel.FillValue.HasValue)
            {
                foreach (var module in ViewModel.ModulesTypes)
                {
                    ViewModel.SetFee(ViewModel.SelectedStudentTypeId, module.Id, studyType, ViewModel.FillValue.Value);
                }
                ViewModel.StatusMessage = $"Đã điền cột '{studyType.GetDisplayName()}' với giá trị {ViewModel.FillValue:N0}";
                StateHasChanged();
            }
        }

        private void ClearSelected()
        {
            foreach (var cell in SelectedCells)
            {
                ViewModel.SetFee(cell.StudentTypeId, cell.ModulesTypeId, cell.StudyType, 0);
            }
            ViewModel.StatusMessage = $"Đã xóa {SelectedCells.Count} ô được chọn";
            ClearSelection();
        }

        private void ClearRow(int moduleTypeId)
        {
            foreach (StudyType studyType in ViewModel.StudyTypes)
            {
                ViewModel.SetFee(ViewModel.SelectedStudentTypeId, moduleTypeId, studyType, 0);
            }
            var moduleName = ViewModel.ModulesTypes.First(m => m.Id == moduleTypeId).Name;
            ViewModel.StatusMessage = $"Đã xóa hàng '{moduleName}'";
            StateHasChanged();
        }

        private void ClearColumn(StudyType studyType)
        {
            foreach (var module in ViewModel.ModulesTypes)
            {
                ViewModel.SetFee(ViewModel.SelectedStudentTypeId, module.Id, studyType, 0);
            }
            ViewModel.StatusMessage = $"Đã xóa cột '{studyType.GetDisplayName()}'";
            StateHasChanged();
        }

        private void CopySelected()
        {
            if (SelectedCells.Count == 1)
            {
                CopiedValue = SelectedCells.First().Value;
                ViewModel.StatusMessage = $"Đã copy giá trị {CopiedValue:N0}";
            }
            else if (SelectedCells.Count > 1)
            {
                // Could implement copying multiple values as JSON or CSV
                ViewModel.StatusMessage = "Chỉ có thể copy 1 ô tại một thời điểm";
            }
        }

        private void PasteToSelected()
        {
            if (CopiedValue.HasValue)
            {
                foreach (var cell in SelectedCells)
                {
                    ViewModel.SetFee(cell.StudentTypeId, cell.ModulesTypeId, cell.StudyType, CopiedValue.Value);
                }
                ViewModel.StatusMessage = $"Đã paste giá trị {CopiedValue:N0} vào {SelectedCells.Count} ô";
                StateHasChanged();
            }
        }

        private async Task SaveAll()
        {
            IsLoading = true;
            try
            {
                var entities = ViewModel.ToEntityList();
                await OnSave.InvokeAsync(entities);
                ViewModel.StatusMessage = $"Đã lưu thành công {entities.Count} bản ghi";
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = $"Lỗi khi lưu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task ExportToExcel()
        {
            await OnExport.InvokeAsync(ViewModel);
        }

        private async Task ImportFromExcel()
        {
            await OnImport.InvokeAsync(ViewModel);
        }

        [JSInvokable]
        public void OnKeyDown(string key, bool ctrlKey, bool shiftKey)
        {
            IsCtrlPressed = ctrlKey;
            IsShiftPressed = shiftKey;
        }

        [JSInvokable]
        public void OnKeyUp(string key)
        {
            if (key == "Control") IsCtrlPressed = false;
            if (key == "Shift") IsShiftPressed = false;
        }
    }
}