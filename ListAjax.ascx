<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListAjax.ascx.cs" Inherits="PRWeb.Modules.ListAjax" %>
<script type="text/javascript">
    $(function () {
        kPortal.SetMenu('qldt', 'modules_list');
    });
    function ChooseAll1(aspnetForm) {

        if (aspnetForm.kChooseAll1.checked == true) {
            for (var i = 0; i < aspnetForm.elements.length; i++) {
                if ((aspnetForm.elements[i].type == "checkbox") && (aspnetForm.elements[i].value == "vnk1")) {
                    aspnetForm.elements[i].checked = true;
                }
            }
        } else {
            for (i = 0; i < aspnetForm.elements.length; i++) {
                if ((aspnetForm.elements[i].type == "checkbox") && (aspnetForm.elements[i].value == "vnk1")) {
                    aspnetForm.elements[i].checked = false;
                }
            }
        }
    }
    function checkMsg1(aspnetForm, setvalue, Msg) {
        var removeList = '';
        var check = false;
        for (var i = 0; i < aspnetForm.elements.length; i++) {
            if (aspnetForm.elements[i].type == "checkbox") {
                if (aspnetForm.elements[i].checked) {
                    if (aspnetForm.elements[i].value == "vnk1") {
                        check = true;
                    }
                    if (aspnetForm.elements[i].name.substr(0, 8) == 'TChoose_') {
                        removeList += ',' + aspnetForm.elements[i].name.substring(8);
                    }
                }
            }
        }
        if (!check) {
            alert('Bạn phải chọn ít nhất một bản ghi.');
            return false;
        }
        if (removeList == '') return false;
        removeList = removeList.substring(1);
        if (confirm(Msg)) {
            aspnetForm.kevent.value = setvalue; aspnetForm.klist.value = removeList;
            return aspnetForm.submit();
        } else return false;
    }
</script>
<input type="hidden" value="0" name="kevent" />
<input type="hidden" value="0" name="klist" />

<div class="app-content flex-column-fluid mb-5">
    <div class="app-container container-fluid">
        <div class="card shadow-sm">
            <div class="card-header">
                <h4 class="card-title">Danh sách học phần</h4>
            </div>
            <div class="card-body">
                <div class="panel-body form">
                    <div class="form-group">
                        <label class="form-label">
                            Đơn vị đào tạo
                        </label>
                        <div>
                            <asp:DropDownList ID="inpDepartmentID" runat="server" AutoPostBack="true" CssClass="form-control input-xs form-select" data-control="select2" OnSelectedIndexChanged="inpDepartmentID_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Nhập từ khóa tìm kiếm</label>
                        <div>
                            <asp:TextBox ID="txtSearch" runat="server" MaxLength="400" CssClass="form-control input-xs" placeholder="Nhập tên môn, mã môn để tìm kiếm..."></asp:TextBox>
                        </div>
                    </div>
                    <asp:ObjectDataSource ID="objList" runat="server" TypeName="PRWeb.Modules.DB" MaximumRowsParameterName="100" SelectMethod="Modules_List"></asp:ObjectDataSource>
                    <div class="table-responsive">
                        <table id="kt_list_table" class="table table-bordered">
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>#</th>
                                    <th>Mã môn</th>
                                    <th>Mã in</th>
                                    <th>Tên môn</th>
                                    <th>Viết tắt</th>
                                    <th>TC quy đổi</th>
                                    <th>TC</th>
                                    <th>LT</th>
                                    <th>TH</th>
                                    <th>BTL</th>
                                    <th>ĐA</th>
                                    <th>Online</th>
                                    <th>Khung</th>
                                    <th>ĐV</th>
                                    <th>Số lớp</th>
                                    <th>Đề cương</th>
                                    <th>1</th>
                                    <th>2</th>
                                    <th>Khóa</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="k-buttons">
                    <button type="button" class="btn btn-primary" onclick="return AddNew('<%=modul %>')">
                        <i class="ki-outline ki-plus-square fs-3 me-2"></i>
                        Thêm mới
                    </button>
                    <button type="button" class="btn btn-danger" onclick="return checkDelete(this.form);">
                        <i class="ki-outline ki-trash fs-3 "></i>
                        Xoá
                    </button>
                    <button type="button" class="btn btn-primary" onclick="window.open('/qldt/print/printlistmodules.htm','print');">
                        <i class="fa fa-print"></i>
                        In danh sách học phần
                    </button>
                    <button type="button" class="btn btn-danger" onclick="changes = false; return checkMsg1(this.form,'5','Bạn xác nhận mở khóa sửa đề cương?');">
                        <i class="fa fa-unlock"></i>
                        Mở khóa sửa đề cương
                    </button>
                    <button type="button" class="btn btn-primary" onclick="changes = false; return checkMsg1(this.form,'6','Bạn xác nhận khóa sửa đề cương?');">
                        <i class="fa fa-lock"></i>
                        Khóa sửa đề cương
                    </button>
                    <button id="butExportExcelDSND" type="button" runat="server" class="btn btn-success" onserverclick="butExportExcel_Click">
                        <i class="ki-outline ki-file-down fs-3 me-2 "></i>Xuất danh sách
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>
<script>
    var modulesTable; // Biến global để lưu reference của DataTable
    var searchTimeout; // Biến để lưu timeout cho debouncing

    function initModulesTable() {
        const tableId = '#kt_list_table';

        console.log("🔄 Bắt đầu khởi tạo DataTable...");

        // Kiểm tra nếu DataTable đã tồn tại
        if ($.fn.DataTable.isDataTable(tableId)) {
            console.log("⚠️ DataTable đã tồn tại, destroy và tạo lại...");
            $(tableId).DataTable().destroy();
        }

        // Khởi tạo DataTable mới
        try {
            modulesTable = $(tableId).DataTable({
                processing: true,
                serverSide: true,
                ajax: {
                    url: '/vnkWWW/Api.ashx?fn=Modules',
                    type: 'POST',
                    data: function(d) {
                        // Thêm tham số tìm kiếm vào request
                        var searchKeyword = $('#<%=txtSearch.ClientID%>').val() || '';
                        var departmentID = $('#<%=inpDepartmentID.ClientID%>').val() || '';
                        
                        d.searchKeyword = searchKeyword;
                        d.departmentID = departmentID;
                        
                        console.log("📤 Gửi dữ liệu tìm kiếm:", {
                            searchKeyword: searchKeyword,
                            departmentID: departmentID,
                            allData: d
                        });
                        return d;
                    },
                    dataSrc: function (json) {
                        console.log("📥 Dữ liệu từ server:", json);
                        return json.data;
                    },
                    error: function (xhr, error, thrown) {
                        console.error("❌ AJAX Lỗi:", {
                            xhr: xhr,
                            error: error,
                            thrown: thrown,
                            responseText: xhr.responseText
                        });
                    },
                    beforeSend: function(xhr, settings) {
                        console.log("📤 Trước khi gửi AJAX:", settings.url, settings.data);
                    },
                    complete: function(xhr, status) {
                        console.log("✅ AJAX hoàn thành:", status);
                        $('#<%=txtSearch.ClientID%>').removeClass('loading');
                    }
                },

                pageLength: 10,
                pagingType: "full_numbers",
                lengthMenu: [[10, 30, 50, 100, -1], [10, 30, 50, 100, "Tất cả"]],
                language: {
                    url: 'https://nsv-cdn.prweb.com.vn/vnkresource/json/vietnamese.json'
                },
                dom: `
                rt
                <"row mt-3" <"col-sm-12 col-md-6 d-flex align-items-center justify-content-md-start"
                        l
                        i
                    >
                    <"col-sm-12 col-md-6 d-flex align-items-center justify-content-md-end"
                        p
                    > >`,

                columns: [
                    { data: null, render: r => `<input type="checkbox" id="kCh_${r.ModulesID}" name="TChoose_${r.ModulesID}" value="vnk1">` },
                    { data: null, render: (data, type, row, meta) => meta.row + 1 },
                    { data: 'ModulesID' },
                    { data: 'ModulesCode' },
                    { data: 'ModulesName' },
                    { data: 'ModulesNameSort' },
                    { data: 'CreditsFee' },
                    { data: 'Credits' },
                    { data: 'CreditsLT' },
                    { data: 'CreditsTH' },
                    { data: 'CreditsK' },
                    { data: 'CreditsK1' },
                    { data: 'CreditsOnline' },
                    { data: null, render: r => `<a href="/qldt/print/decuongchitiethocphan.htm?modulesid=${r.ModulesID}" target="_blank"><i class="fa fa-print"></i></a>` },
                    { data: 'DepartmentName' },
                    { data: 'SoLopHP' },
                    { data: null, render: r => `<a href="/qldt/print/decuongchitiethocphan.htm?modulesid=${r.ModulesID}" target="_blank"><i class="fa fa-print"></i></a>` },
                    { data: null, render: r => `<a href="?modul=modules&ctr=edit&id=${r.ModulesID}&lg=vn"><i class="ki-outline ki-pencil fs-3 "></i></a>` },
                    { data: null, render: r => `<input type="checkbox" id="kCh_${r.ModulesID}" name="TChoose_${r.ModulesID}" value="vnk1">` },
                    { data: null, render: r => `<input type="checkbox" id="kCh_${r.ModulesID}" name="TChoose_${r.ModulesID}" value="vnk1">` }
                ]
            });

            console.log("✅ DataTable đã được khởi tạo thành công:", modulesTable);

            // Gắn event cho DataTable
            modulesTable.on('xhr.dt', function() {
                console.log("🔄 AJAX request đã hoàn thành");
                $('#<%=txtSearch.ClientID%>').removeClass('loading');
            });

        } catch (error) {
            console.error("❌ Lỗi khi khởi tạo DataTable:", error);
        }
    }

    function setupSearchEvents() {
        console.log("🔗 Thiết lập sự kiện tìm kiếm...");
        
        // Kiểm tra element tồn tại
        var txtSearchElement = $('#<%=txtSearch.ClientID%>');
        var departmentElement = $('#<%=inpDepartmentID.ClientID%>');
        
        console.log("🔍 TextSearch element:", txtSearchElement.length > 0 ? "Tìm thấy" : "Không tìm thấy");
        console.log("🏢 Department element:", departmentElement.length > 0 ? "Tìm thấy" : "Không tìm thấy");

        // Sự kiện tìm kiếm cho textbox txtSearch
        txtSearchElement.on('input keyup', function() {
            var searchValue = $(this).val();
            console.log("⌨️ Người dùng gõ:", searchValue);
            
            // Thêm loading class
            $(this).addClass('loading');
            
            // Clear timeout cũ
            clearTimeout(searchTimeout);
            
            // Tạo timeout mới để debounce
            searchTimeout = setTimeout(function() {
                console.log("🔍 Bắt đầu tìm kiếm với từ khóa:", searchValue);
                
                // Kiểm tra DataTable có tồn tại không
                if (modulesTable && $.fn.DataTable.isDataTable('#kt_list_table')) {
                    console.log("✅ DataTable tồn tại, đang reload...");
                    try {
                        modulesTable.ajax.reload(function(json) {
                            console.log("🔄 Reload hoàn thành:", json);
                        }, false);
                    } catch (error) {
                        console.error("❌ Lỗi khi reload DataTable:", error);
                    }
                } else {
                    console.error("❌ DataTable không tồn tại hoặc chưa được khởi tạo");
                    console.log("DataTable object:", modulesTable);
                    console.log("Is DataTable:", $.fn.DataTable.isDataTable('#kt_list_table'));
                }
            }, 500);
        });

        // Sự kiện thay đổi dropdown department
        departmentElement.on('change', function() {
            var departmentValue = $(this).val();
            console.log("🏢 Đổi đơn vị đào tạo:", departmentValue);
            
            if (modulesTable && $.fn.DataTable.isDataTable('#kt_list_table')) {
                console.log("✅ Reload DataTable cho department...");
                modulesTable.ajax.reload(null, false);
            } else {
                console.error("❌ DataTable không tồn tại khi thay đổi department");
            }
        });
        
        console.log("✅ Sự kiện tìm kiếm đã được thiết lập");
    }

    $(document).ready(function () {
        console.log("🚀 Document ready - Bắt đầu khởi tạo...");
        
        // Khởi tạo DataTable trước
        initModulesTable();
        
        // Đợi một chút rồi mới setup events
        setTimeout(function() {
            setupSearchEvents();
        }, 1000);
        
        console.log("✅ Khởi tạo hoàn thành");
    });

</script>

<style>
    /* CSS cho loading indicator */
    .loading {
        background-image: url('data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wjRLjbnKwZQMMnwJFHCKHK8cqhXElSaYxkKSAhY2AkDjkgAEEeqJQSEkJADs=');
        background-repeat: no-repeat;
        background-position: right 10px center;
        padding-right: 30px;
    }
    
    .form-control.input-xs {
        transition: all 0.3s ease;
    }
    
    .form-control.input-xs:focus {
        border-color: #007bff;
        box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25);
    }
</style>