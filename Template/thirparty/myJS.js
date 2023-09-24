var documentName;
$(document).ready(function () {
    
    //active menu
    documentName = document.location.pathname.replace("/", "");
    $("a[href$='" + documentName + "']").closest(".nav-link").addClass('active').parentsUntil(".nav-sidebar").addClass('menu-open').prev(".nav-link").addClass('active');
    var $aLink = $("a[href$='" + documentName + "']");
    var bc = '';
    var bc = '';

    var pageName = $aLink.text();
    if (documentName == 'Print.aspx') {
        pageName = 'Print'
        bc = '<li class="breadcrumb-item active">Print</li>';
    } else if (documentName == 'ViewForm.aspx') {
        pageName = 'View Form'
        bc = '<li class="breadcrumb-item active">View Form</li>';
    } else if (documentName != 'Home.aspx') {
        $aLink.parents('li').each(function (n, li) {
            var a = $(li).children('a').text();
            var activeBc;
            if (pageName == a) {
                activeBc = 'active'
            }
            bc = '<li class="breadcrumb-item ' + activeBc + '">' + (a) + '</li>' + bc;
        });
    }   else {
        pageName = 'Home'
    }

    if (pageName != '') {
        document.title = pageName + ' | Local Housing Office ';
        $('#pageTitle').text(pageName);
    }

    $('.breadcrumb').html('<li class="breadcrumb-item"><a href="Home.aspx"><i class="ace-icon fa fa-home home-icon"></i>&nbsp;Home</a></li>' + bc);

    //Tables functions
    $('.outputTable').DataTable({
        stateSave: true, "responsive": true, "lengthChange": false, "autoWidth": false,
        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

    //Tables functions
    $('.cepTable').DataTable({
        stateSave: true, "responsive": true, "lengthChange": false, "autoWidth": false,
        "buttons": ["copy", "excel", "print"]
    }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

    $('.basicTable').DataTable({
        stateSave: true, "order": [[0, "desc"]]
    });

    // textbox double format
    $(".decOnly").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    // textbox integer format
    $(".intOnly").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if ((event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    //validate
    $(".form1").validate({
        ignore: [],
        element: 'span',
        //errorPlacement: function (error, element) {
        //    error.addClass('invalid-feedback');
        //    element.closest('.form-group').append(error);
        //},
        //highlight: function (element, errorClass, validClass) {
        //    $(element).addClass('is-invalid');
        //},
        //unhighlight: function (element, errorClass, validClass) {s
        //    $(element).removeClass('is-invalid').addClass('is-valid');
        //}

        highlight: function (element, errorClass, validClass) {
            var elem = $(element);
            if (elem.hasClass("multiple")) {
                $("#" + elem.attr("id")).next("span").find('span').addClass('is-invalid');
            } else if (elem.hasClass("select2-hidden-accessible")) {
                $("#select2-" + elem.attr("id") + "-container").parent().addClass('is-invalid');
            } else {
                elem.addClass('is-invalid');
            }
        },
        unhighlight: function (element, errorClass, validClass) {
            var elem = $(element);
            if (elem.hasClass("multiple")) {
                $("#" + elem.attr("id")).next("span").find('span').removeClass('is-invalid');
            } else if (elem.hasClass("select2-hidden-accessible")) {
                $("#select2-" + elem.attr("id") + "-container").parent().removeClass('is-invalid');
            } else {
                elem.removeClass('is-invalid');
            }
        },
        errorPlacement: function (error, element) {
            var elem = $(element);
            if (elem.hasClass("multiple")) {
                element = $("#" + elem.attr("id")).next("span");
                error.insertAfter(element);
            } else if (elem.hasClass("iGroup")) {
                element = $("#" + elem.attr("id")).parent();
                error.insertAfter(element);
            } else if (elem.hasClass("select2-hidden-accessible")) {
                element = $("#" + elem.attr("id")).next("span");
                error.insertAfter(element);
            } else if (elem.hasClass("datetimepicker-input")) {
                element = $("#" + elem.attr("id")).parent();
                error.insertAfter(element);
            } else if (elem.hasClass("appendGroup")) {
                element = $("#" + elem.attr("id")).parent();
                error.insertAfter(element);
            } else {
                error.insertAfter(element);
            }
        }
    });

    $('#modal_add').on('hidden.bs.modal', function () {
        window.location.replace(documentName);
    });

    $('#modal_status').on('hidden.bs.modal', function () {
        window.location.replace(documentName);
    });

    $('.modal').on('shown.bs.modal', function () {
        $('input:visible:enabled:first,textarea, select', this).first().focus();
    });

    //Initialize Select2 Elements
    $('.select2').select2({ theme: "bootstrap4" })

    //Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

    //validate select2
    $('.select2').select2({}).on("change", function (e) {
        $(this).valid()
    });

    //autocomplete off
    $("input").attr("autocomplete", "off");

    $('.setDateTime').datetimepicker({
        icons: { time: 'far fa-clock' },
        maxDate: new Date()
    });

    var today = new Date();
    $('.startNow').datetimepicker({
        setStartDate: new Date(),
        format: 'L',
        autoclose: 1,
    });

    $('.lastNow').datetimepicker({
        setLastDate: new Date(),
        format: 'L',
        autoclose: 1,        
    });

    $('.datetimepick').daterangepicker({
        timePicker: true,
        //timePickerIncrement: 30,
        locale: {
            format: 'MM/DD/YYYY hh:mm A'
        }
    })

    //Timepicker
    $('.timepicker').datetimepicker({
        format: 'HH:mm',
    })


    var start = moment().startOf('month');
    var end = moment().endOf('month');
    $('.monthDateRange').daterangepicker({
        'applyClass': 'btn-sm btn-success',
        'cancelClass': 'btn-sm btn-default',
        startDate: start,
        endDate: end,
        locale: {
            applyLabel: 'Apply',
            cancelLabel: 'Cancel',
        },
        ranges: {
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
            'Last 3 Months': [moment().subtract(3, 'month').startOf('month'), moment().endOf('month')],
            'Last 6 Months': [moment().subtract(6, 'month').startOf('month'), moment().endOf('month')],
            'This Year': [moment().startOf('year'), moment().endOf('year')]
        }

    });

    $('.thisIsDateRange').daterangepicker();

    //uplaod attachments
    $('.uploadAttachment1').ace_file_input({
        style: 'well',
        btn_choose: 'Drop image here or click to choose',
        btn_change: null,
        no_icon: 'ace-icon fa fa-picture-o',
        droppable: true,
        thumbnail: 'large',
        //before_change: function (files, dropped) {
        //    var fp = $('.uploadAttachment1');
        //    var lg = fp[0].files.length; // get length
        //    var items = fp[0].files;
        //    var fileSize = 0;

        //    for (var i = 0; i < lg; i++) {
        //        fileSize = fileSize + items[i].size; // get file size
        //    }

        //    //if (fileSize > 2048000) {
        //    //    $.gritter.add({
        //    //        title: 'File(s) too big!',
        //    //        text: 'Total file size should not exceed 2Mb!',
        //    //        class_name: 'gritter-error gritter-center'
        //    //    });
        //    //    return false;
        //    //} else {
        //    return true;
        //    //}
        //},
    });

    //view attachments
    var $overflow = '';
    var colorbox_params = {
        rel: 'colorbox',
        reposition: true,
        scalePhotos: true,
        scrolling: false,
        previous: '<i class="ace-icon fa fa-arrow-left"></i>',
        next: '<i class="ace-icon fa fa-arrow-right"></i>',
        close: '&times;',
        current: '{current} of {total}',
        maxWidth: '100%',
        maxHeight: '100%',
        onOpen: function () {
            $overflow = document.body.style.overflow;
            document.body.style.overflow = 'hidden';
        },
        onClosed: function () {
            document.body.style.overflow = $overflow;
        },
        onComplete: function () {
            $.colorbox.resize();
        }
    };

    $('.ace-thumbnails [data-rel="colorbox"]').colorbox(colorbox_params);
    $("#cboxLoadingGraphic").html("<i class='ace-icon fa fa-spinner orange fa-spin'></i>");//let's add a custom loading icon

    $(document).one('ajaxloadstart.page', function (e) {
        $('#colorbox, #cboxOverlay').remove();
    });

    checkWriteUser();

    

    $(".sortable").sortable();

    $('body').on('focus', '[contenteditable]', function () {
        const $this = $(this);
        $this.data('before', $this.html());
    }).on('blur keyup paste input', '[contenteditable]', function () {
        const $this = $(this);
        if ($this.data('before') !== $this.html()) {
            $this.data('before', $this.html());
            $this.trigger('change');
        }
    });
});

function timeDiff(date1, date2) {
    var today = new Date(date1);
    var endDate = new Date(date2);
    var days = parseInt((endDate - today) / (1000 * 60 * 60 * 24));
    var hours = parseInt(Math.abs(endDate - today) / (1000 * 60 * 60) % 24);
    var minutes = parseInt(Math.abs(endDate.getTime() - today.getTime()) / (1000 * 60) % 60);

    return (String(hours).padStart(2, '0') + ':' + String(minutes).padStart(2, '0'))
}

function printMe(refID, mode) {
    window.open("Print.aspx?ID=" + refID + ':' + mode, '_blank');
}
   
function generateTable() {
        var duration = $('#txt_filterDuration').val().split(' - ');
        var status = $('#dl_status').val();

        if (duration != '' && status != '') {
            $.ajax({
                type: "POST",
                url: documentName + "/LoadList",
                data: "{_from: '" + duration[0] + "', _to: '" + duration[1] + "', status: '" + status + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    if (r.d.includes('Error:')) {
                        systemMsg(0, r.d);
                    } else {
                        $('#example1').DataTable().clear();
                        $('#example1').DataTable().destroy();
                        $("#example1").append(r.d);
                        $('#example1').DataTable({
                            stateSave: true,
                            "order": [[0, "desc"]],
                            "responsive": true, "lengthChange": false, "autoWidth": false,
                            "buttons": ["copy", "excel", "print"]
                        }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
                    }
                }, error: function (request, status, error) {
                    systemMsg(0, request.responseText);
                }
            });
        }
    }

function validateEntry(obj, mode) {
        $.ajax({
            type: "POST",
            url: documentName + "/validateEntry",
            data: "{mode: '" + mode + "', entry: '" + obj.value + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (r) {
                if (r.d == 1) {
                    systemMsg(0, 'Entry record already exists.');
                    obj.value = '';
                    obj.focus();
                }
            },
            failure: function (response) {
                systemMsg(0, response.d);
            }
        });
}

function validateForm(className) {
    jQuery.validator.addClassRules(className, {
        required: true
    });

    if ($('.' + className).valid()) {
        return true;
    } else {
        return false;
    }
}

function systemMsg(mode, msg) {
    toastr.clear();
    toastr.options.positionClass = 'toast-bottom-right';
    switch (mode) {
        case 0:
            toastr.error(msg);
            break;
        case 1:
            toastr.success(msg);
            break;
    }
}

function showDetails() {
    $('.divList').attr('hidden', 'hidden');
    $('.divDetails').removeAttr('hidden');
}

function refreshPage() {
    window.location.replace(documentName);
}

function showModalAdd() {
    $('#modal_add').modal('show');
}

function openLink(obj, mode) {
    if (obj != '') {
        window.open('ViewForm.aspx?ID=' + obj + ':' + mode, '_blank');
    }
}

function deleteAttachment(obj, fileCounter) {
    $.ajax({
        type: "POST",
        url: documentName + "/DeleteFile",
        data: "{'downloadPath':'" + obj + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        failure: function (response) {
            systemMsg(0, response.d);
        },
        success: function () {
            $("#" + fileCounter).remove();
            systemMsg(1, 'Selected attachment successfully deleted!');
            //location.reload();
        }
    });
}

function checkWriteUser() {
    $.ajax({
        type: "POST",
        url: "Users.aspx/checkWriteUser",
        data: "{'pageLink':'" + documentName + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        failure: function (response) {
            systemMsg(0, response.d);
        },
        success: function (r) {
            if (parseFloat(r.d) < 3) {
                $('#div_header').removeAttr('hidden');
            }
        }
    });
}

function setUpdateDetails() {
    //let previous = document.getElementById("div_previousVersion");
    //let current = document.getElementById("div_currentVersion");
    //let spans = current.getElementsByTagName("span");

    //for (let span of spans) {
    //    //alert(span.textContent);
    //    span.wrap('<blockqoute = "blockqoute"></blockqoute>');
    //    //if (span.textContent != '') {
    //    //    derived.append(span.textContent);
    //    //}
    //}

    $('.div_currentVersion').find('h1, h2, h3, h4, h5, h6, span, div').each(function () {
        var value = $(this).html();
        var previous = $('.div_previousVersion').text();

        if (previous != '' && !previous.includes(value.trim()) && !value.includes('<br>')) {
            $(this).wrap('<blockquote ="blockquote"></blockquote>')
        }
    });

    //var previous = $('#<%=div_oldVersion.ClientID%>').text();
    //$('.note-editable').find('h1, h2, h3, h4, h5, h6, span').each(function () {
    //    var value = $(this).text();

    //    if (previous != '' && !previous.includes(value.trim()) && !value.includes('<br>')) {
    //        $('#<%=txt_newVersion.ClientID%>').val(previousVerNo + 1);
    //    }
    //});
}
$(document).on('select2:open', () => {
    document.querySelector('.select2-search__field').focus();
    width: 'resolve'
});
