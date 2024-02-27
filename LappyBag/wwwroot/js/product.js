
//$(document).ready(function () {
//    loadDataTable();
//});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/GetAll/' },
        "columns": [
            { data: 'brand', "width": "15%" },
            { data: 'modelName', "width": "15%" },
            { data: 'screenSize', "width": "15%" },
            { data: 'cpuModel', "width": "15%" },
            { data: 'ramMemory', "width": "15%" },
            { data: 'hardDiskSize', "width": "15%" },
            { data: 'category.name', "width": "15%" },
            { data: 'os', "width" : "15%" }
        ]
    });
}
