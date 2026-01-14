
$(document).ready(function () {
    loadDataTable(status);
});

var dataTable;
function loadDataTable(status) {
    console.log(status);
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Order/GetAll?status='+ status},
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "5%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return  ` <div class="d-flex gap-2">
                                   <a href= "order/details?orderId=${data}" class="btn btn-primary">Edit</a>
                             </div>`
                },
                "width": "10%" 
            }
        ]
    });
}
