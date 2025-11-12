selectedProducts = selectedProducts || [];
const baseUrl = window.appBaseUrl || '/';
$(document).ready(function () {
    // Determine the placeholder language based on the current language
    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#CategoryDropdown', 'حدد القسم');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#CategoryDropdown', 'Select the category');
    }

    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#BrandsDropdown', 'حدد العلامة التجارية');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#BrandsDropdown', 'Select the brand');
    }

    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#LabelsDropdown');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#LabelsDropdown');
    }


    const videoUrlInput = $("input[name='VideoUrl']");
    const initialUrl = videoUrlInput.val();
    if (initialUrl) {
        updateVideoPreview(initialUrl);
    }
    videoUrlInput.on("input", function () {
        updateVideoPreview(this.value);
    });

    $('[data-bs-toggle="tooltip"]').tooltip();

    // Attach event listeners for all "Clear" buttons dynamically
    initializeClearButtons([
        { inputId: 'CoverImage', previewId: 'CoverImagePreview', buttonId: 'clearCoverImage', hiddenInputId: 'KeepCoverImage' },
        { inputId: 'FeaturedImage', previewId: 'FeaturedImagePreview', buttonId: 'clearFeaturedImage', hiddenInputId: 'KeepFeaturedImage' },
    ]);

    // Initialize Datetimepicker For Start and End Sale Dates
    initializeDatePicker('.sale-start-date');
    initializeDatePicker('.sale-end-date');

    // Toggle Sale Schedule for Pricing Tab
    $('#toggleSchedule').click(function () {
        let $saleSchedule = $('#saleScheduleFields');

        if ($saleSchedule.is(':visible')) {
            $saleSchedule.stop(true, true).slideUp();
        } else {
            $saleSchedule.stop(true, true).slideDown();
        }
    });


    $(document).on('click', '.toggleSchedule', function () {
        let targetId = $(this).data('target');
        $(targetId).stop(true, true).slideToggle();
    });

    $('#productDataTabs a').click(function (e) {
        e.preventDefault();
        $('#productDataTabs .nav-link').removeClass('active'); // Remove active class from all tabs
        $(this).addClass('active'); // Add active class to the clicked tab
        $(this).tab('show'); // Show the corresponding tab
    });


    $('#SalePrice, #RegularPrice').on('input', function () {
        var regularPrice = parseFloat($('#RegularPrice').val());
        var salePrice = parseFloat($('#SalePrice').val());

        if (!isNaN(regularPrice) && !isNaN(salePrice) && salePrice > regularPrice) {
            $('#salePriceError').text("Sale Price cannot be greater than Regular Price !.");
        } else {
            $('#salePriceError').text("");
        }
    });

    $('#manageStockCheckbox').change(toggleStockFields);
    toggleStockFields(); // Run on page load

    // used in linked products{
    $('#categoryFilterDropdown').change(loadProductsByCategory);
    $('#saveLinkedProducts').click(saveSelectedProducts);
    $(document).on('click', '.remove-product', removeProduct);
    $('#productSearch').on('keyup', filterProductsByName);

    // Clear search bar & restore all products when modal is closed
    $('#linkedProductsModal').on('hidden.bs.modal', function () {
        $('#productSearch').val('');
        $('#productList tr').show();
    });

    // Keep previously selected products checked when modal opens
    $('#linkedProductsModal').on('shown.bs.modal', function () {
        $('.product-checkbox').each(function () {
            let productId = $(this).val();
            $(this).prop('checked', selectedProducts.includes(productId));
        });
    });

    // Get already linked products from the view in edit
    document.querySelectorAll("#selectedProductsList li").forEach(item => {
        selectedProducts.push(item.getAttribute("data-product-id"));
    });


    //}

    // Prodcut attributes {

    toggleAttributesTab();
    $("#ProductType").change(function () {
        toggleAttributesTab();
    });
    $("#attributeDropdown").change(function () {
        var selectedAttrId = $(this).val(); // Get selected attribute ID
        if (selectedAttrId) {
            var attributeCard = $("#attribute_" + selectedAttrId);
            if (attributeCard.length) {
                attributeCard.removeClass("d-none"); // Show card
            }
        }
    });

    $(document).on("click", ".remove-attribute", function () {
        var attrId = $(this).data("attr-id");
        $("#attribute_" + attrId).addClass("d-none");

        // Uncheck all values inside the removed attribute
        $("#attribute_" + attrId).find("input[type=checkbox]").prop("checked", false);
        $("#attribute_" + attrId).find("input, select, textarea").prop("disabled", true);
    });


    $(document).on("click", ".btn[data-bs-toggle='collapse']", function () {
        var icon = $(this).find("i");
        if (icon.hasClass("lni-chevron-down")) {
            icon.removeClass("lni-chevron-down").addClass("lni-chevron-up");
        } else {
            icon.removeClass("lni-chevron-up").addClass("lni-chevron-down");
        }
    });

    // Attach event listeners dynamically for all existing and future "Clear" buttons
    $(document).on("click", "[id^=clearAttributeImage_]", function () {
        let buttonId = $(this).attr("id"); // Get button ID
        let valueId = buttonId.split("_")[1]; // Extract value ID from button ID

        clearFileInput(`attributeImage_${valueId}`, `attributeImagePreview_${valueId}`, `KeepImage_${valueId}`);
    });

    // Show selected attribute cards on edit
    $(".card.d-none").each(function () {
        var hasCheckedValues = $(this).find("input[type=checkbox]:checked").length > 0;
        if (hasCheckedValues) {
            $(this).removeClass("d-none");
        }
    });

    // Ensure sale schedule fields show when dates exist
    $(".sale-start-date, .sale-end-date").each(function () {
        if ($(this).val()) {
            $(this).closest(".row").show();
        }
    });

    // }
    // Tabs {
    let tabIndex = $("#productTabsContainer .tab-item").length;

    initializeTinyMCE();
    $("#addTabBtn").click(function () {
        addNewTab();
    });
    $(document).on("click", ".remove-tab", function () {
        let tabContainer = $(this).closest(".tab-item");
        let textAreaId = tabContainer.find("textarea").attr("id");

        // Remove TinyMCE instance if it exists
        if (tinymce.get(textAreaId)) {
            tinymce.get(textAreaId).remove();
        }

        tabContainer.remove();
        renumberTabs(); // Renumber remaining tabs
    });
    // }

    //  Cover Image Validation
    $("#CoverImage").change(function () {
        validateImage(this, "CoverImagePreview", "coverImageError");
    });

    // Bind Featured Image Validation
    $("#FeaturedImage").change(function () {
        validateImage(this, "FeaturedImagePreview", "featuredImageError");
    });

    // Handle dynamic Attribute Images (Event Delegation for Dynamic Elements)
    $(document).on("change", "[id^='attributeImage_']", function () {
        const id = this.id.split("_")[1];
        validateImage(this, "attributeImagePreview_" + id, "attributeImageError_" + id);
    });


});

function validateImage(input, errorElementId) {
    const errorElement = document.getElementById(errorElementId);
    if (!errorElement) {
        console.warn(`Error element not found: ${errorElementId}`);
        return true; 
    }

    errorElement.textContent = ""; 

    if (input.files.length > 0) {
        const file = input.files[0];
        const validExtensions = ["image/jpeg", "image/jpg" ,"image/png", "image/webp"];
        const maxSize = 5 * 1024 * 1024; // 5MB

        if (!validExtensions.includes(file.type)) {
            errorElement.textContent = "Invalid file type. Only JPG, PNG, and WEBP are allowed.";
            return false;
        }
        if (file.size > maxSize) {
            errorElement.textContent = "File size must be 5MB or less.";
            return false;
        }
    }
    return true; 
}




// Initialize Select2
function initializeSelect2(selector, placeholderText) {
    $(selector).select2({
        theme: "bootstrap-5",
        placeholder: placeholderText,
        minimumResultsForSearch: 0 // Enables search for all dropdowns
    });
    // Ensure placeholder appears on clear
    $(selector).on('select2:clear', function () {
        $(this).val(null).trigger('change');
    });
}

function initializeClearButtons(buttonsConfig) {
    buttonsConfig.forEach(function (config) {
        const button = document.getElementById(config.buttonId);
        if (button) {
            button.addEventListener('click', function () {
                clearFileInput(config.inputId, config.previewId, config.hiddenInputId);
            });
        }
    });
}

function initializeDatePicker(selector) {
    if (typeof flatpickr === "undefined") {
        console.error("Flatpickr is not loaded.");
        return;
    }
    if (currentLanguage === 'ar-JO') {
        var lang = "ar";
    } else if (currentLanguage === 'en-US') {
        lang = "en";
    }
    flatpickr(selector, {
        dateFormat: "Y-m-d h:i K", // Format: YYYY-MM-DD HH:MM AM/PM
        enableTime: true, // Enable time selection
        time_24hr: false, // Use 12-hour format with AM/PM
        locale: lang, // Set locale
        disableMobile: true, // Ensure consistent behavior across devices
        minDate: "today", // Disable past dates (optional)
        position: "auto center", // Adjust positioning
        theme: "material_blue", // Modern theme
        defaultHour: 12, // Default to 12:00 PM
        defaultMinute: 0, // Ensure minute starts at 00
        allowInput: true, // Let users type a date
    });

}

function previewImage(event, previewElementId) {
    const input = event.target;
    const preview = document.getElementById(previewElementId);

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;

            // Reset the hidden input to true when a new image is selected
            const hiddenInputId = input.getAttribute('data-hidden-input-id');
            if (hiddenInputId) {
                const hiddenInput = document.getElementById(hiddenInputId);
                if (hiddenInput) hiddenInput.value = "true";
            }
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function clearFileInput(inputId, previewElementId, hiddenInputId) {
    const input = document.getElementById(inputId);
    const preview = document.getElementById(previewElementId);
    const hiddenInput = document.getElementById(hiddenInputId);

    // Clear the file input value
    input.value = null;

    // Reset the preview to the placeholder image
    preview.src = "/images/placeholder.png";

    // Set the hidden input value to false
    if (hiddenInput) {
        hiddenInput.value = "false";
    }
}

function updateVideoPreview(url) {
    const iframe = $("#videoPreview");

    if (url.includes("youtube.com") || url.includes("youtu.be")) {
        const videoId = extractYouTubeID(url);
        iframe.attr("src", videoId ? `https://www.youtube.com/embed/${videoId}` : "");
    } else if (url.includes("vimeo.com")) {
        const videoId = extractVimeoID(url);
        iframe.attr("src", videoId ? `https://player.vimeo.com/video/${videoId}` : "");
    } else {
        iframe.attr("src", "");
    }
}
function extractYouTubeID(url) {
    const match = url.match(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/\s]{11})/);
    return match ? match[1] : null;
}
function extractVimeoID(url) {
    const match = url.match(/vimeo\.com\/(\d+)/);
    return match ? match[1] : null;
}

function toggleStockFields() {
    if ($('#manageStockCheckbox').is(':checked')) {
        $('#stockManagementFields').slideDown();
        $('#stockStatusField').slideUp();
    } else {
        $('#stockManagementFields').slideUp();
        $('#stockStatusField').slideDown();
    }
}

// used in linked productes {
function loadProductsByCategory() {
    let selectedCategory = $('#categoryFilterDropdown').val();
    const loadProductsUrl = `${baseUrl}EsAdmin/Products/GetProductsByCategory`;
    $.ajax({
        url: loadProductsUrl,
        type: 'GET',
        data: { categoryId: selectedCategory || 0 }, // Default to 0 if no category is selected
        success: updateProductList,
        error: function () {
            console.error('Error fetching products');
        }
    });
}
/**
 * Updates the modal product list dynamically.
 * @param {Array} products - List of products from the AJAX response.
 */
function updateProductList(products) {
    let productList = $('#productList');
    productList.empty();

    if (products.length === 0) {
        productList.append('<tr><td colspan="2">No products found.</td></tr>');
        return;
    }

    products.forEach(product => {
        let isChecked = selectedProducts.includes(product.id.toString()) ? "checked" : "";

        productList.append(`
            <tr>
                <td><input type="checkbox" class="product-checkbox" value="${product.id}"  data-product-name="${product.name}" ${isChecked} /></td>
                <td>${product.name}</td>
            </tr>
        `);
    });
}

function saveSelectedProducts() {
    $('.product-checkbox:checked').each(function () {
        let productId = $(this).val().toString();  // ✅ Ensure it's always a string
        let productName = $(this).attr('data-product-name');

        if (!selectedProducts.includes(productId)) {
            selectedProducts.push(productId);  // ✅ Now all IDs are strings

            $('#selectedProductsList').append(`
                <li class="list-group-item d-flex justify-content-between align-items-center" data-product-id="${productId}">
                    <span><i class="bx bx-cube md hydrated"></i> ${productName}</span>
                    <button type="button" class="btn btn-danger btn-sm remove-product">
                        <i class="lni lni-close"></i>
                    </button>
                </li>
            `);
        }
    });

    $('#linkedProductsModal').modal('hide');
}



//Updates the displayed list of selected products.
function updateSelectedProductsList() {
    let selectedListContainer = $('#selectedProductsList');
    selectedListContainer.empty();

    selectedProducts.forEach(productId => {
        let productName = $('#productList').find(`.product-checkbox[value="${productId}"]`).closest('tr').find('td:eq(1)').text();

        selectedListContainer.append(`
            <li class="list-group-item d-flex justify-content-between align-items-center" data-product-id="${productId}">
                ${productName}
                <button type="button" class="btn btn-danger btn-sm remove-product">
                    <i class="lni lni-close"></i>
                </button>
            </li>
        `);
    });
}

//Removes a product from the selected list.
function removeProduct() {
    let productId = $(this).closest('li').data('product-id');

    // Remove from selectedProducts array
    selectedProducts = selectedProducts.filter(id => id != productId);

    // Remove from the UI
    $(this).closest('li').remove();

    // Uncheck the corresponding checkbox in the modal
    $('.product-checkbox[value="' + productId + '"]').prop('checked', false);
}

function filterProductsByName() {
    let searchText = $('#productSearch').val().toLowerCase();

    $('#productList tr').each(function () {
        let productName = $(this).find('td:eq(1)').text().toLowerCase();
        $(this).toggle(productName.includes(searchText));
    });
}
// } 


//Product attributes{
function toggleAttributesTab() {
    var productType = $("#ProductType").val();
    var attributesTab = $("a[href='#attributesTab']");
    var attributesContent = $("#attributesTab");
    var pricingTab = $("a[href='#pricingTab']");
    var pricingContent = $("#pricingTab");

    if (productType === "variable") {
        attributesTab.show();
        attributesContent.show();
    } else {
        attributesTab.hide();
        attributesContent.hide();

        // Show the Pricing tab and its content
        pricingTab.tab('show');
        pricingContent.addClass("show active");
    }
}


// }

// Tabs {
function addNewTab() {
    let tabIndex = $("#productTabsContainer .tab-item").length;

    let newTabHtml = `
        <div class="card shadow-sm border-0 rounded-3 mb-3 tab-item">
            <div class="card-body position-relative">
                <button type="button" class="btn-close btn-outline-danger remove-tab position-absolute top-0 end-0 m-2"></button>
                
                <div class="row g-2 mb-3">
                    <div class="col-md-6">
                        <label class="form-label fw-medium">${getTranslation("Tab Title")}</label>
                        <input type="text" class="form-control" name="ProductTabs[${tabIndex}].Title" maxlength="40" required/>
                        <span class="text-danger field-validation-valid" data-valmsg-for="ProductTabs[${tabIndex}].Title" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-medium">${getTranslation("Tab Order")}</label>
                        <input type="number" class="form-control" name="ProductTabs[${tabIndex}].Order" min="1" value="${tabIndex + 1}"/>
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label fw-medium">${getTranslation("Tab Content")}</label>
                    <textarea class="form-control tinyMCE" id="tinyMCE_${tabIndex}" name="ProductTabs[${tabIndex}].Content" rows="5"></textarea>
                </div>

                <input type="hidden" name="ProductTabs[${tabIndex}].Id" value="0" />
            </div>
        </div>`;

    $("#productTabsContainer").append(newTabHtml);

    // Reinitialize TinyMCE for the new tab content
    initializeTinyMCE(`#tinyMCE_${tabIndex}`);
}

// Function to renumber the tabs after one is removed
function renumberTabs() {
    $("#productTabsContainer .tab-item").each(function (index) {
        $(this).find("input[name^='ProductTabs']").each(function () {
            let name = $(this).attr("name").replace(/ProductTabs\[\d+\]/, `ProductTabs[${index}]`);
            $(this).attr("name", name);
        });
        $(this).find("textarea[name^='ProductTabs']").each(function () {
            let name = $(this).attr("name").replace(/ProductTabs\[\d+\]/, `ProductTabs[${index}]`);
            $(this).attr("name", name);
            let id = `tinyMCE_${index}`;
            $(this).attr("id", id);
        });
        $(this).find("input[name$='.Order']").val(index + 1); // Renumber order input
    });
    initializeTinyMCE();
}
// Function to initialize TinyMCE for a given selector
function initializeTinyMCE(selector) {
    tinymce.init({
        ...tinyMCEInitSettings, // Use your global TinyMCE settings
        selector: selector
    });
}

// }


//Gallery Images
// Get existing images from the server
Dropzone.autoDiscover = false;
var removedFiles = []; // Global array to track removed files


var myDropzone = new Dropzone("#myDropzone", {
    previewsContainer: "#sortablePreviews", // Target the custom container for previews
    url: "#",
    autoProcessQueue: false,
    maxFiles: 100,
    maxFilesize: 5, // File size limit in MB
    acceptedFiles: ".jpg,.jpeg,.png,.gif",
    addRemoveLinks: true,
    dictDefaultMessage: 'Drag and drop gallery images here or click to upload',
    init: async function () {
        var dropzoneInstance = this;

        // Load existing images for editing
        if (existingGalleryImages && existingGalleryImages.length > 0) {
            for (const image of existingGalleryImages) {
                const imageUrl = `${baseUrl}images/Products/GalleryImages/` + image.galleryImageUrl;
                const fileName = image.galleryImageAltName;


                // Fetch the image and convert it to a File object
                const file = await fetchImageAsFile(imageUrl, fileName);

                // Create a mockFile object to display the image in Dropzone
                const mockFile = {
                    name: fileName,
                    size: file.size,
                    accepted: true,
                    status: Dropzone.ADDED,
                    previewElement: null
                };

                // Add the mock file to Dropzone
                dropzoneInstance.emit("addedfile", mockFile);

                // Customize the preview element
                const previewElement = mockFile.previewElement;
                if (previewElement) {
                    const thumbnailElement = previewElement.querySelector('.dz-image');
                    if (thumbnailElement) {
                        thumbnailElement.innerHTML = '';
                        const imgElement = document.createElement('img');
                        imgElement.src = imageUrl;
                        imgElement.style.width = "100%";
                        imgElement.style.height = "100%";
                        thumbnailElement.appendChild(imgElement);
                    }
                }

                // Mark the file as successfully uploaded
                mockFile.status = Dropzone.SUCCESS;
                mockFile.previewElement.classList.add("dz-success", "dz-complete");

                // Add the File object to Dropzone's files array
                dropzoneInstance.files.push(file);
            }
        }

        // Track removed files
        dropzoneInstance.on("removedfile", function (file) {
            removedFiles.push(file.name); // Add the removed file's name to the list
            console.log("Removed file:", file.name);
        });

        // Make previews sortable
        Sortable.create(document.querySelector("#sortablePreviews"), {
            animation: 380,
            draggable: ".dz-preview",
            onEnd: function () {
                var sortedPreviews = document.querySelectorAll("#sortablePreviews .dz-preview");
                var newFilesOrder = [];

                sortedPreviews.forEach(function (previewElement) {
                    var fileName = previewElement.querySelector("[data-dz-name]").innerText;
                    var matchedFile = dropzoneInstance.files.find(file => file.name === fileName);
                    if (matchedFile) {
                        newFilesOrder.push(matchedFile);
                    }
                });

                dropzoneInstance.files = newFilesOrder;
                console.log("New file order:", dropzoneInstance.files.map(file => file.name));
            }
        });
    }
});


// Function to fetch an image and convert it to a File object
async function fetchImageAsFile(imageUrl, fileName) {
    const response = await fetch(imageUrl);
    const blob = await response.blob();
    return new File([blob], fileName, { type: blob.type });
}


document.getElementById("my-Form").addEventListener("submit", function (e) {
    e.preventDefault(); // Prevent default form submission

    tinymce.triggerSave();

    // Original FormData
    const originalFormData = new FormData(this);
    let isValid = true;

    // Validate Cover Image
    if (!validateImage(document.getElementById("CoverImage"), "coverImageError")) {
        isValid = false;
    }
    // Validate Featured Image
    if (!validateImage(document.getElementById("FeaturedImage"), "featuredImageError")) {
        isValid = false;
    }
    // Validate Attribute Images (if they exist)
    document.querySelectorAll("[id^='attributeImage_']").forEach(input => {
        const id = input.id.split("_")[1];
        if (!validateImage(input, "attributeImageError_" + id)) {
            isValid = false;
        }
    });

    // Prevent form submission if validation fails
    if (!isValid) {
        console.log("Form submission prevented due to validation errors.");
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'يرجى تصحيح الأخطاء في النموذج.' : 'Please fix the errors in the form.',
            showConfirmButton: true
        });
        return;
    }

    // Create new FormData for reindexed data
    const formData = new FormData();

    // Copy non-ProductAttributeMappings fields from original FormData
    for (let [key, value] of originalFormData) {
        if (!key.startsWith("ProductAttributeMappings")) {
            formData.append(key, value);
        }
    }

    // Reindex ProductAttributeMappings
    const visibleCards = document.querySelectorAll("#selectedAttributesContainer .card:not(.d-none)");

    visibleCards.forEach((card, newIndex) => {
        const oldIndex = card.querySelector("input.attribute-index")?.value;
        const attrId = card.id.split("_")[1];

        // Add .Index field
        formData.append("ProductAttributeMappings.Index", newIndex);

        // Process all inputs in the card
        card.querySelectorAll("input, select, textarea").forEach(input => {
            const oldName = input.name;
            if (oldName && oldName.includes(`SelectedAttributes[${oldIndex}]`)) {
                // Reindex the name to match ProductAttributeMappingViewModel
                const newName = oldName.replace(`SelectedAttributes[${oldIndex}]`, `ProductAttributeMappings[${newIndex}]`);
                let value = originalFormData.get(oldName); // Get value from original FormData

                // Handle file inputs (e.g., attributeImage)
                if (input.type === "file" && input.files.length > 0) {
                    formData.append(newName, input.files[0]);
                } else if (value) {
                    formData.append(newName, value);
                }

            }
        });
    });

    // Ensure unchecked checkboxes send false (with reindexed names)
    document.querySelectorAll(".form-check-input[type='checkbox']").forEach(function (checkbox) {
        const oldName = checkbox.name;
        if (!checkbox.checked && oldName.includes("SelectedAttributes")) {
            // Find the card and its new index
            const card = checkbox.closest(".card:not(.d-none)");
            if (card) {
                const oldIndex = card.querySelector("input.attribute-index")?.value;
                const newIndex = Array.from(visibleCards).indexOf(card);
                const newName = oldName.replace(`SelectedAttributes[${oldIndex}]`, `ProductAttributeMappings[${newIndex}]`);
                formData.append(newName, "false");
            }
        }
    });

    // Remove duplicates by ensuring everything is a string
    selectedProducts = [...new Set(selectedProducts.map(String))];
    selectedProducts.forEach(productId => {
        formData.append("LinkedProductIds", productId);
    });

    // Append gallery images
    myDropzone.files.forEach((file) => {
        if (!removedFiles.includes(file.name)) {
            formData.append("GalleryImages", file);
        }
    });

    // Log reindexed FormData
    const formDataLog = [];
    for (let [key, value] of formData) {
        formDataLog.push(`${key}: ${value instanceof File ? value.name : value}`);
    }

    // Submit the form using Fetch API
    fetch(this.action, {
        method: "POST",
        body: formData,
    })
        .then((response) => {
            if (response.status === 210) {
                // Success Alert using SweetAlert2
                Swal.fire({
                    icon: 'success',
                    title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                    showConfirmButton: true
                })
            } else {
                // Error Alert using SweetAlert2
                Swal.fire({
                    icon: 'error',
                    title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء إرسال النموذج.' : 'An error occurred while submitting the form.',
                    showConfirmButton: true
                });
            }
        })
        .catch((error) => {
            console.error("Form submission error:", error);
            // Error Alert using SweetAlert2 for the catch block
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء إرسال النموذج.' : 'An error occurred while submitting the form.',
                showConfirmButton: true
            });
        });
});



