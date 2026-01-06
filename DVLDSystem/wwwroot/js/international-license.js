
var dataTable;
$(document).ready(function () {

    dataTable = $('#tblData').DataTable({
        language: { url: '/lib/datatables/i18n/ar.json' },
        "ajax": { url: '/internationallicense/getall' },
        "columns": [
            { className: 'control', orderable: false, data: null, defaultContent: '', title: '' },
            { data: 'id', "width": "20%" },
            { data: 'id', "width": "10%" },
            { data: 'driverId', "width": "12%" },
            { data: 'issuedUsingLocalLicenseId', "width": "15%" },
            {
                data: 'issueDate',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
                "width": "10%"
            },
            {
                data: 'expirationDate',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString();
                },
                "width": "12%"
            },
            {
                data: 'isActive',
                render: function (data) {
                    if (data) {
                        return `<input type="checkbox" class="input-checkbox-disabled" checked>`
                    }
                    else {
                        return `<input type="checkbox" class="input-checkbox-disabled">`
                    }
                },
                "width": "9%"
            },
            {
                data: null,
                render: function (data) {

                    return `<div class="dropdown">
                                    <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-boundary="window"
                                        data-bs-display="static" type="button"
                                        id="actionMenu-${data.id}" data-bs-toggle="dropdown" aria-expanded="false">
                                            <i class="fas fa-ellipsis-v"></i>
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton-${data.id}">
                                        <li>
                                            <a href="/Person/Card/${data.driver.person.id}" class="dropdown-item">
                                                    <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> معلومات الشخص 
                                            </a>
                                        </li>
                                        <div><hr class="dropdown-divider"></div>
                                        <li>
                                            <a href="/InternationalLicense/LicenseInfo/${data.id}" class="dropdown-item">
                                                    <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> عرض الرخصة 
                                            </a>
                                        </li>
                                        <div><hr class="dropdown-divider"></div>
                                        <li>
                                            <a href="/Driver/PersonLicensesHistory?DriverId=${data.driverId}" class="dropdown-item">
                                                    <i class="fa-solid fa-circle-info fa-lg fa-fw me-2 icon-secondary"></i> عرض سجل الرخص 
                                            </a>
                                        </li>
                                    </ul>
                            </div>`
                },
                "width":"20%"
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