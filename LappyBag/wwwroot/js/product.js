
$(document).ready(function () {
    loadDataTable();
});
var dataTable;
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Product/GetAll/' },
        "columns": [
            { data: 'brand', "width": "5%" },
            { data: 'modelName', "width": "15%" },
            { data: 'screenSize', "width": "10%" },
            { data: 'cpuModel', "width": "20%" },
            { data: 'ramMemory', "width": "5%" },
            { data: 'hardDiskSize', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            { data: 'os', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return  ` <div class="d-flex gap-2">
                                   <a href= "Product/Upsert/${data}" class="btn btn-primary">Edit</a>
                                   <a Onclick="deleteProduct(${data})" class="btn btn-danger">Delete</a>
                             </div>`
                },
                "width": "15%" 
            }
        ]
    });
}
function deleteProduct(id){
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
                url: `/Admin/Product/Delete?id=${ id }`,
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