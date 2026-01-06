
var dataTable;
$(document).ready(function () {
    dataTable = $('#tblData').DataTable({
        language: { url: '/lib/datatables/i18n/ar.json' }, 
        "ajax": { url: '/driver/getall' },
        "columns": [
            { className: 'control', orderable: false, data: null, defaultContent: '', title: '' },
            { data: 'id', "width": "10%" },
            { data: 'personId', "width": "10%" },
            { data: 'nationalNo', "width": "15%" },
            { data: 'fullName', "width": "20%" },
            {
                data: 'createdDate',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
                "width": "15%"
            }, 
            { data: 'activeLicensesCount', "width": "15%" }, 
            {
                data: 'id',
                render: function (data) {
                    return `<a href="/Driver/PersonLicensesHistory?DriverId=${data}" class="btn btn-info">عرض سجل الرخص</a>`
                },
                "width": "20%",
            },

        ],
        responsive: true,
        processing: true,
        pageLength: 10
    });
});
