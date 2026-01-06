
var dataTable;
$(document).ready(function () {

    dataTable = $('#tblData').DataTable({
        language: { url: '/lib/datatables/i18n/ar.json' },
        "ajax": { url: '/detainedlicense/getall' },
        "columns": [
            { className: 'control', orderable: false, data: null, defaultContent: '', title: '' },
            { data: 'id', "width": "8%" },
            { data: 'licenseId', "width": "11%" },
            {
                data: 'detainDate',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
                "width": "9%"
            },
            {
                data: 'isReleased',
                render: function (data) {
                    if (data) {
                        return `<input type="checkbox" class="input-checkbox-disabled" checked />`
                    }
                    else {
                        return `<input type="checkbox" class="input-checkbox-disabled" />`
                    }
                },

                "width": "8%"

            },
            { data: 'fineFees',"width": "9%"},
            {
                data: 'releaseDate',
                render: function (data) {
                    if (data == null)
                        return "-";

                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
                "width": "10%"
            },
            {
                data: "releaseApplicationId",
                render: function (data) {
                    if (data == null)
                        return "-";
                    return data;
                },
                "width": "12%"
            },
            { data: 'nationalNo',"width": "10%"},
            { data: 'fullName',"width": "13%"},
            {
                data: null,
                render: function (data) {
                    var dropDown = "";
                    dropDown+= `<div class="dropdown">
                                    <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-boundary="window"
                                        data-bs-display="static" type="button"
                                        id="actionMenu-${data.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                            <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data.id}">
                                        <li>
                                            <a href="/Person/Card/${data.personId}" class="dropdown-item">
                                                    <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> معلومات الشخص 
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a href="/License/LicenseInfoById/${data.licenseId}" class="dropdown-item">
                                                    <i class="fa-solid fa-eye fa-lg fa-fw me-2 icon-secondary"></i> معلومات الرخصة 
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li>
                                            <a href="/Driver/PersonLicensesHistory?PersonId=${data.personId}" class="dropdown-item">
                                                    <i class="fa-solid fa-landmark fa-lg fa-fw me-2 icon-secondary"></i> عرض سجل رخص الشخص
                                            </a>
                                        </li>
                                        <li><hr class="dropdown-divider"></li>`
                    if (data.isReleased) {
                        dropDown += `<li>
                                            <a href="/DetainedLicense/Release/${data.licenseId}" class="dropdown-item disabled">
                                                    <i class="fa-solid fa-hand-holding fa-lg fa-fw me-2 icon-primary"></i> فك حجز رخصة
                                            </a>
                                     </li>`;
                    }
                    else {
                        dropDown += `<li>
                                            <a href="/DetainedLicense/Release?DetainedLicenseId=${data.id}" class="dropdown-item">
                                                    <i class="fa-solid fa-hand-holding fa-lg fa-fw me-2 icon-primary"></i> فك حجز رخصة
                                            </a>
                                     </li>`

                    }

                    dropDown += `</ul>
                                 </div>`;
                    return dropDown;
                },
                "width": "25%"
            }
        ],
        responsive: true,
        processing: true,
        pageLength: 10

    });
});
function preventParentDropdownClosing(e) {
    $(e.target).next('ul').toggle();
    e.stopPropagation();
    e.preventDefault();
}