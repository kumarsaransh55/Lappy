
$(document).ready(function () {
    loadDataTable();
});

var dataTable;
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/User/GetAll',
        },
            "columns": [
                { data: 'name', "width": "10%" },
                { data: 'email', "width": "15%" },
                { data: 'phoneNumber', "width": "15%" },
                { data: 'companyName', "width": "15%" },
                { data: 'role', "width": "15%" },
                {
                    data: { id : "id", lockoutEnd : "lockoutEnd"},
                    "render": function (data) {
                        const todayDate = new Date().getTime();
                        const lockoutDate = new Date(data.lockoutEnd).getTime();
                           
                        let renderHtml = `<div class="d-flex justify-content-center gap-2">
                            <a href = "user/rolemanagement/${data.id}" class="btn btn-primary text-nowrap" > <i class="bi bi-pencil-square"></i>Edit</a>`;
                        if (lockoutDate > todayDate) {
                            renderHtml += `<a onclick=LockUnlock('${data.id}') class="btn btn-danger text-nowrap" >
                                <i class="bi bi-lock-fill"></i>Unlock</a >`;
                        } else {
                            renderHtml += `<a onclick=LockUnlock('${data.id}') class="btn btn-success text-nowrap" >
                            <i class="bi bi-unlock-fill"></i>Lock</a>`;
                        }
                        renderHtml += `</div>`;
                        return renderHtml;
                    },
                    "width": "23%"
                }
            ]
    });
}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/user/LockUnlock/" + id,
        success: function (data) {
            if (data.success) {
                toastr.success(data.message)
                dataTable.ajax.reload();
            } else {
                toastr.error(data.message)
            }
        },
        error: function (status) {
            toastr.error(status);
        }
    })
}