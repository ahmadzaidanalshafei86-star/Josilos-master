const baseUrl = window.appBaseUrl || '/';
$(document).ready(function () {
    // Initialize select2 for Category, Pages, and Icon dropdowns
    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#CategoryDropdown', 'حدد القسم');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#CategoryDropdown', 'Select the Category');
    }

    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#ProductCategoryDropdown', 'حدد القسم');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#ProductCategoryDropdown', 'Select the Category');
    }

    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#PagesDropdown', 'حدد الصفحة');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#PagesDropdown', 'Select the page');
    }


    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#IconDropdown', 'اختر ايقونة لعنصر القائمة');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#IconDropdown', 'Select menu item icon');
    }

    // Attach event handlers
    $('#Type').change(toggleSections);
    $('#saveMenuOrder').click(saveMenuOrder);
    $('#addMenuItemForm').submit(function (event) {
        event.preventDefault();
        if ($(this).data('mode') === 'edit') {
            updateMenuItem();
        } else {
            addMenuItem();
        }
        removeEmptyDiv();
    });

    $('#togglePublicationButton').click(togglePublicationStatus);
    $('#deleteMenuItem').click(deleteMenuItems);

    // Prevent dragging when clicking on the checkbox
    $('.menu-item-checkbox').on('mousedown', function (e) {
        e.stopPropagation();
    });

    // Prevent dragging when clicking on the Edit button
    $('.edit-menu-item-btn').on('mousedown', function (e) {
        e.stopPropagation();
    });

    // Prevent dragging when clicking on manage translations button
    $('.menu-item-translations-btn').on('mousedown', function (e) {
        e.stopPropagation();
    });

    // Initialize nestable for menu ordering
    $('#nestable').nestable();

    // Set the initial visibility based on the current Type value
    toggleSections();

    // Populate the icon dropdown
    populateIconDropdown();


    // Event handler for edit button click
    $(document).on('click', '.edit-menu-item-btn', function () {
        const menuItemId = $(this).data('id');
        const GetMenuItemUrl = `${baseUrl}EsAdmin/MenuManager/GetMenuItem/${menuItemId}`;
        $.ajax({
            url: GetMenuItemUrl,
            method: 'GET',
            success: function (data) {
                // Populate form fields
                $('#Title').val(data.title);
                $('#Type').val(data.type).trigger('change'); // Trigger change to show/hide sections
                $('#Target').val(data.target);
                $('#PublishCheckbox').prop('checked', data.isPublished);
                $('#IconDropdown').val(data.icon).trigger('change');

                // Reset dropdowns to avoid incorrect selections
                $('#CategoryDropdown').val('').trigger('change');
                $('#ProductCategoryDropdown').val('').trigger('change');
                $('#PagesDropdown').val('').trigger('change');
                $('#AnotherLink').val('');

                // Populate the correct dropdown based on type
                switch (data.type) {
                    case 'Category':
                        $('#CategoryDropdown').val(data.url).trigger('change');
                        break;
                    case 'ProductCategory':
                        $('#ProductCategoryDropdown').val(data.url).trigger('change');
                        break;
                    case 'Page':
                        $('#PagesDropdown').val(data.url).trigger('change');
                        break;
                    case 'CustomLink':
                        $('#AnotherLink').val(data.url);
                        break;
                    case 'HomePage':
                    case 'Careers':
                    case 'BlankLink':
                        // No dropdown to populate; URL is hardcoded
                        break;
                    default:
                        console.warn('Unknown menu item type:', data.type);
                        break;
                }

                // Update form header and button text based on language
                if (currentLanguage === 'ar-JO') {
                    $('.form-header h5').text("تعديل عنصر القائمة");
                    $('#addMenuItemForm button[type=submit]').text("تعديل عنصر القائمة");
                } else if (currentLanguage === 'en-US') {
                    $('.form-header h5').text("Update Menu Item");
                    $('#addMenuItemForm button[type=submit]').text("Update Menu Item");
                }

                // Animate form header
                $('.form-header').removeClass('bg-success').addClass('bg-warning').delay(1000).queue(function (next) {
                    $(this).removeClass('bg-warning').addClass('bg-success');
                    next();
                });

                // Set form mode to edit
                $('#addMenuItemForm').data('mode', 'edit');
                $('#addMenuItemForm').data('menuItemId', menuItemId);
            },
            error: function () {
                Swal.fire({
                    title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                    text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لتعديل عنصر قائمة !" : "You don't have permission to edit menu item!",
                    icon: "error"
                });
            }
        });
    });

});

// Initialize select2 with the specified selector and placeholder text
function initializeSelect2(selector, placeholderText) {
    $(selector).select2({
        theme: "bootstrap-5",
        placeholder: placeholderText,
        allowClear: true,
        minimumResultsForSearch: 1
    });
}

// Function to get selected menu item IDs
function getSelectedMenuItemIds() {
    return $('.menu-item-checkbox:checked')
        .map(function () {
            return parseInt($(this).closest('li.dd-item').data('id'));
        })
        .get();
}

// Populate Line icons dropdown
function populateIconDropdown() {
    const icons = [
        "lni lni-500px", "lni lni-world", "lni lni-list", "lni lni-adobe", "lni lni-agenda", "lni lni-airbnb", "lni lni-alarm",
        "lni lni-alarm-clock", "lni lni-amazon", "lni lni-amazon-original", "lni lni-amazon-pay",
        "lni lni-ambulance", "lni lni-amex", "lni lni-anchor", "lni lni-android", "lni lni-android-original",
        "lni lni-angellist", "lni lni-angle-double-down", "lni lni-angle-double-left", "lni lni-angle-double-right",
        "lni lni-angle-double-up", "lni lni-angular", "lni lni-apartment", "lni lni-cogs", "lni lni-coin",
        "lni lni-comments", "lni lni-comments-alt", "lni lni-comments-reply", "lni lni-compass", "lni lni-construction",
        "lni lni-construction-hammer", "lni lni-consulting", "lni lni-control-panel", "lni lni-cpanel", "lni lni-creative-commons",
        "lni lni-credit-cards", "lni lni-crop", "lni lni-cross-circle", "lni lni-crown", "lni lni-css3",
        "lni lni-cup", "lni lni-customer", "lni lni-cut", "lni lni-dashboard", "lni lni-database",
        "lni lni-delivery", "lni lni-dev", "lni lni-diamond", "lni lni-diamond-alt", "lni lni-diners-club",
        "lni lni-dinner", "lni lni-direction", "lni lni-direction-alt", "lni lni-direction-ltr", "lni lni-direction-rtl",
        "lni lni-discord", "lni lni-discover", "lni lni-display", "lni lni-display-alt", "lni lni-docker",
        "lni lni-dollar", "lni lni-domain", "lni lni-download", "lni lni-dribbble", "lni lni-drop",
        "lni lni-dropbox", "lni lni-dropbox-original", "lni lni-drupal", "lni lni-drupal-original", "lni lni-dumbbell",
        "lni lni-edge", "lni lni-emoji-cool", "lni lni-emoji-friendly", "lni lni-emoji-happy", "lni lni-emoji-sad",
        "lni lni-emoji-smile", "lni lni-emoji-speechless", "lni lni-emoji-suspect", "lni lni-emoji-tounge", "lni lni-empty-file",
        "lni lni-enter", "lni lni-envelope", "lni lni-eraser", "lni lni-euro", "lni lni-exit",
        "lni lni-exit-down", "lni lni-exit-up", "lni lni-eye", "lni lni-facebook", "lni lni-facebook-filled",
        "lni lni-facebook-messenger", "lni lni-facebook-original", "lni lni-facebook-oval", "lni lni-figma", "lni lni-files",
        "lni lni-firefox", "lni lni-firefox-original", "lni lni-fireworks", "lni lni-first-aid", "lni lni-flag",
        "lni lni-flag-alt", "lni lni-flags", "lni lni-flickr", "lni lni-flower", "lni lni-folder",
        "lni lni-forward", "lni lni-frame-expand", "lni lni-fresh-juice", "lni lni-full-screen", "lni lni-funnel",
        "lni lni-gallery", "lni lni-game", "lni lni-gift", "lni lni-git", "lni lni-github",
        "lni lni-github-original", "lni lni-goodreads", "lni lni-google", "lni lni-google-drive", "lni lni-google-pay",
        "lni lni-google-wallet", "lni lni-graduation", "lni lni-graph", "lni lni-grid", "lni lni-grid-alt",
        "lni lni-grow", "lni lni-hacker-news", "lni lni-hammer", "lni lni-hand", "lni lni-handshake",
        "lni lni-harddrive", "lni lni-headphone", "lni lni-headphone-alt", "lni lni-heart", "lni lni-heart-filled",
        "lni lni-heart-monitor", "lni lni-helicopter", "lni lni-helmet", "lni lni-help", "lni lni-highlight",
        "lni lni-highlight-alt", "lni lni-home", "lni lni-hospital", "lni lni-hourglass", "lni lni-html5",
        "lni lni-image", "lni lni-inbox", "lni lni-indent-decrease", "lni lni-indent-increase", "lni lni-infinite",
        "lni lni-information", "lni lni-instagram", "lni lni-instagram-filled", "lni lni-instagram-original", "lni lni-invention",
        "lni lni-invest-monitor", "lni lni-investment", "lni lni-island", "lni lni-italic", "lni lni-java",
        "lni lni-javascript", "lni lni-jcb", "lni lni-joomla"
    ];
    const $iconDropdown = $('#IconDropdown');
    icons.forEach(icon => {
        $iconDropdown.append(`<option value="${icon}">${icon}</option>`);
    });

    $iconDropdown.select2({
        templateResult: formatIcon,
        templateSelection: formatIcon
    });

    function formatIcon(icon) {
        if (!icon.id) return icon.text;
        return $(`<span><i class="${icon.id} me-2"></i>${icon.id}</span>`);
    }
}

// Toggle the visibility of sections based on the selected Type value
function toggleSections() {
    var type = $('#Type').val();
    $('#category-section').hide();
    $('#product-category-section').hide();
    $('#page-section').hide();
    $('#custom-url-section').hide();

    switch (type) {
        case 'Category':
            $('#category-section').show();
            break;
        case 'ProductCategory':
            $('#product-category-section').show();
            break;
        case 'Page':
            $('#page-section').show();
            break;
        case 'CustomLink':
            $('#custom-url-section').show();
            break;
        default:
            // No section needed for HomePage, Careers, BlankLink
            break;
    }
}

// Save the menu order by sending an AJAX request
function saveMenuOrder() {
    var rawOrder = $('#nestable').nestable('serialize');
    var formattedOrder = [];

    function processItems(items, parentId = null) {
        items.forEach((item, index) => {
            formattedOrder.push({
                id: item.id,
                parentId: parentId,
                order: index
            });
            if (item.children) {
                processItems(item.children, item.id);
            }
        });
    }

    processItems(rawOrder);
    const SaveOrderUrl = `${baseUrl}EsAdmin/MenuManager/SaveOrder`;
    $.ajax({
        url: SaveOrderUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formattedOrder),
        success: function () {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم حفظ ترتيب القائمة بنجاح!" : "Menu order saved successfully!",
                icon: "success"
            });
        },
        error: function (xhr) {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ليس لديك إذن للتعديل علي ترتيب القائمة !" : "You don't have permission to reorder the menu items !",
                icon: "error"
            });

            setTimeout(function () {
                location.reload();
            }, 3000);
        }
    });
}

// Add a new menu item via AJAX
function addMenuItem() {
    var formData = {
        Title: $('#Title').val(),
        Target: $('#Target').val(),
        Type: $('#Type').val(),
        Icon: $('#IconDropdown').val(),
        IsPublished: $('#PublishCheckbox').is(':checked')
    };
    switch (formData.Type) {
        case 'Category':
            formData.Url = $('#CategoryDropdown').val(); // Assumes dropdown value is the category slug
            break;
        case 'ProductCategory':
            formData.Url = $('#ProductCategoryDropdown').val(); // Assumes dropdown value is the e-commerce category slug
            break;
        case 'Page':
            formData.Url = $('#PagesDropdown').val(); // Assumes dropdown value is the page slug
            break;
        case 'CustomLink':
            formData.Url = $('#AnotherLink').val(); // Custom URL input
            break;
        case 'HomePage':
            formData.Url = baseUrl; // Hardcoded homepage URL
            break;
        case 'Careers':
            formData.Url = `${baseUrl}Careers`; // Hardcoded careers path
            break;
        case 'BlankLink':
            formData.Url = '/#'; // Hardcoded blank link
            break;
        default:
            formData.Url = null; // Fallback for unexpected types
            break;
    }


    const AddUrl = `${baseUrl}EsAdmin/MenuManager/AddMenuItem`;
    $.ajax({
        url: AddUrl,
        type: 'POST',
        data: formData,
        success: function (newMenuItem) {
            var statusBadge = newMenuItem.isPublished
                ? '<span class="badge bg-success ms-2">Published</span>'
                : '<span class="badge bg-danger ms-2">Not published</span>';

            var listItemHtml = `
        <li class="dd-item" data-id="${newMenuItem.id}">
            <div class="dd-handle">
                <i class="${newMenuItem.iconClass}"></i> ${newMenuItem.title} (${newMenuItem.type})
                ${statusBadge}
            </div>
            <ol class="dd-list"></ol>
        </li>`;

            var $nestableList = $('#nestable > .dd-list');
            if ($nestableList.length === 0) {
                $('#nestable').append('<ol class="dd-list"></ol>');
                $nestableList = $('#nestable > .dd-list');
            }
            $nestableList.append(listItemHtml);
            removeEmptyDiv();

            $('#nestable').nestable('destroy').nestable();

            $('#addMenuItemForm')[0].reset();

            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم إضافة عنصر القائمة بنجاح!" : "Menu item added successfully!",
                icon: "success"
            });

            setTimeout(function () {
                location.reload();
            }, 2000);
        }
        ,
        error: function (jqXHR) {
            if (jqXHR.status === 403) {
                Swal.fire({
                    title: currentLanguage === 'ar-JO' ? "ليس لديك اذن لاضافة عنصر القائمة !" : "You don't have permission to add menu item!",
                    icon: "error"
                });
            }
        }
    });
}

// Update an existing menu item via AJAX
function updateMenuItem() {
    const menuItemId = $('#addMenuItemForm').data('menuItemId');
    var formData = {
        Title: $('#Title').val(),
        Target: $('#Target').val(),
        Type: $('#Type').val(),
        Icon: $('#IconDropdown').val(),
        IsPublished: $('#PublishCheckbox').is(':checked')
    };

    switch (formData.Type) {
        case 'Category':
            formData.Url = $('#CategoryDropdown').val(); // Assumes dropdown value is the category slug
            break;
        case 'ProductCategory':
            formData.Url = $('#ProductCategoryDropdown').val(); // Assumes dropdown value is the e-commerce category slug
            break;
        case 'Page':
            formData.Url = $('#PagesDropdown').val(); // Assumes dropdown value is the page slug
            break;
        case 'CustomLink':
            formData.Url = $('#AnotherLink').val(); // Custom URL input
            break;
        case 'HomePage':
            formData.Url = baseUrl;
            break;
        case 'Careers':
            formData.Url = `${baseUrl}Careers`; // Hardcoded careers path
            break;
        case 'BlankLink':
            formData.Url = '/#'; // Hardcoded blank link
            break;
        default:
            formData.Url = null; // Fallback for unexpected types
            break;
    }
    const UpdateUrl = `${baseUrl}EsAdmin/MenuManager/UpdateMenuItem/${menuItemId}`;
    $.ajax({
        url: UpdateUrl,
        type: 'POST',
        data: formData,
        success: function () {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم تعديل عنصر القائمة بنجاح !" : "Menu item updated successfully!",
                icon: "success"
            });

            setTimeout(function () {
                location.reload();
            }, 3000);
        },
        error: function () {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ اثناء تعديل عنصر القائمة !" : "Error updating menu item!",
                icon: "error"
            });
        }
    });

}

// Function to remove dd-empty div
function removeEmptyDiv() {
    $('.dd-empty').remove();
}

// Function to toggle publication status for selected menu items
function togglePublicationStatus() {
    var selectedIds = getSelectedMenuItemIds();

    if (selectedIds.length === 0) {
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "يرجى اختيار عنصر واحد على الأقل من القائمة لتبديل الحالة." : "Please select at least one menu item to toggle the status.",
            icon: "warning"
        });
        return;
    }
    const ToggoleUrl = `${baseUrl}EsAdmin/MenuManager/TogglePublicationStatus`;
    $.ajax({
        url: ToggoleUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(selectedIds),
        success: function () {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم تبديل حالة نشر عنصر القائمة بنجاح !" : "Publication status toggled successfully!",
                icon: "success"
            });

            setTimeout(function () {
                location.reload();
            }, 3000);
            $('.menu-item-checkbox:checked').prop('checked', false);
        },
        error: function () {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ليس لديك اذن لتبديل حالة عنصر القائمة !" : "You don't have permission to toggle publication status.",
                icon: "error"
            });
        }
    });
}

// Function to delete selected menu items
function deleteMenuItems() {
    var selectedIds = getSelectedMenuItemIds();

    if (selectedIds.length === 0) {
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "يرجى اختيار عنصر واحد على الأقل من القائمة للحذف !" : "Please select at least one menu item to delete.",
            icon: "warning"
        });
        return;
    }

    Swal.fire({
        title: currentLanguage === 'ar-JO' ? "هل أنت متأكد أنك تريد حذف عناصر القائمة المحددة؟" : "Are you sure you want to delete the selected menu items?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? "نعم" : "Yes",
        cancelButtonText: currentLanguage === 'ar-JO' ? "لا" : "No"
    }).then((result) => {
        if (result.isConfirmed) {
            const DeleteUrl = `${baseUrl}EsAdmin/MenuManager/DeleteMenuItems`;
            $.ajax({
                url: DeleteUrl,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(selectedIds),
                success: function () {
                    Swal.fire({
                        title: currentLanguage === 'ar-JO' ? "تم حذف عنصر القائمة بنجاح !" : "Selected menu items deleted successfully!",
                        icon: "success"
                    });

                    setTimeout(function () {
                        location.reload();
                    }, 3000);

                    $('.menu-item-checkbox:checked').prop('checked', false);
                },
                error: function () {
                    Swal.fire({
                        title: currentLanguage === 'ar-JO' ? "ليس لديك إذن لحذف عنصر القائمة !" : "You don't have permission to delete this menu item!",
                        icon: "error"
                    });
                }
            });
        }
    });
}

