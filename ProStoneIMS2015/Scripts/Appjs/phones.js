    $(function () {
        $.ajaxSetup({ cache: false });
        //$("a[data-modal]").on("click", function (e) {        
        $(document).on("click","a[data-modal]", function (e) {
            $('#myModalContent').load(this.href, function () {
                $('#psModal').modal({
                    keyboard: true
                }, 'show');

                bindForm(this);
            });
            return false;
        });


    });

    function bindForm(dialog) {
        $('form', dialog).submit(function () {
            $('#progress').show();
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.success) {
                        $('#psModal').modal('hide');
                        $('#progress').hide();
                        location.reload();
                    } else {
                        $('#progress').hide();
                        if (this.method == "POST") {
                            $('#myModalContent').html(result);
                        }
                        bindForm();                        
                    }
                }
            });
            return false;
        });
    }

    $(document).ready(function () {
        $("#grid tbody tr").each(function (i, row) {
            var $actualRow = $(row);
            if ($actualRow.find('input[type=checkbox]').prop('checked') == true) {
                //$actualRow.css('background-color', '#EAF2D3');
                $actualRow.addClass('text-muted');
            }
        });
    });