// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#upload').hide();
    $('#texturl').show();
    $('#othertext').hide();

    //$('#file').change(function () {
    //    $('#texturl').toggle(); $('#upload').toggle();

    //});

    $('#file').change(function () {
        selected_value = $("input[name='file']:checked").val();
        if (selected_value == "URL") {
            $('#texturl').show(); $('#upload').hide();
        }
        else {
            $('#texturl').hide(); $('#upload').show();
        }
    });

    $('#keywords').change(function () {
        //var rr = [];
        //$('"#keywords :selected').each(function (i, selected) {
        //    rr[i] = $(selected).text();
        //});

        var selections = $("#keywords option:selected").text();
        if (selections.indexOf("OTHER") >= 0 ) {
            $('#othertext').show();
        }
        else {
            $('#othertext').hide(); 
        }
    });
});


$('#Available').change(function () {
    if ($('#Available').is(':checked')) { $('#file').show(); $('#texturl').show(); $('#upload').attr('checked', false); $('#texturl').attr('checked', true); $('#upload').hide(); } else { $('#file').hide(); $('#upload').hide(); $('#texturl').hide(); }
});

