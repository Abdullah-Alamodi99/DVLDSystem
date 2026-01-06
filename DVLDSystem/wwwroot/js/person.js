
var dataTable;
$(document).ready(function () {
    dataTable = $('#tblData').DataTable({
        language: { url: '/lib/datatables/i18n/ar.json' },
        "ajax": { url: '/person/getall'},
        "columns": [
            { className: 'control', orderable: false, data: null, defaultContent: '', title: '' },
            { data: 'id'},
            { data: 'nationalNo' },
            { data: 'fullName' },
            {
                data: 'dateOfBirth',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
            },
            {
                "data": "gender",
                render: function (data) {
                    return data === 0 ? "ذكر" : "أنثى"
                },
            },
            { data: 'country.name' },
            { data: 'phone'},
            { data: 'email'},
            
            {
                data: null,
                render: function (data, type, row) {
                    return `<div class="dropdown">
                                    <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-boundary="window"
                                        data-bs-display="static" type="button"
                                        id="actionMenu-${data.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                            <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <div class="dropdown-menu" aria-labelledby="dropdownMenu-${data.id}">
                                        <a href="/person/card/${row.id}" class="dropdown-item">
                                            <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> معلومات الشخص
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a href="/person/addedit" class="dropdown-item">
                                            <i class="fa-solid fa-user-plus fa-lg fa-fw me-2 icon-secondary"></i> أضافة شخص
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a href="/person/addedit/${row.id}" class="dropdown-item">
                                            <i class="fa-solid fa-user-pen fa-lg fa-fw me-2 icon-secondary"></i> تعديل بيانات الشخص
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a onClick = Delete('/person/delete/${row.id}') class="dropdown-item cursor-pointer">
                                            <i class="fa-solid fa-trash fa-lg fa-fw me-2 icon-secondary"></i> حذف الشخص
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a class="dropdown-item" href="mailto:${row.email}">
                                            <i class="fa-solid fa-envelope fa-lg fa-fw me-2 icon-secondary"></i> أرسال رسالة
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a class="dropdown-item" href="tel:${row.phone}">
                                            <i class="fa-solid fa-phone-volume fa-lg fa-fw me-2 icon-secondary"></i> الاتصال بالشخص
                                        </a>
                                    </div>
                                </div>`
                },
            },
            
        ],
        responsive: true,
        processing: true,
        pageLength: 10

    });
});


function Delete(url) {
    Swal.fire({
        title: "هل أنت متأكد",
        text: "بمجرد تنفيذ أمر حذف الشخص، لا يمكنك التراجع عن هذا",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        cancelButtonText:"الغاء",
        confirmButtonText: "نعم، حذف"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success)
                        toastr.success(data.message);
                    else
                        toastr.error(data.message);
                },
                error: function (data) {
                    toastr.error(data.message);
                }
            })
        }
    });
}