    $(function () {
        $.ajaxSetup({ cache: false });
        //$("a[data-modal]").on("click", function (e) {        
        $(document).on("click", "a[data-modal]", function (e) {
            //$("div.modal").unbind("shown.bs.modal");
            $(this).closest('tr').addClass("selectedRow");
            $('div.modal').on('hidden.bs.modal', function () {
                $('#grid tbody tr.selectedRow').removeClass("selectedRow");
            });
            $('#myModalContent').load(this.href, function () {
                $('#psmdl').modal({
                    keyboard: true,
                    backdrop: 'static'
                }, 'show');
                bindForm(this);
            });
            $('div.modal').on('shown.bs.modal', function () {                
                SetUpPickers();
                $('div.modal input.tt-hint').remove();
                setTypeAhead('.tt-lookup');
            });
            return false;
        });


    });

    function bindForm(dialog) {
        $('form', dialog).submit(function () {
            //SetUpPickers();
            //alert('hello..');
            $('#progress').show();
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.success) {
                        $('#psmdl').modal('hide');
                        $('#progress').hide();
                        //location.reload();
                        if (result.action == "edit") {
                            var _selectedRow = $("#grid tbody tr.selectedRow td");
                            _selectedRow.each(function (index) {
                                if (index < _selectedRow.length - 1) {
                                    var tdchild = index + 1;
                                    var tdata = result.col[index];
                                    if (tdata == "True") {
                                        $("tr.selectedRow td:nth-child(" + tdchild + ")").html('<i class="fa fa-check"></i>');
                                    }
                                    else if (tdata == "False") {
                                        $("tr.selectedRow td:nth-child(" + tdchild + ")").html('');
                                    }
                                    else {
                                        $("tr.selectedRow td:nth-child(" + tdchild + ")").html(tdata);
                                    }
                                }
                            });
                            _selectedRow = $("#grid tbody tr.selectedRow");
                            if (result.inactive == "True") {
                                _selectedRow.addClass('text-muted');
                                $("#grid tbody tr.selectedRow input:checkbox[name='Inactive']").prop('checked', true);
                            }
                            else {
                                _selectedRow.removeClass('text-muted');
                                $("#grid tbody tr.selectedRow input:checkbox[name='Inactive']").prop('checked', false);
                            }
                        }
                        else if(result.action == "delete") {
                            $("#grid tbody tr.selectedRow").remove();
                        }
                        else if (result.action == "insert") {
                            if (result.ctrl == "Inventory") {
                                grid.reload();
                            }
                            else {
                                if ($('#grid tbody tr').length == 0) {
                                    $.ajax({
                                        url: result.ctrl,
                                        type: "GET",
                                    })
                                    .done(function (partialViewResult) {
                                        $("#gridContent").html(partialViewResult);
                                    });
                                }
                                else {
                                    $("#grid tbody tr.newRow").removeClass("newRow");
                                    $("#grid > tbody tr:first").clone().addClass("newRow").prependTo("#grid > tbody");
                                    var _newRow = $("#grid tbody tr.newRow td");
                                    _newRow.each(function (index) {
                                        if (index < _newRow.length - 1) {
                                            var tdchild = index + 1;
                                            var tdata = result.col[index];
                                            if (tdata == "True") {
                                                $("tr.newRow td:nth-child(" + tdchild + ")").html('<i class="fa fa-check"></i>');
                                            }
                                            else if (tdata == "False") {
                                                $("tr.newRow td:nth-child(" + tdchild + ")").html('');
                                            }
                                            else {
                                                $("tr.newRow td:nth-child(" + tdchild + ")").html(tdata);
                                            }
                                        }
                                        $('tr.newRow td:last-child a[title="Detail"]').attr("href", result.ctrl + "/details/" + result.id);
                                        $('tr.newRow td:last-child a[title="Edit"]').attr("href", result.ctrl + "/edit/" + result.id);
                                        $('tr.newRow td:last-child a[title="Delete"]').attr("href", result.ctrl + "/delete/" + result.id);
                                    });
                                    $("#grid tbody tr.newRow").removeClass("newRow");
                                }
                            }
                        }
                        else if (result.action == "insquote") {
                            window.location.href = result.RedirectUrl;
                        }
                    } else {
                        $('#progress').hide();
                        if (this.method == "POST") {
                            $('#myModalContent').html(result);
                        }
                        //bindForm();                        
                    }
                }
            });
            return false;
        });
        //alert('hello');

    }

    $(document).ready(muText = function () {
        $("#grid tbody tr").each(function (i, row) {
            var $actualRow = $(row);
            if ($actualRow.find('input[type=checkbox]').prop('checked') == true) {
                $actualRow.addClass('text-muted');
            }
        });
    });

    $("#clearFilter").on('click', function () {
        $.ajax({
            url: this.action,
            type: this.method,
            success: function (result) {
                // feel free to execute any code 
                // in the success callback
                $('#gridContent').html(result);
                $('#search-input').val('').focus();
            }
        });
        return false;
    });
