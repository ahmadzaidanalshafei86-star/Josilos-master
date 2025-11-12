const baseUrl = window.appBaseUrl || '/';


$(document).ready(function () {
    // Initially hide the Submit Selected button
    toggleSubmitButton();

    var languageUrl = '';

    // Determine the language URL based on the current language
    if (currentLanguage === 'ar-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    } else if (currentLanguage === 'en-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    } else {
        // Default to English if the language is not set or recognized
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    }

    $('table').DataTable({
        "pageLength": 5,
        "lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
        fixedHeader: true,
        autoWidth: false, // Disable automatic column width calculation
        "language": {
            "url": languageUrl
        }
    });


    // Select/Deselect all checkboxes
    $('#select-all').click(function () {
        $('.user-checkbox').prop('checked', this.checked);
        toggleSubmitButton();
    });

    // Toggle the visibility of the Submit Selected button
    $('.user-checkbox').change(function () {
        toggleSubmitButton();
    });

    // Collect selected user IDs and submit them
    $('#submit-selected').click(function () {
        var selectedUserIds = [];
        $('.user-checkbox:checked').each(function () {
            selectedUserIds.push($(this).val());
        });

        const BulkToggleStatusUrl = `${baseUrl}EsAdmin/Users/BulkToggleStatus`;
        if (selectedUserIds.length > 0) {
            $.ajax({
                url: BulkToggleStatusUrl,
                type: 'POST',
                data: JSON.stringify(selectedUserIds),
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    // Messages based on current language
                    const successMessage = currentLanguage === 'ar-JO' ? 'تم تعليق المستخدمين المحددين بنجاح.' : 'Selected users are suspended successfully.';

                    Swal.fire({
                        icon: 'success',
                        title: successMessage,
                        showConfirmButton: true
                    });

                    setTimeout(function () {
                        location.reload(); // Reload the page after 2 seconds
                    }, 2000);
                },
                error: function (xhr, status, error) {
                    // Messages based on current language
                    const errorMessage = currentLanguage === 'ar-JO' ? 'تم رفض الوصول' : 'Access denied';

                    Swal.fire({
                        icon: 'error',
                        title: errorMessage,
                        showConfirmButton: true
                    });
                }
            });

        } else {
            // Messages based on current language
            const warningMessage = currentLanguage === 'ar-JO' ? 'يرجى تحديد مستخدم واحد على الأقل.' : 'Please select at least one user.';

            Swal.fire({
                icon: 'warning',
                title: warningMessage,
                showConfirmButton: true
            });
        }

    });

    $(document).on('shown.bs.dropdown', '.dropdown', function () {
        const $dropdownMenu = $(this).find('.dropdown-menu');
        const offset = $dropdownMenu.offset();
        $dropdownMenu.css({
            position: 'absolute',
            top: offset.top,
            left: offset.left,
            zIndex: 1050
        }).appendTo('body');
    });
    // Toggle user status
    $(document).on('click', '.toggle-status', function () {
        const userId = $(this).data('user-id');
        toggleUserStatus(userId, this);
    });
});

function toggleSubmitButton() {
    if ($('.user-checkbox:checked').length > 0) {
        $('#submit-selected').show();
    } else {
        $('#submit-selected').hide();
    }
}

async function toggleUserStatus(userId, element) {
    try {
        const toggleUrl = `${baseUrl}EsAdmin/Users/ToggleStatus?id=${userId}`;
        const response = await fetch(toggleUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        // Messages based on the current language
        const successMessage = currentLanguage === 'ar-JO' ? 'تم تبديل حالة المستخدم بنجاح!' : 'User status toggled successfully!';
        const permissionDeniedMessage = currentLanguage === 'ar-JO' ? 'ليس لديك إذن للقيام بهذا الاجراء.' : 'You don\'t have permission to do this.';
        const errorMessage = currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء تبديل الحالة.' : 'An error occurred while toggling the status.';

        // Only show success if the response is OK (status 200)
        if (response.status === 200) {
            const row = $(element).closest('tr');
            const badgeSpan = row.find('td span.badge');

            if (badgeSpan.length) {
                const isActive = badgeSpan.text().trim() === 'Active';
                badgeSpan.hide().text(isActive ? 'Not Active' : 'Active').fadeIn(200);
                badgeSpan.toggleClass('bg-success bg-danger');
            }

            Swal.fire({
                icon: 'success',
                title: successMessage,
                showConfirmButton: true
            });

            setTimeout(function () {
                location.reload(); // Reload the page after 2 seconds
            }, 2000);
        } else if (response.status === 403) {
            Swal.fire({
                icon: 'error',
                title: permissionDeniedMessage,
                showConfirmButton: true
            });
        } else {
            // Show generic error for other status codes
            Swal.fire({
                icon: 'error',
                title: errorMessage,
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: errorMessage,
            showConfirmButton: true
        });
    }
}


async function deleteUser(userId, element) {
    // Confirm deletion with SweetAlert2
    const confirmMessage = currentLanguage === 'ar-JO' ?
        'هل أنت متأكد أنك تريد حذف هذا المستخدم؟' :
        'Are you sure you want to delete this user?';

    const result = await Swal.fire({
        icon: 'question',
        title: confirmMessage,
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/Users/Delete?id=${userId}`;
        const response = await fetch(DeleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        // Only show success if the response is OK (status 200)
        if (response.status === 200) {
            const row = $(element).closest('tr');
            row.fadeOut(500, function () {
                $(this).remove();
            });

            const successMessage = currentLanguage === 'ar-JO' ?
                'تم حذف المستخدم بنجاح!' :
                'User deleted successfully!';

            Swal.fire({
                icon: 'success',
                title: successMessage,
                showConfirmButton: true
            });

            setTimeout(function () {
                location.reload(); // Reload the page after 2 seconds
            }, 2000);
        } else if (response.status === 403) {
            // Redirect to Access Denied page for 403 error
            window.location.href = '/Identity/Account/AccessDenied';
        } else if (response.status === 400) {
            // Show generic error for other status codes
            const errorMessage = currentLanguage === 'ar-JO' ?
                'لا يمكنك حذف حسابك!' :
                "You can't delete your account!";
            Swal.fire({
                icon: 'error',
                title: errorMessage,
                showConfirmButton: true
            });
        } else {
            const errorMessage = currentLanguage === 'ar-JO' ?
                'خطأ أثناء حذف هذا المستخدم' :
                'Error while deleting this user';
            Swal.fire({
                icon: 'error',
                title: errorMessage,
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error:', error);
        const errorMessage = currentLanguage === 'ar-JO' ?
            'حدث خطأ أثناء حذف المستخدم' :
            'An error occurred while deleting the user.';
        Swal.fire({
            icon: 'error',
            title: errorMessage,
            showConfirmButton: true
        });
    }
}

