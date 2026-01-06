
var dataTable;

const TestTypes = Object.freeze({
    'VisionTest': 1,
    'WrittenTest': 2,
    'StreetTest': 3
});
$(document).ready(function () {
    
    dataTable = $('#tblData').DataTable({
        language: { url: '/lib/datatables/i18n/ar.json' },
        "ajax": { url: '/localdrivinglicenseapplication/getall' },
        "columns": [
            { className: 'control', orderable: false, data: null, defaultContent: '', title: '' },
            { data: 'id', "width": "10%" },
            { data: 'licenseClassName', "width": "20%" },
            { data: 'nationalNo', "width": "12%" },
            { data: 'fullName', "width": "16%" },
            {
                data: 'applicationDate',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
                "width": "9%"
            },
            {
                data: 'applicationStatus',
                render: function (data) {
                    switch (data) {
                        case 1:
                            return 'جديد';
                        case 2:
                            return 'تم الالغاء';
                        case 3:
                            return 'مكتمل';
                        default:
                            return 'غير معروف';
                    }
                },
                "width": "9%"
            },

            { data: 'passedTestsCount', width: "12%" },

            {
                data: null,
                render: function (data) {

                    var dropDown = `<div class="dropdown">
                                    <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-boundary="window"
                                        data-bs-display="static" type="button"
                                        id="actionMenu-${data.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                            <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data.id}">
                                        <li>
                                            <a href="/localdrivinglicenseapplication/details/${data.id}" class="dropdown-item">
                                                    <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> تفاصيل الطلب 
                                            </a>
                                        </li>`;
                    if (data.applicationStatus == 2 || data.applicationStatus == 3) {
                        dropDown += `<li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "editApp" href="/localdrivinglicenseapplication/addedit/${data.id}" class="dropdown-item disabled">
                                                <i class="fa-solid fa-user-pen fa-lg fa-fw me-2 icon-secondary"></i> تعديل الطلب 
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "deleteApp" onClick = Delete('/localdrivinglicenseapplication/delete/${data.id}') class="dropdown-item cursor-pointer disabled">
                                                <i class="fa-solid fa-trash fa-lg fa-fw me-2 icon-secondary"></i> حذف الطلب
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "cancelApp" onClick = Cancel('/localdrivinglicenseapplication/cancel/${data.id}') class="dropdown-item cursor-pointer disabled">
                                                <i class="fa-solid fa-rectangle-xmark fa-lg fa-fw me-2 icon-secondary"></i> إلغاء الطلب
                                            </a>
                                        </li>
                                        `
                    }
                    else {
                        dropDown += `<li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "editApp" href="/localdrivinglicenseapplication/addedit/${data.id}" class="dropdown-item">
                                                <i class="fa-solid fa-user-pen fa-lg fa-fw me-2 icon-secondary"></i> تعديل الطلب 
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "deleteApp" onClick = Delete('/localdrivinglicenseapplication/delete/${data.id}') class="dropdown-item cursor-pointer">
                                                <i class="fa-solid fa-trash fa-lg fa-fw me-2 icon-secondary"></i> حذف الطلب
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "cancelApp" onClick = Cancel('/localdrivinglicenseapplication/cancel/${data.id}') class="dropdown-item cursor-pointer">
                                                <i class="fa-solid fa-rectangle-xmark fa-lg fa-fw me-2 icon-secondary"></i> إلغاء الطلب
                                            </a>
                                        </li>`
                    }
                    if (data.applicationStatus == 3 || data.applicationStatus == 2) {
                        dropDown += `<li><hr class="dropdown-divider"></li>
                                     <li class="dropdown-submenu">
                                        <a id = "sechuldeTests" class="dropdown-item dropdown-toggle disabled" href="#" onclick="preventParentDropdownClosing(event);">
                                                <i class="fa-solid fa-file-waveform fa-lg fa-fw me-2 icon-primary"></i> جدول الأختبارات
                                        </a>
                                        <ul class="dropdown-menu">
                                            <li>
                                                <a class="dropdown-item text-dark" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.VisionTest}">
                                                    <i class="fa-solid fa-eye-low-vision fa-lg fa-fw me-2 icon-primary"></i> فحص النظر
                                                </a>
                                            </li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li>
                                                <a class="dropdown-item text-dark" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.WrittenTest}">
                                                    <i class="fa-solid fa-pen-to-square fa-lg fa-fw me-2 icon-primary"></i> الفحص الكتابي
                                                </a>
                                            </li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li>
                                                <a class="dropdown-item text-dark" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeId=${TestTypes.StreetTest}">
                                                    <i class="fa-solid fa-street-view fa-lg fa-fw me-2 icon-primary"></i> فحص الشارع
                                                </a>
                                            </li>
                                        </ul>
                                        
                                     </li>`
                    }
                    else {
                        if (data.hasPassedVisionTest && data.hasPassedWrittenTest && data.hasPassedStreetTest) {
                            dropDown += `<li><hr class="dropdown-divider"></li>
                                    <li class="dropdown-submenu sechulde-tests">
                                    <a class="dropdown-item dropdown-toggle disabled" href="#" onclick="preventParentDropdownClosing(event);">
                                            <i class="fa-solid fa-file-waveform fa-lg fa-fw me-2 icon-primary"></i> جدول الأختبارات
                                    </a>
                                    <ul class="dropdown-menu">
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.VisionTest}">
                                                <i class="fa-solid fa-eye-low-vision fa-lg fa-fw me-2 icon-primary"></i> فحص النظر
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.WrittenTest}">
                                                <i class="fa-solid fa-pen-to-square fa-lg fa-fw me-2 icon-primary"></i> الفحص الكتابي
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.StreetTest}">
                                                <i class="fa-solid fa-street-view fa-lg fa-fw me-2 icon-primary"></i> فحص الشارع
                                            </a>
                                        </li>
                                    </ul>
                                        
                                    </li>`
                        }
                        else {
                            if (data.hasPassedVisionTest) {
                                if (data.hasPassedWrittenTest) {
                                    dropDown += `<li><hr class="dropdown-divider"></li>
                                    <li class="dropdown-submenu sechulde-tests">
                                    <a class="dropdown-item dropdown-toggle" href="#" onclick="preventParentDropdownClosing(event);">
                                            <i class="fa-solid fa-file-waveform fa-lg fa-fw me-2 icon-primary"></i> جدول الأختبارات
                                    </a>
                                    <ul class="dropdown-menu">
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.VisionTest}">
                                                <i class="fa-solid fa-eye-low-vision fa-lg fa-fw me-2 icon-primary"></i> فحص النظر
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.WrittenTest}">
                                                <i class="fa-solid fa-pen-to-square fa-lg fa-fw me-2 icon-primary"></i> الفحص الكتابي
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.StreetTest}">
                                                <i class="fa-solid fa-street-view fa-lg fa-fw me-2 icon-primary"></i> فحص الشارع
                                            </a>
                                        </li>
                                    </ul>
                                        
                                    </li>`
                                }
                                else {
                                    dropDown += `<li><hr class="dropdown-divider"></li>
                                    <li class="dropdown-submenu sechulde-tests">
                                    <a class="dropdown-item dropdown-toggle" href="#" onclick="preventParentDropdownClosing(event);">
                                            <i class="fa-solid fa-file-waveform fa-lg fa-fw me-2 icon-primary"></i> جدول الأختبارات
                                    </a>
                                    <ul class="dropdown-menu">
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.VisionTest}">
                                                <i class="fa-solid fa-eye-low-vision fa-lg fa-fw me-2 icon-primary"></i> فحص النظر
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.WrittenTest}">
                                                <i class="fa-solid fa-pen-to-square fa-lg fa-fw me-2 icon-primary"></i> الفحص الكتابي
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.StreetTest}">
                                                <i class="fa-solid fa-street-view fa-lg fa-fw me-2 icon-primary"></i> فحص الشارع
                                            </a>
                                        </li>
                                    </ul>
                                        
                                    </li>`
                                }
                            }
                            else {
                                dropDown += `<li><hr class="dropdown-divider"></li>
                                    <li class="dropdown-submenu sechulde-tests">
                                    <a class="dropdown-item dropdown-toggle" href="#" onclick="preventParentDropdownClosing(event);">
                                            <i class="fa-solid fa-file-waveform fa-lg fa-fw me-2 icon-primary"></i> جدول الأختبارات
                                    </a>
                                    <ul class="dropdown-menu">
                                        <li>
                                            <a class="dropdown-item text-dark" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.VisionTest}">
                                                <i class="fa-solid fa-eye-low-vision fa-lg fa-fw me-2 icon-primary"></i> فحص النظر
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.WrittenTest}">
                                                <i class="fa-solid fa-pen-to-square fa-lg fa-fw me-2 icon-primary"></i> الفحص الكتابي
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a class="dropdown-item text-dark disabled" href="/TestAppointment/Index?LDLAppID=${data.id}&TestTypeID=${TestTypes.StreetTest}">
                                                <i class="fa-solid fa-street-view fa-lg fa-fw me-2 icon-primary"></i> فحص الشارع
                                            </a>
                                        </li>
                                    </ul>
                                        
                                    </li>`
                            }
                        }
                    }

                    if (data.applicationStatus == 3) {
                        dropDown += `<li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "issueLicense" class="dropdown-item disabled" href="/license/issue?LDLApplicationId=${data.id}">
                                                <i class="fa-solid fa-id-card fa-fw fa-lg me-2 icon-primary"></i> إصدار رخصة قيادة للمرة الأولى
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "viewLicense" class="dropdown-item" href="/License/LicenseInfoByApplicationId?ApplicationId=${data.id}">
                                                <i class="fa-solid fa-eye fa-lg fa-fw me-2 icon-secondary"></i> عرض الرخصة
                                            </a>
                                        </li>`;
                    }
                    else {
                        if (data.passedTestsCount == 3) {
                            dropDown += `<li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "issueLicense" class="dropdown-item" href="/license/issue?LDLApplicationId=${data.id}">
                                                <i class="fa-solid fa-id-card fa-fw fa-lg me-2 icon-primary"></i> إصدار رخصة قيادة للمرة الأولى
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "viewLicense" class="dropdown-item disabled" href="/License/LicenseInfoByApplicationId?ApplicationId=${data.id}">
                                                <i class="fa-solid fa-eye fa-lg fa-fw me-2 icon-secondary"></i> عرض الرخصة
                                            </a>
                                        </li>`
                        }
                        else {
                            dropDown += `<li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "issueLicense" class="dropdown-item disabled" href="/license/issue?LDLApplicationId=${data.id}">
                                                <i class="fa-solid fa-id-card fa-fw fa-lg me-2 icon-primary"></i> إصدار رخصة قيادة للمرة الأولى
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "viewLicense" class="dropdown-item disabled" href="/License/LicenseInfoByApplicationId?ApplicationId=${data.id}">
                                                <i class="fa-solid fa-eye fa-lg fa-fw me-2 icon-secondary"></i> عرض الرخصة
                                            </a>
                                        </li>`
                        }
                    }

                    dropDown += `<li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a id = "viewPersonLicenses" class="dropdown-item" href="/Driver/PersonLicensesHistory?PersonId=${data.personId}">
                                                <i class="fa-solid fa-landmark fa-lg fa-fw me-2 icon-secondary"></i> عرض سجل رخص الشخص
                                            </a>
                                        </li>
                                    </ul>
                                </div>`

                    return dropDown;       
                    
                },
                "width": "10%",
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
        text: "بمجرد تنفيذ أمر حذف الطلب، لا يمكنك التراجع عن هذا",
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
                    toastr.success(data.message);
                },
                error: function (data) {
                    toastr.error(data.message);
                }
            })
        }
    });
}

function Cancel(url) {
    Swal.fire({
        title: "هل أنت متأكد",
        text: "بمجرد تنفيذ أمر ألغاء الطلب، لا يمكنك التراجع عن هذا",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        cancelButtonText: "الغاء",
        confirmButtonText: "نعم، إلغاء"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "PUT",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
                error: function (data) {
                    toastr.error(data.message);
                }
            })
        }
    });
}

function preventParentDropdownClosing(e) {
    $(e.target).next('ul').toggle();
    e.stopPropagation();
    e.preventDefault();
}