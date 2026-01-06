document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.dropdown-submenu > a').forEach(function (element) {
        element.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            let nextEl = this.nextElementSibling;
            if (nextEl && nextEl.classList.contains('dropdown-menu')) {
                nextEl.classList.toggle('show');
            }
        });
    });

    document.querySelectorAll('.dropdown').forEach(function (dropdown) {
        dropdown.addEventListener('hidden.bs.dropdown', function () {
            this.querySelectorAll('.dropdown-menu.show').forEach(function (submenu) {
                submenu.classList.remove('show');
            });
        });
    });
});