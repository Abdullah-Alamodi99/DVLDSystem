document.addEventListener("DOMContentLoaded", function () {
    const dropdownSubmenus = document.querySelectorAll(".dropdown-submenu");

    dropdownSubmenus.forEach(function (submenu) {
        submenu.addEventListener("mouseenter", function () {
            const dropdownMenu = submenu.querySelector(".dropdown-menu");
            if (!dropdownMenu) return;
            if (window.innerWidth < 992) {
                dropdownMenu.classList.add("drop-down-vertical");
                dropdownMenu.classList.remove("position-absolute", "start-100", "top-0", "mt-0");
            } else {
                dropdownMenu.classList.remove("drop-down-vertical");
                dropdownMenu.classList.add("position-absolute", "start-100", "top-0", "mt-0");
            }
        });
    });

    window.addEventListener("resize", function () {
        dropdownSubmenus.forEach(function (submenu) {
            const dropdownMenu = submenu.querySelector(".dropdown-menu");
            if (!dropdownMenu) return;
            if (window.innerWidth < 992) {
                dropdownMenu.classList.add("drop-down-vertical");
                dropdownMenu.classList.remove("position-absolute", "start-100", "top-0", "mt-0");
            } else {
                dropdownMenu.classList.remove("drop-down-vertical");
                dropdownMenu.classList.add("position-absolute", "start-100", "top-0", "mt-0");
            }
        });
    });
});