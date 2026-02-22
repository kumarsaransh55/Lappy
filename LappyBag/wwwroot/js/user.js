
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
                           
                        let renderHtml = `<div class="d-flex gap-2">
                            <a href = "user/details?userId=${data.id}" class="btn btn-primary" > <i class="bi bi-pencil-square"></i>Edit</a>`;
                        if (lockoutDate > todayDate) {
                            renderHtml += `<a onclick=LockUnlock('${data.id}') class="btn btn-danger" style = "cursor:pointer;" >
                                <i class="bi bi-lock-fill"></i>Unlock</a >`;
                        } else {
                            renderHtml += `<a onclick=LockUnlock('${data.id}') class="btn btn-success" style="cursor:pointer;">
                            <i class="bi bi-unlock-fill"></i>Lock</a>`;
                        }
                        renderHtml += `</div>`;
                        return renderHtml;
                    },
                    "width": "20%"
                }
            ]
    });
}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "Admin/user/LockUnlock?id=" + id,
        success: function (data) {

        }
    })
}