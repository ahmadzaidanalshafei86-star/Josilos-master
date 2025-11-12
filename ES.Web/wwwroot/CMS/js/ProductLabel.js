const baseUrl = window.appBaseUrl || '/';

$(function () {

    // Initialize Bootstrap tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-tooltip="tooltip"]');
    const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

    // Determine language URL based on currentLanguage
    let languageUrl = 'https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json';
    if (currentLanguage === 'ar-JO') {
        languageUrl = 'https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json';
    } else if (currentLanguage === 'en-JO') {
        languageUrl = 'https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json';
    }

    // Initialize DataTable
    let table = $('#table').DataTable({
        pageLength: 10,
        lengthMenu: [[10, 20, 40, 50], [10, 20, 40, 50]],
        fixedHeader: true,
        language: {
            url: languageUrl
        },
        destroy: true
    });

    // Populate edit modal
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        const name = $(this).data('name');
        $('#editId').val(id);
        $('#editName').val(name);
    });

    // Handle delete button click with SweetAlert2
    $(document).on('click', '.delete-btn', async function () {
        const id = $(this).data('id');
        const row = $(this).closest('tr');

        // Confirm deletion using SweetAlert2
        const confirmDeletion = await Swal.fire({
            icon: 'warning',
            title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد أنك تريد حذف هذه التسمية؟' : 'Are you sure you want to delete this label?',
            showCancelButton: true,
            confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
            cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
        });

        if (!confirmDeletion.isConfirmed) {
            return;
        }

        try {
            const deleteUrl = `${baseUrl}EsAdmin/ProductLabels/Delete`;
            const token = $('input[name="__RequestVerificationToken"]').val();

            const response = await fetch(deleteUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: `id=${encodeURIComponent(id)}&__RequestVerificationToken=${encodeURIComponent(token)}`
            });

            console.log('Response:', response);

            if (response.ok) {
                console.log('Label deleted successfully.');
                table.row(row).remove().draw(false);

                // Show success message
                await Swal.fire({
                    icon: 'success',
                    title: currentLanguage === 'ar-JO' ? 'تم حذف التسمية بنجاح!' : 'Label deleted successfully!',
                    showConfirmButton: true
                });
            } else if (response.status === 403) {
                const responseText = await response.text();
                console.error('Failed to delete label:', responseText);

                // Show permission error
                await Swal.fire({
                    icon: 'error',
                    title: currentLanguage === 'ar-JO' ? 'ليس لديك صلاحية لحذف هذه التسمية!' : 'You don\'t have permission to delete this label!',
                    showConfirmButton: true
                });
            } else {
                const responseText = await response.text();
                console.error('Failed to delete label:', responseText);

                // Show generic error
                await Swal.fire({
                    icon: 'error',
                    title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف التسمية.' : 'An error occurred while deleting the label.',
                    showConfirmButton: true
                });
            }
        } catch (error) {
            console.error('Error deleting label:', error);

            // Show error message
            await Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف التسمية.' : 'An error occurred while deleting the label.',
                showConfirmButton: true
            });
        }
    });

    // Handle add modal display for validation errors
    if (window.hasValidationErrors) {
        const addModalElement = document.getElementById('addProductLabelModal');
        const addModal = new bootstrap.Modal(addModalElement, {
            backdrop: 'static',
            keyboard: false
        });
        addModal.show();

        // Clean up modal state on close
        addModalElement.addEventListener('hidden.bs.modal', function () {
            addModal.dispose();
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css('padding-right', '');
        });
    }

    // Ensure modal backdrop is removed when any modal is closed
    $('#addProductLabelModal, #editProductLabelModal').on('hidden.bs.modal', function () {
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');
    });
});