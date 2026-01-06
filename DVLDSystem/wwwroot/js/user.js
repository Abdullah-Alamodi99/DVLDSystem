
var dataTable;
$(document).ready(function () {
    dataTable = $('#tblData').DataTable({
        language: { url: '/lib/datatables/i18n/ar.json' },
        "ajax": { url: '/user/getall' },
        "columns": [
            { className: 'control', orderable: false, data: null, defaultContent: '', title: '' },
            { data: 'id', "width": "35%" },
            { data: 'personId', "width": "15%" },
            { data: 'person.fullName', "width": "25%" },
            { data: 'userName', "width": "15%" },
            {
                data: 'isActive',
                render: function (data) {
                    if (data) {
                        return `<input type = "checkbox" checked class="input-checkbox-disabled">`
                    }
                    else {
                        return `<input type = "checkbox" class="input-checkbox-disabled">`
                    }
                }
            },

            {
                data: null,

                

                render: function (data) {
                    var dropDown = "";
                    dropDown += `<div class="dropdown position-static">
                                    <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-boundary="window"
                                        data-bs-display="static" type="button"
                                        id="actionMenu-${data.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                            <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <div class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data-id}">
                                        <a href="/user/card/${data.id}" class="dropdown-item">
                                            <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> معلومات المستخدم
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a href = "/identity/account/register" class="dropdown-item">
                                            <i class="fa-solid fa-user-plus fa-lg fa-fw me-2 icon-secondary"></i> أضافة مستخدم
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a onClick = Delete('/user/delete/${data.id}') class="dropdown-item cursor-pointer">
                                            <i class="fa-solid fa-trash fa-lg fa-fw me-2 icon-secondary"></i> حذف المستخدم
                                        </a>
                                        `;
                    if (!data.isActive) {
                        dropDown += `<div><hr class="dropdown-divider"></div>
                                        <a onClick = ActivateDeactivateUserAccount('/user/ActivateDeactivateUserAccount/${data.id}') class="dropdown-item cursor-pointer">
                                            <i class="fa-solid fa-user-check fa-lg fa-fw me-2 icon-secondary"></i> تنشيط الحساب
                                        </a>
                                        `;
                    }
                    else {
                        dropDown += `<div><hr class="dropdown-divider"></div>
                                        <a onClick = ActivateDeactivateUserAccount('/user/ActivateDeactivateUserAccount/${data.id}') class="dropdown-item cursor-pointer">
                                            <i class="fa-solid fa-user-slash fa-lg fa-fw me-2 icon-secondary"></i> تجميد الحساب
                                        </a>
                                        `;
                    }

                    dropDown += `<div><hr class="dropdown-divider"></div>
                                        <a class="dropdown-item" href="mailto:${data.person.email}">
                                            <i class="fa-solid fa-envelope fa-lg fa-fw me-2 icon-secondary"></i> أرسال رسالة
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a class="dropdown-item" href="tel:${data.person.phone}">
                                            <i class="fa-solid fa-phone-volume fa-lg fa-fw me-2 icon-secondary"></i> الاتصال بالشخص
                                        </a>
                                        <div><hr class="dropdown-divider"></div>
                                        <a class="dropdown-item" href="/user/rolemanagement/${data.id}">
                                            <i class="fa-solid fa-user-shield fa-lg fa-fw me-2 icon-secondary"></i> صلاحيات المستخدم 
                                        </a>
                                    </div>
                                </div>`;

                    return dropDown;
                },
                "width": "15%",
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
        text: "بمجرد تنفيذ أمر حذف المستخدم، لا يمكنك التراجع عن هذا",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        cancelButtonText: "الغاء",
        confirmButtonText: "نعم، حذف"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
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

function ActivateDeactivateUserAccount(url) {
    Swal.fire({
        title: "هل أنت متأكد",
        text:  "هل انت متاكد من القيام بهذه العملية؟",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        cancelButtonText: "الغاء",
        confirmButtonText: "نعم، متأكد"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "PUT",
                success: function (data) {
                    dataTable.ajax.reload();
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