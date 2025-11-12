document.addEventListener("DOMContentLoaded", function () {
    // Initialize Select2 Dropdowns for both sections
    initializeSelect2('#categorySelect', 'Select the category');
    initializeSelect2('#categorySelectPages', 'Select the pages category');

    // Initialize "Select All" Checkboxes for both sections
    initializeSelectAllCheckboxes();

    // Attach event listeners for category selection logic
    document.getElementById("canCreate")?.addEventListener("change", toggleCategorySelection);
    document.getElementById("canRead")?.addEventListener("change", toggleCategorySelection);
    document.getElementById("canUpdate")?.addEventListener("change", toggleCategorySelection);
    document.getElementById("canDelete")?.addEventListener("change", toggleCategorySelection);

    // Attach event listeners for pages selection logic
    document.getElementById("canCreatePage")?.addEventListener("change", togglePagesSelection);
    document.getElementById("canReadPage")?.addEventListener("change", togglePagesSelection);
    document.getElementById("canUpdatePage")?.addEventListener("change", togglePagesSelection);
    document.getElementById("canDeletePage")?.addEventListener("change", togglePagesSelection);

    // Attach event listeners for individual checkboxes to update "Select All" for both sections
    document.querySelectorAll('[class^="roleClaimSection"]').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const startIndex = parseInt(this.id.split('_')[1]);
            const sectionIndex = Math.floor(startIndex / 4) * 4;
            updateMasterCheckbox(sectionIndex, sectionIndex + 4);
        });
    });

    // Run category selection logic on page load
    toggleCategorySelection();
    togglePagesSelection();
});

function filterPermissions() {
    const searchInput = document.getElementById('searchPermissions')?.value.toLowerCase();
    const sections = document.querySelectorAll('.permission-section');

    sections.forEach(section => {
        const title = section.querySelector('.permission-title')?.textContent.toLowerCase();
        section.style.display = title.includes(searchInput) ? '' : 'none';
    });
}

// Function to toggle individual section checkboxes based on the "Select All" checkbox
function toggleSection(startIndex, endIndex, masterCheckbox) {
    const sectionIndex = Math.floor(startIndex / 4) + 1;
    const checkboxes = document.querySelectorAll(`.roleClaimSection${sectionIndex}`);

    checkboxes.forEach(checkbox => {
        checkbox.checked = masterCheckbox.checked;
    });

    updateMasterCheckbox(startIndex, endIndex);
}

// Function to check if all checkboxes in the section are selected and update the "Select All" checkbox
function updateMasterCheckbox(startIndex, endIndex) {
    const sectionIndex = Math.floor(startIndex / 4) + 1;
    const checkboxes = document.querySelectorAll(`.roleClaimSection${sectionIndex}`);
    const masterCheckbox = document.getElementById(`selectAllSection${sectionIndex}`);

    if (!masterCheckbox) return;

    masterCheckbox.checked = Array.from(checkboxes).every(checkbox => checkbox.checked);
}

// Function to check the initial state of the "Select All" checkboxes when the page loads
function initializeSelectAllCheckboxes() {
    const sectionCount = 15; // Number of sections
    const claimsPerSection = 4; // Number of claims per section

    for (let sectionIndex = 0; sectionIndex < sectionCount; sectionIndex++) {
        const startIndex = sectionIndex * claimsPerSection;
        const endIndex = startIndex + claimsPerSection;
        updateMasterCheckbox(startIndex, endIndex);
    }
}

// Function to enable/disable category selection and "Select All" button based on permissions
function toggleCategorySelection() {
    let canUpdate = document.getElementById("canUpdate")?.checked ?? false;
    let canDelete = document.getElementById("canDelete")?.checked ?? false;
    let categorySelect = document.getElementById("categorySelect");
    let selectAllButton = document.querySelector('.select-all-button');

    if (!categorySelect || !selectAllButton) return;

    // Enable/disable the dropdown and "Select All" button based on permissions
    const shouldEnable = canUpdate || canDelete;
    categorySelect.disabled = !shouldEnable;
    selectAllButton.disabled = !shouldEnable;

    // Clear dropdown selection if both "Update" and "Delete" are unchecked
    if (!shouldEnable) {
        $(categorySelect).val(null).trigger('change'); // Clear Select2 selection
    }
}

// Function to enable/disable pages selection and "Select All" button based on permissions
function togglePagesSelection() {
    let canUpdatePage = document.getElementById("canUpdatePage")?.checked ?? false;
    let canDeletePage = document.getElementById("canDeletePage")?.checked ?? false;
    let categorySelectPages = document.getElementById("categorySelectPages");
    let selectAllButtonPages = document.querySelector('.select-all-button-pages');

    if (!categorySelectPages || !selectAllButtonPages) return;

    // Enable/disable the dropdown and "Select All" button based on permissions
    const shouldEnablePage = canUpdatePage || canDeletePage;
    categorySelectPages.disabled = !shouldEnablePage;
    selectAllButtonPages.disabled = !shouldEnablePage;

    // Clear dropdown selection if both "Update" and "Delete" are unchecked
    if (!shouldEnablePage) {
        $(categorySelectPages).val(null).trigger('change'); // Clear Select2 selection
    }
}

// Function to initialize Select2 dropdown with a "Select All" button
function initializeSelect2(selector, placeholderText) {
    if (typeof $ !== "undefined" && $.fn.select2) {
        // Initialize Select2
        $(selector).select2({
            theme: "bootstrap-5",
            placeholder: placeholderText,
            minimumResultsForSearch: 0 // Enables search for all dropdowns
        });

        // Check if the "Select All" button already exists to avoid duplication
        if ($(selector).next('.select2-container').next('.select-all-button').length === 0 && $(selector).attr('id') === 'categorySelect') {
            // Create a "Select All" button for categories
            var selectAllButton = $('<button>', {
                type: 'button',
                text: 'Select All',
                class: 'btn btn-danger btn-sm select-all-button',
                style: 'margin-top: 10px;',
                disabled: true // Initially disabled
            });

            // Insert the button after the Select2 container
            $(selector).next('.select2-container').after(selectAllButton);

            // Add click event to the "Select All" button
            selectAllButton.on('click', function () {
                var options = $(selector).find('option');
                options.prop('selected', true); // Select all options
                $(selector).trigger('change'); // Trigger change event to update Select2
            });
        }

        // Check if the "Select All" button already exists to avoid duplication
        if ($(selector).next('.select2-container').next('.select-all-button-pages').length === 0 && $(selector).attr('id') === 'categorySelectPages') {
            // Create a "Select All" button for pages
            var selectAllButtonPages = $('<button>', {
                type: 'button',
                text: 'Select All',
                class: 'btn btn-danger btn-sm select-all-button-pages',
                style: 'margin-top: 10px;',
                disabled: true // Initially disabled
            });

            // Insert the button after the Select2 container
            $(selector).next('.select2-container').after(selectAllButtonPages);

            // Add click event to the "Select All" button
            selectAllButtonPages.on('click', function () {
                var options = $(selector).find('option');
                options.prop('selected', true); // Select all options
                $(selector).trigger('change'); // Trigger change event to update Select2
            });
        }

        // Add event listeners to the "Update" and "Delete" checkboxes
        $('#canUpdate, #canDelete, #canUpdatePage, #canDeletePage').on('change', function () {
            toggleCategorySelection();
            togglePagesSelection();
        });

        // Initialize the state of the dropdown and button
        toggleCategorySelection();
        togglePagesSelection();
    } else {
        console.warn("Select2 is not loaded. Ensure jQuery and Select2 are included.");
    }
}