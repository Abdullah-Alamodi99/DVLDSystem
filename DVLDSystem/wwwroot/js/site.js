$(function () {

    function adjustFooter() {
        const bodyHeight = document.body.scrollHeight;
        const viewportHeight = window.innerHeight;

        if (bodyHeight <= viewportHeight) {
            $('#mainFooter').addClass('fixed-bottom');
        } else {
            $('#mainFooter').removeClass('fixed-bottom');
        }
    }

    adjustFooter();
    $(window).on('resize', adjustFooter);
});