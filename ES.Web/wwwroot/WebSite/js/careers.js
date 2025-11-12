$(document).ready(function () {
    // Handle Apply link click
    $('.apply-link').on('click', function (e) {
        e.preventDefault();
        var careerId = $(this).data('career-id');
        var $form = $('#apply-form-' + careerId);

        // Collapse any other open forms
        $('.apply-form').not($form).collapse('hide');

        // Toggle the selected form
        $form.collapse('toggle');
    });

    // Handle Cancel button click
    $('.btn-cancel').on('click', function () {
        var careerId = $(this).data('career-id');
        $('#apply-form-' + careerId).collapse('hide');
    });

    // Form validation to ensure careerId is present
    $('form').on('submit', function (e) {
        if (!$(this).find('input[name="careerId"]').val()) {
            e.preventDefault();
            alert('Please select a job to apply for.');
        }

        $('.checkbox-validator').each(function () {
            var fieldName = $(this).attr('name');
            var checkedCount = $('input[name="' + fieldName + '"]:checked').length;
            if (checkedCount === 0) {
                e.preventDefault();
                $(this).siblings('.text-danger').show();
            } else {
                $(this).siblings('.text-danger').hide();
            }
        });
    });
});