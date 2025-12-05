
$(document).ready(function () {
    loadDataTable();
});
var dataTable;
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/Admin/Company/GetAll/'},
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return  ` <div class="d-flex gap-2">
                                   <a href= "Company/Upsert/${data}" class="btn btn-primary">Edit</a>
                                   <a Onclick="deleteCompany(${data})" class="btn btn-danger">Delete</a>
                             </div>`
                },
                "width": "15%" 
            }
        ]
    });
}
function deleteCompany(id){
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: `/Admin/Company/Delete?id=${ id }`,
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        Swal.fire({
                            title: "Deleted!",
                            text: "Your Product has been deleted.",
                            icon: "success"
                        });
                    } else {
                        Swal.fire({
                            title: "Not Deleted!",
                            text: "Your Product has not been deleted.",
                            icon: "warning"
                        });
                    }
                    
                },
                error: function (data) {
                    Swal.fire({
                        title: "Error!",
                        text: "Error encountered while deleting the product.",
                        icon: "error"
                    });
                }
            });
        }
    });
}