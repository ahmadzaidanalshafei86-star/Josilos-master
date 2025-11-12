const baseUrl = window.appBaseUrl || '/';
let removedFiles = []; // removed gallery/images
let removedDocs = []; // removed documents

$(document).ready(function () {
    // Initialize Tender Materials multi-select
    $('#tenderMaterialsSelect').select2({
        theme: "bootstrap-5",
        placeholder: currentLanguage === 'ar-JO' ? "اختر المواد" : "Select materials"
    });

    // Initialize TinyMCE
    tinymce.init({ ...tinyMCEInitSettings, selector: '#tinyMCEDetails, #tinyMCEPricesOffered' });

    // Blink toggle
    const $toggle = $('#SpecialOfferBlinkToggle');
    const $blinkSection = $('#blinkDatesSection');
    function updateBlinkSection() {
        $blinkSection.toggle($toggle.is(':checked'));
    }
    $toggle.on('change', updateBlinkSection);
    updateBlinkSection();

    // Dynamic Other Attachments
    let attachmentIndex = $('#otherAttachmentsContainer .other-attachment-item').length;
    $('#addAttachmentBtn').on('click', function () {
        const html = `
        <div class="row g-2 mb-2 other-attachment-item">
            <div class="col-md-5">
                <input type="text" name="TenderOtherAttachments[${attachmentIndex}].Name" class="form-control" placeholder="${currentLanguage === 'ar-JO' ? 'اسم المرفق' : 'Attachment name'}" required />
            </div>
            <div class="col-md-5">
                <input type="file" name="TenderOtherAttachments[${attachmentIndex}].File" class="form-control" accept=".pdf,.doc,.docx,.jpg,.png" />
            </div>
            <div class="col-md-2 text-end">
                <button type="button" class="btn btn-danger remove-attachment w-100">${currentLanguage === 'ar-JO' ? 'حذف' : 'Remove'}</button>
            </div>
        </div>`;
        $('#otherAttachmentsContainer').append(html);
        attachmentIndex++;
    });

    $(document).on('click', '.remove-attachment', function () {
        $(this).closest('.other-attachment-item').remove();
    });

    // Preview images
    window.previewImage = function (event, previewId) {
        const input = event.target;
        const preview = document.getElementById(previewId);
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = e => preview.src = e.target.result;
            reader.readAsDataURL(input.files[0]);
        }
    };

    // Form submission
    $('#tenderForm').on('submit', function (e) {
        e.preventDefault();
        tinymce.triggerSave();

        const formData = new FormData(this);

        // Append Other Attachments
        $('#otherAttachmentsContainer input[type="file"]').each(function () {
            if (this.files.length > 0) formData.append(this.name, this.files[0]);
        });

        // Submit via fetch
        fetch(this.action, { method: 'POST', body: formData })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    Swal.fire({
                        icon: 'success',
                        title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => {
                        //const redirectUrl = `${baseUrl.endsWith('/') ? baseUrl : baseUrl + '/'}EsAdmin/Tenders`;
                        //window.location.href = redirectUrl;

                        const baseUrlWithSlash = baseUrl.endsWith("/") ? baseUrl : baseUrl + "/";

                        if (data.id) {
                            // Redirect after create
                            //window.location.href = `${baseUrlWithSlash}EsAdmin/Materials/Edit/${data.id}`;
                            window.location.href = `${baseUrlWithSlash}EsAdmin/TenderTranslates?tenderId=${data.id}`;
                        }
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء الإرسال.' : 'An error occurred while submitting.',
                        showConfirmButton: true
                    });
                }
            }).catch(err => {
                console.error(err);
                Swal.fire({
                    icon: 'error',
                    title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء الإرسال.' : 'An error occurred while submitting.',
                    showConfirmButton: true
                });
            });
    });
});
