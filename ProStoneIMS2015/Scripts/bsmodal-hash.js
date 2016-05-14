//Add hash for Modal dialog to avoid navigating back to history
$('div.modal').on('show.bs.modal', function () {
    var modal = this;
    var hash = modal.id;
    window.location.hash = hash;
    window.onhashchange = function () {
        if (!location.hash) {
            $(modal).modal('hide');
        }
    }
});

$('div.modal').on('hide', function () {
    var hash = this.id;
    history.pushState('', document.title, window.location.pathname);
});

