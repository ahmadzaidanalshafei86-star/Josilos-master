$(document).ready(function () {
    var languageUrl = '';

    // Determine the language URL based on the current language
    if (currentLanguage === 'ar-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    } else if (currentLanguage === 'en-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    } else {
        // Default to Arabic
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    }

    var table = $('#table').DataTable({
        "pageLength": 5,
        "lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
        fixedHeader: true,
        "language": {
            "url": languageUrl
        },
        "order": [[0, "asc"]]
    });


    // Toggle delivery status
    $(document).on('click', '.toggle-status', function () {
        const deliveryId = $(this).data('delivery-id');
        toggleStatus(deliveryId, this);
    });
});

const baseUrl = window.appBaseUrl || '/';

async function toggleStatus(deliveryId, element) {
    try {
        const toggleUrl = `${baseUrl}EsAdmin/ProductDeliveries/ToggleStatus?id=${deliveryId}`;
        const response = await fetch(toggleUrl, {
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
                const isActive = badgeSpan.attr("data-status") === "Active";

                badgeSpan.hide()
                    .text(isActive ? "Not Available" : "Available")
                    .attr("data-status", isActive ? "NotActive" : "Active")
                    .fadeIn(200);

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
                title: currentLanguage === 'ar-JO' ? 'ليس لديك صلاحية لتعديل هذه المنطقة!' : 'You don\'t have permission to edit this delivery zone!',
                showConfirmButton: true
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن النشر' : 'Can\'t publish',
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

async function deleteDelivery(deliveryId, element) {
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد أنك تريد حذف هذه المنطقة؟' : 'Are you sure you want to delete this delivery zone?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) return;

    try {
        const deleteUrl = `${baseUrl}EsAdmin/ProductDeliveries/Delete?id=${deliveryId}`;

        const response = await fetch(deleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف المنطقة بنجاح!' : 'Delivery zone deleted successfully!',
                showConfirmButton: true
            });

        } else if (response.status === 403) {
            const responseText = await response.text();
            console.error('Permission denied:', responseText);

            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك صلاحية لحذف هذه المنطقة!' : 'You don\'t have permission to delete this delivery zone!',
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error deleting delivery zone:', error);
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف المنطقة.' : 'An error occurred while deleting the delivery zone.',
            showConfirmButton: true
        });
    }
}
