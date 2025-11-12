const baseUrl = window.appBaseUrl || '/';
$(document).ready(function () {
    var languageUrl = '';

    // Determine the language URL based on the current language
    if (currentLanguage === 'ar-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    } else if (currentLanguage === 'en-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    } else {
        // Default to Arabic if not recognized
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    }

    // Initialize DataTable
    var table = $('#table').DataTable({
        "pageLength": 50,
        "lengthMenu": [[50, 75, 100, 120], [50, 75, 100, 120]],
        "language": {
            "url": languageUrl
        },
        "order": [[1, "asc"]]
    });

    // Toggle tender status
    $(document).on('click', '.toggle-status', function () {
        const tenderId = $(this).data('tender-id');
        toggleTenderStatus(tenderId, this);
    });

    // Store original value and handle input changes
    $('.order-input').each(function () {
        const $input = $(this);
        $input.data('original-value', $input.val());
    });

    // Show/hide save icon on change
    $('.order-input').on('input', function () {
        const $input = $(this);
        const $saveIcon = $input.next('.save-order');
        const currentValue = $input.val();
        const originalValue = $input.data('original-value');

        if (currentValue !== originalValue && currentValue && !isNaN(currentValue)) {
            $saveIcon.fadeIn(200);
        } else {
            $saveIcon.fadeOut(200);
        }
    });

    // Enter key handler
    $('.order-input').on('keypress', async function (e) {
        if (e.which === 13) {
            e.preventDefault();
            const $input = $(this);
            const tenderId = $input.data('tender-id');
            const newOrder = $input.val();

            if (!newOrder || isNaN(newOrder)) {
                $input.addClass('is-invalid');
                return;
            }

            await updateOrder($input, tenderId, newOrder);
        }
    });

    // Save icon click handler
    $('.save-order').on('click', async function () {
        const $saveIcon = $(this);
        const tenderId = $saveIcon.data('tender-id');
        const $input = $saveIcon.prev('.order-input');
        const newOrder = $input.val();

        if (!newOrder || isNaN(newOrder)) {
            $input.addClass('is-invalid');
            return;
        }

        await updateOrder($input, tenderId, newOrder);
    });
});

async function updateOrder($input, tenderId, newOrder) {
    try {
        $input.prop('disabled', true);
        const $saveIcon = $input.next('.save-order');
        $saveIcon.addClass('is-loading');

        const updateUrl = `${baseUrl}EsAdmin/Tenders/UpdateOrder?id=${tenderId}&order=${newOrder}`;

        const response = await fetch(updateUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        if (response.status === 200) {
            $input.removeClass('is-loading').addClass('is-valid');
            $saveIcon.removeClass('is-loading').fadeOut(200);
            $input.data('original-value', newOrder); // Update original value after success

            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                showConfirmButton: true
            });

            setTimeout(() => $input.removeClass('is-valid'), 1000);
        } else if (response.status === 403) {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لتعديل هذا العطاء!' : 'You don\'t have permission to edit this tender!',
                showConfirmButton: true
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن تحديث الترتيب' : 'Can\'t update order',
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ' : 'An error occurred',
            showConfirmButton: true
        });
    } finally {
        $input.prop('disabled', false);
        $input.next('.save-order').removeClass('is-loading');
    }
}

async function toggleTenderStatus(tenderId, element) {
    try {
        const ToggleUrl = `${baseUrl}EsAdmin/Tenders/ToggleStatus?id=${tenderId}`;
        const response = await fetch(ToggleUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        if (response.status === 200) {
            const row = $(element).closest('tr');
            const badgeSpan = row.find('td span.badge');

            if (badgeSpan.length) {
                const isActive = badgeSpan.text().trim() === 'Published';
                badgeSpan.hide().text(isActive ? 'Not published' : 'Published').fadeIn(200);
                badgeSpan.toggleClass('bg-success bg-danger');
            }

            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                showConfirmButton: true
            });

            setTimeout(function () {
                location.reload();
            }, 2000);
        } else if (response.status === 403) {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لتعديل حالة هذا العطاء!' : 'You don\'t have permission to edit this tender status!',
                showConfirmButton: true
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن نشر العطاء' : 'Can\'t publish tender',
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ' : 'An error occurred',
            showConfirmButton: true
        });
    }
}

async function deleteTender(tenderId, element) {
    console.log('deleteTender called for ID:', tenderId);

    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد من أنك تريد حذف هذا العطاء؟' : 'Are you sure you want to delete this tender?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) return;

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/Tenders/Delete?id=${tenderId}`;
        const response = await fetch(DeleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        console.log('Response:', response);

        if (response.ok) {
            console.log('Tender deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف العطاء بنجاح!' : 'Tender deleted successfully!',
                showConfirmButton: true
            });
        } else if (response.status == 403) {
            const responseText = await response.text();
            console.error('Failed to delete tender:', responseText);

            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لحذف هذا العطاء!' : 'You don\'t have permission to delete this tender!',
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error deleting tender', error);
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف العطاء' : 'An error occurred while deleting the tender.',
            showConfirmButton: true
        });
    }
}
