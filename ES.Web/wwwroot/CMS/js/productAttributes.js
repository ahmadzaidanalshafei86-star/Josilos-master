const baseUrl = window.appBaseUrl || '/';
$(document).ready(function () {

    var languageUrl = '';

    // Determine the language URL based on the current language
    if (currentLanguage === 'ar') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    } else if (currentLanguage === 'en') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    } else {
        // Default to English if the language is not set or recognized
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    }

    $('#attributesTable').DataTable({
        "pageLength": 5,
        "lengthMenu": [[5, 10, 15, 20], [5, 10, 15, 20]],
        fixedHeader: true,
        "ordering": false, // Disable sorting
        "language": {
            "url": languageUrl
        }
    });

    initializeEventHandlers();
    enableDragAndDrop();
   
});

// Initialize Event Handlers
function initializeEventHandlers() {
    $(document).on("click", ".addValueBtn", openAddValueModal);
    $("#addValueForm").submit(handleAddValue);
    $(document).on("click", ".delete-attribute-icon", handleDeleteAttribute);
    $(".edit-attribute-icon").click(loadAttributeForEdit);
    $(document).on("click", ".delete-value", handleDeleteValue);
    $(document).on("click", ".edit-value", enableInlineEditing); 
    $("#attributeForm").submit(submitAttributeForm);
    enableDragAndDrop();
    $(".saveOrderBtn").click(function () {
        updateValueOrder();
    });
    
}

// Localized messages based on currentLanguage (set in Razor View)
const messages = {
    addSuccess: {
        en: "Attribute added successfully!",
        ar: "تمت إضافة السمة بنجاح!"
    },
    addError: {
        en: "Failed to add attribute.",
        ar: "فشل في إضافة السمة."
    },
    valueSuccess: {
        en: "Value added successfully!",
        ar: "تمت إضافة القيمة بنجاح!"
    },
    valueError: {
        en: "Failed to add value.",
        ar: "فشل في إضافة القيمة."
    },
    fieldRequired: {
        en: "This field is required!",
        ar: "هذا الحقل مطلوب!"
    },
    reloadError: {
        en: "Failed to load attributes.",
        ar: "فشل في تحميل السمات."
    },
     confirmDelete: {
        en: "Are you sure?",
        ar: "هل أنت متأكد؟"
    },
    deleteWarning: {
        en: "This action cannot be undone!",
        ar: "لا يمكن التراجع عن هذا الإجراء!"
    },
    yesDelete: {
        en: "Yes, delete it!",
        ar: "نعم، احذفه!"
    },
    cancel: {
        en: "Cancel",
        ar: "إلغاء"
    },
    deleteSuccess: {
        en: "Attribute deleted successfully!",
        ar: "تم حذف السمة بنجاح!"
    },
    deleteError: {
        en: "Failed to delete attribute.",
        ar: "فشل في حذف السمة."
    },
    updateSuccess: {
        en: "Updated successfully!",
        ar: "تم التحديث بنجاح!"
    },
    updateError: {
        en: "Failed to update.",
        ar: "فشل في التحديث."
    },
    unauthorized: {
        en: {
            title: "Unauthorized",
            text: "You do not have permission to perform this action!"
        },
        ar: {
            title: "غير مصرح",
            text: "ليس لديك إذن لتنفيذ هذا الإجراء!"
        }
    }
};

// Handle Add/Edit Submission
function submitAttributeForm(event) {
    event.preventDefault();

    let id = $("#attributeId").val();
    let name = $("#attributeName").val().trim();

    if (name === "") {
        showAlert("warning", messages.fieldRequired[currentLanguage]);
        return;
    }

    let url = id > 0 ? `${baseUrl}EsAdmin/ProductAttributes/UpdateAttribute` : `${baseUrl}EsAdmin/ProductAttributes/AddAttribute`; // Determine action
    let successMessage = id > 0 ? messages.updateSuccess[currentLanguage] : messages.addSuccess[currentLanguage];

    $.ajax({
        url: url,
        type: "POST",
        data: { id: id, name: name },
        success: function () {
            showAlert("success", successMessage);

            if (id > 0) {
                $(`.attribute-name[data-id="${id}"]`).text(name); // Update name in table
                setTimeout(() => location.reload(), 1000);
            } else {
                setTimeout(() => location.reload(), 1000); // Reload to show new attribute
            }

            resetForm(); // Clear form after submission
        },
        error: function (jqXHR) {
            if (jqXHR.status === 403) {
                Swal.fire({
                    icon: "error",
                    title: messages.unauthorized[currentLanguage].title,
                    text: messages.unauthorized[currentLanguage].text,
                });
                setTimeout(() => location.reload(), 2000);
            } else {
                showAlert("error", messages.updateError[currentLanguage]);
            }
        }
    });
}


// Open Modal to Add Value to Attribute
function openAddValueModal() {
    let attributeId = $(this).data("id");
    let attributeName = $(this).data("name");

    $("#attributeTitle").text(attributeName);
    $("#attributeId").val(attributeId);
    $("#addValueModal").modal("show");
}

// Handle Adding Value to Attribute
function handleAddValue(event) {
    event.preventDefault();
    let attributeId = $("#attributeId").val();
    let value = $("#attributeValue").val().trim();
    let order = $("#valueOrder").val();

    if (value === "") {
        showAlert("error", messages.fieldRequired[currentLanguage]);
        return;
    }

    const AddValueUrl = `${baseUrl}EsAdmin/ProductAttributes/AddValue`;
    $.ajax({
        url: AddValueUrl,
        type: "POST",
        data: { attributeId: attributeId, value: value, order: order },
        success: function () {
            showAlert("success", messages.valueSuccess[currentLanguage]);
            $("#addValueModal").modal("hide");
            setTimeout(() => location.reload(), 1000);
        },
        error: function (jqXHR) {
            if (jqXHR.status === 403) {
                Swal.fire({
                    icon: "error",
                    title: messages.unauthorized[currentLanguage].title,
                    text: messages.unauthorized[currentLanguage].text,
                });
            } else {
                showAlert("error", messages.valueError[currentLanguage]);
            }
        }
    });
}

function handleDeleteAttribute() {
    let attributeId = $(this).data("id");
    let row = $(this).closest("tr");

    Swal.fire({
        title: messages.confirmDelete[currentLanguage],
        text: messages.deleteWarning[currentLanguage],
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: messages.yesDelete[currentLanguage],
        cancelButtonText: messages.cancel[currentLanguage]
    }).then((result) => {
        if (result.isConfirmed) {
            const DeleteAttrUrl = `${baseUrl}EsAdmin/ProductAttributes/Delete`;
            $.ajax({
                url: DeleteAttrUrl,
                type: "POST",
                data: { id: attributeId },
                success: function () {
                    showAlert("success", messages.deleteSuccess[currentLanguage]);
                    row.fadeOut(300, function () { $(this).remove(); });
                },
                error: function (xhr) {
                    if (xhr.status === 403) {
                        Swal.fire({
                            icon: "error",
                            title: messages.unauthorized[currentLanguage].title,
                            text: messages.unauthorized[currentLanguage].text,
                        });
                    } else {
                        showAlert("error", messages.deleteError[currentLanguage]);
                    }
                }
            });
        }
    });
}


function loadAttributeForEdit() {
    let attributeId = $(this).data("id");
    let attributeName = $(this).data("name");

    $("#attributeId").val(attributeId);
    $("#attributeName").val(attributeName);

    // Change text and icon for Edit Mode
    $("#AttributeFormHeader").text(currentLanguage === "ar" ? "تحديث السمة" : "Update Attribute");
    $("#submitText").text(currentLanguage === "ar" ? "تحديث السمة" : "Update Attribute");
    $("#submitIcon").removeClass("lni-plus").addClass("lni-save");

    // Change header background color to warning
    $(".form-header").removeClass("bg-success").addClass("bg-warning");

    // Scroll to the form if the container exists
    const formContainer = $("#AttributeFormHeader");
    if (formContainer.length) {
        $('html, body').animate({
            scrollTop: formContainer.offset().top - 100
        }, 500);
    }
}


function resetForm() {
    $("#attributeId").val(0);
    $("#attributeName").val("");

    // Reset button text and icon to Add Mode
    $("#AttributeFormHeader").text(currentLanguage === "ar" ? "إضافة سمة جديدة" : "Add New Attribute");
    $("#submitText").text(currentLanguage === "ar" ? "إضافة سمة" : "Add Attribute");
    $("#submitIcon").removeClass("lni-save").addClass("lni-plus");
    // Reset header background color to success
    $(".form-header").removeClass("bg-warning").addClass("bg-success");
   
}


// Edit Value Text and reordering 
// {

// Enable Drag & Drop on Badge Itself
function enableDragAndDrop() {
    $(".valuesContainer").each(function () {
        new Sortable(this, {
            animation: 150,
            handle: ".value-item",
            onEnd: function () {
                $(this.el).closest("tr").find(".saveOrderBtn").fadeIn();
                $(this.el).closest("tr").find(".saveOrderMessage").fadeIn();
            }
        });
    });
}

function updateValueOrder(container) {
    let orderData = [];

    // Collect values from .value-item elements
    $(container).find(".value-item").each(function (index) {
        orderData.push({ id: $(this).data("id"), order: index + 1 });
    });

    // Prevent sending request if there's no data
    if (orderData.length === 0) {
        Swal.fire("Error", messages.noItems[currentLanguage], "error");
        return; // Stop function execution
    }
    const updateValueOrderUrl = `${baseUrl}EsAdmin/ProductAttributes/UpdateValueOrder`;
    $.ajax({
        url: updateValueOrderUrl,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(orderData),
        success: function () {
            Swal.fire("Success", messages.updateSuccess[currentLanguage], "success");
            $(container).closest("tr").find(".saveOrderBtn").fadeOut();
            $(container).closest("tr").find(".saveOrderMessage").fadeOut();
        },
        error: function (xhr) {
            if (xhr.status === 403) {
                Swal.fire({
                    icon: "error",
                    title: messages.unauthorized[currentLanguage].title,
                    text: messages.unauthorized[currentLanguage].text,
                });
                setTimeout(() => location.reload(), 2000);
            } else {
                Swal.fire("Error", messages.orderError[currentLanguage], "error");
            }
        }
    });
}

$(document).on("click", ".saveOrderBtn", function () {
    let container = $(this).closest("tr").find(".valuesContainer");
    updateValueOrder(container);
});


//  Inline Editing
function enableInlineEditing(event) {
    event.preventDefault();
    let valueItem = $(this).closest(".value-item");
    let valueText = valueItem.find(".value-text");
    let currentText = valueText.text().trim();

    let input = $("<input>", {
        type: "text",
        class: "form-control form-control-sm",
        value: currentText
    });

    valueText.replaceWith(input);
    input.focus();

    input.on("blur", function () {
        let newText = $(this).val().trim();
        if (newText && newText !== currentText) {
            updateValueText(valueItem.data("id"), newText, $(this));
        } else {
            revertInputToText($(this), currentText);
        }
    }).on("keypress", function (e) {
        if (e.key === "Enter") $(this).blur();
    });
}

function updateValueText(valueId, newText, inputElement) {
    const updateValueTextUrl = `${baseUrl}EsAdmin/ProductAttributes/UpdateValueText`;
    $.post(updateValueTextUrl, { id: valueId, NewValueText: newText })
        .done(function () {
            revertInputToText(inputElement, newText);
        })
        .fail(function (jqXHR) {
            if (jqXHR.status === 403) {
                Swal.fire({
                    icon: "error",
                    title: messages.unauthorized[currentLanguage].title,
                    text: messages.unauthorized[currentLanguage].text,
                });
                setTimeout(() => location.reload(), 2000);
            } else {
                showAlert("error", messages.updateError[currentLanguage]);
            }
        });
}

function revertInputToText(inputElement, text) {
    let span = $("<span>", {
        class: "value-text flex-grow-1 px-2",
        "data-id": inputElement.data("id"),
        text: text
    });
    inputElement.replaceWith(span);
}

function handleDeleteValue(event) {
    event.preventDefault();
    let valueId = $(this).data("id");
    let valueItem = $(`.value-item[data-id="${valueId}"]`);

    Swal.fire({
        title: messages.confirmDelete[currentLanguage],
        text: messages.deleteWarning[currentLanguage],
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: messages.yesDelete[currentLanguage],
        cancelButtonText: messages.cancel[currentLanguage]
    }).then(result => {
        if (result.isConfirmed) {
            const DeleteValueUrl = `${baseUrl}EsAdmin/ProductAttributes/DeleteValue`;
            $.ajax({
                url: DeleteValueUrl,
                type: "POST",
                data: { id: valueId },
                success: function (response, textStatus, xhr) {
                    if (xhr.status === 200) {  // Check for HTTP status 200
                        valueItem.remove();
                        Swal.fire("Deleted!", messages.deleteSuccess[currentLanguage], "success");
                    } else {
                        Swal.fire("Error!", response.message || messages.deleteError[currentLanguage], "error");
                    }
                },
                error: function (xhr) {
                    if (xhr.status === 403) {
                        Swal.fire({
                            icon: "error",
                            title: messages.unauthorized[currentLanguage].title,
                            text: messages.unauthorized[currentLanguage].text,
                        });
                    } else {
                        Swal.fire("Error!", xhr.responseJSON?.message || messages.deleteError[currentLanguage], "error");
                    }
                }
            });
        }
    });
}


//}

function showAlert(type, message) {
    Swal.fire({
        icon: type,
        title: message,
        position: "center",  
        showConfirmButton: true, 
        timer: 2500, // Optional: Auto-close after 2.5 seconds
        customClass: currentLanguage === "ar" ? "swal-rtl" : ""
    });
}

