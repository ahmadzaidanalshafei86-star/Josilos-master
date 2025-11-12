const baseUrl = window.appBaseUrl || '/';

document.addEventListener("DOMContentLoaded", function () {
    initializeEventListeners();

    setTimeout(() => { // Small delay to ensure DOM is ready
        document.querySelectorAll(".fieldType").forEach((selectElement, index) => {
            handleFieldTypeChange(index, selectElement);
        });
    }, 50);

    const formId = document.getElementById("formId").value;

    if (formId > 0) {
        loadFormDataFromModel();
    }


    $("#formBuilder").on("submit", function (event) {
        event.preventDefault(); // Prevent default form submission
    });

});


function initializeEventListeners() {
    document.getElementById("saveFormBtn").addEventListener("click", saveForm);
    document.getElementById("addFieldBtn").addEventListener("click", addField);
    document.getElementById("previewFormBtn").addEventListener("click", previewForm);
}

// load data in edit
function loadFormDataFromModel() {
    const formFields = JSON.parse(document.getElementById("formFieldsData").value);

    if (formFields && formFields.length > 0) {
        formFields.forEach((field, index) => {
            addField(field, index); // Pass the entire field object to retain FieldId
        });
    }
}


// 🔹 Add a new field dynamically
function addField(fieldData = null) {
    const fieldIndex = document.querySelectorAll(".field-container").length;
    const fieldId = fieldData?.id ?? 0; // ✅ Ensure FieldId is included
    const fieldContainer = document.createElement("div");
    fieldContainer.classList.add("field-container", "card", "p-3", "shadow-sm", "border-0", "rounded-lg", "position-relative", "mb-3", "transition-all");
    fieldContainer.setAttribute("data-index", fieldIndex);

    // Default values if fieldData is null (new field)
    const fieldHint = fieldData?.fieldHint ?? "";
    const fieldType = fieldData ? fieldData.fieldType : "text";
    const isRequired = fieldData ? fieldData.isRequired : false;
    const isPublished = fieldData ? fieldData.isPublished : true;

    // Get localized text dynamically based on current language
    const enterQuestion = currentLanguage.startsWith("ar") ? "أدخل سؤالك" : "Enter your question";
    const enterHint = currentLanguage.startsWith("ar") ? "أدخل تلميحًا (اختياريًا)" : "Enter a hint (optional)";
    const shortAnswer = currentLanguage.startsWith("ar") ? "إجابة قصيرة" : "Short Answer";
    const paragraph = currentLanguage.startsWith("ar") ? "فقرة" : "Paragraph";
    const phoneNumber = currentLanguage.startsWith("ar") ? "رقم الهاتف" : "Phone Number";
    const emailAddress = currentLanguage.startsWith("ar") ? "البريد الإلكتروني" : "Email Address";
    const multipleChoice = currentLanguage.startsWith("ar") ? "اختيار متعدد" : "Multiple Choice";
    const checkboxes = currentLanguage.startsWith("ar") ? "خانات الاختيار" : "Checkboxes";
    const fileUpload = currentLanguage.startsWith("ar") ? "تحميل ملف" : "File Upload";
    const rating = currentLanguage.startsWith("ar") ? "تقييم" : "Rating";
    const datePicker = currentLanguage.startsWith("ar") ? "التقويم" : "Date Picker";
    const required = currentLanguage.startsWith("ar") ? "إجباري" : "Required";
    const publish = currentLanguage.startsWith("ar") ? "نشر" : "Publish";
    const addOptionLabel = currentLanguage.startsWith("ar") ? "إضافة خيار" : "Add Option";

    fieldContainer.innerHTML = `
    <div class="d-flex align-items-center">
        <div class="drag-handle cursor-grab me-3">
            <i class="lni lni-menu text-muted fs-5"></i>
        </div>
        <div class="flex-grow-1">
            <input type="hidden" name="Fields[${fieldIndex}].Id" value="${fieldId}" />

            <input type="text" class="form-control border-0 shadow-none fw-bold mb-2 bg-light rounded-pill px-3 py-2"
                   name="Fields[${fieldIndex}].FieldName" placeholder="${enterQuestion}" value="${fieldData ? fieldData.fieldName : ''}" required />

            <!-- Hint Textarea -->
            <textarea class="form-control border-0 shadow-none bg-light rounded px-3 py-2 mb-2"
                      name="Fields[${fieldIndex}].FieldHint" placeholder="${enterHint}">${fieldHint}</textarea>

            <select class="form-select fieldType mb-2 bg-light rounded-pill px-3 py-2" 
                    name="Fields[${fieldIndex}].FieldType" onchange="handleFieldTypeChange(${fieldIndex}, this)">
                <option value="text" ${fieldType === "text" ? "selected" : ""}>${shortAnswer}</option>
                <option value="textarea" ${fieldType === "textarea" ? "selected" : ""}>${paragraph}</option>
                <option value="phoneNumber" ${fieldType === "phoneNumber" ? "selected" : ""}>${phoneNumber}</option>
                <option value="email" ${fieldType === "email" ? "selected" : ""}>${emailAddress}</option>
                <option value="select" ${fieldType === "select" ? "selected" : ""}>${multipleChoice}</option>
                <option value="checkbox" ${fieldType === "checkbox" ? "selected" : ""}>${checkboxes}</option>
                <option value="file" ${fieldType === "file" ? "selected" : ""}>${fileUpload}</option>
                <option value="rating" ${fieldType === "rating" ? "selected" : ""}>${rating}</option>
                <option value="date" ${fieldType === "date" ? "selected" : ""}>${datePicker}</option>
            </select>

            <div class="optionsContainer mt-2 d-none" id="optionsContainer-${fieldIndex}">
                <div class="option-list mb-2"></div>
                <button type="button" class="btn btn-sm btn-outline-primary rounded-pill addOptionBtn"
                        onclick="addOption(${fieldIndex})">
                    <i class="lni lni-plus me-1"></i>${addOptionLabel}
                </button>
            </div>

            <div class="d-flex justify-content-between align-items-center mt-3">
                <div></div>
                <div class="form-check form-switch">
                    <input class="form-check-input isRequiredSwitch" type="checkbox" 
                           name="Fields[${fieldIndex}].IsRequired" value="true" ${isRequired ? "checked" : ""}>
                    <label class="form-check-label text-muted">${required}</label>
                </div>
                <div class="form-check form-switch">
                    <input class="form-check-input PublishSwitch" type="checkbox"
                      name="Fields[${fieldIndex}].IsPublished" value="true" ${isPublished ? "checked" : ""}>
                    <label class="form-check-label text-muted">${publish}</label>
                </div>
            </div>

            <input type="hidden" name="Fields[${fieldIndex}].Order" class="field-order" value="${fieldIndex}" />
        </div>

        <button type="button" class="btn btn-outline-danger btn-sm removeFieldBtn ms-3 rounded-circle"
                onclick="removeField(${fieldIndex})">
            <i class="lni lni-trash"></i>
        </button>
    </div>
    `;

    document.getElementById("fieldsContainer").appendChild(fieldContainer);

    // ✅ Populate options if available
    if (fieldData?.options?.length) {
        fieldData.options.forEach((option, optionIndex) => {
            addOption(fieldIndex, option.optionText, optionIndex, option.id); // ✅ Pass OptionId
        });
        document.getElementById(`optionsContainer-${fieldIndex}`).classList.remove("d-none");
    }

    setTimeout(() => {
        fieldContainer.style.opacity = "1";
        fieldContainer.style.transform = "translateY(0)";
    }, 10);

    // Add event listener for removing the field
    fieldContainer.querySelector(".removeFieldBtn").addEventListener("click", function () {
        fieldContainer.remove();
        updateFieldOrder();
    });

    makeFieldsSortable();
}

function addOption(fieldIndex, optionText = "", optionIndex = null, optionId = 0) {

    const optionsContainer = document.getElementById(`optionsContainer-${fieldIndex}`).querySelector(".option-list");

    const optionCount = optionsContainer.querySelectorAll(".option-input").length;
    optionIndex = optionIndex !== null ? optionIndex : optionCount;

    const optionWrapper = document.createElement("div");
    optionWrapper.classList.add("d-flex", "align-items-center", "mb-2");

    optionWrapper.innerHTML = `
        <!-- ✅ Ensure correct OptionId binding -->
        <input type="hidden" name="Fields[${fieldIndex}].Options[${optionIndex}].Id" value="${optionId}" />
        
        <!-- ✅ OptionText input -->
        <input type="text" class="form-control option-input flex-grow-1" 
               name="Fields[${fieldIndex}].Options[${optionIndex}].OptionText" 
               value="${optionText}" placeholder="Enter option" required />
        
        <!-- Delete button -->
        <button type="button" class="btn btn-sm btn-outline-danger ms-2 removeOptionBtn"
                onclick="this.parentElement.remove()">
            <i class="lni lni-trash"></i>
        </button>
    `;

    optionsContainer.appendChild(optionWrapper);
}


// 🔹 Save the form data
function saveForm() {
    const fields = document.querySelectorAll(".field-container");

    // 🔹 Check if there are no fields
    if (fields.length === 0) {
        Swal.fire({
            icon: "warning",
            title: "No Fields Added!",
            text: "Please add at least one question before saving the form."
        });
        return;
    }

    let isValid = true; // Flag to track validation status

    // Validate Email Field 
    const emailInput = document.getElementById("formEmail");

    if (emailInput) {

        const emailValue = emailInput.value.trim();
        const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/; // email regex validation

        if (emailValue && !emailRegex.test(emailValue)) {
            isValid = false;
        }
    }

    // 🔹 Validate Field Titles
    fields.forEach(field => {
        const fieldNameInput = field.querySelector("[name*='FieldName']");
        const errorContainer = fieldNameInput.nextElementSibling; // Get error message container

        if (!fieldNameInput.value.trim()) {
            isValid = false;
            if (!errorContainer || !errorContainer.classList.contains("error-message")) {
                const errorMessage = document.createElement("div");
                errorMessage.classList.add("text-danger", "error-message");
                errorMessage.innerText = "Field title is required.";
                fieldNameInput.after(errorMessage);
            }
        } else {
            if (errorContainer && errorContainer.classList.contains("error-message")) {
                errorContainer.remove();
            }
        }
    });

    if (!isValid) return; // Stop form submission if validation fails

    // Proceed with form submission
    const formData = getFormData();

    // Determine if it's a Create or Edit operation

    const url = formData.Id > 0 ? `${baseUrl}EsAdmin/Forms/Edit` : `${baseUrl}EsAdmin/Forms/Create`; // Decide API endpoint


    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    icon: "success",
                    title: "Form Saved !",
                    text: "Your form has been successfully saved."
                });

            } else {
                Swal.fire({
                    icon: "error",
                    title: "Error!",
                    text: "Something went wrong while saving the form."
                });
            }
        })
        .catch(error => {
            console.error("Error:", error);
            Swal.fire({
                icon: "error",
                title: "Unexpected Error!",
                text: "An error occurred while saving the form."
            });
        });
}


// 🔹 Get form data from UI
function getFormData() {
    return {
        Id: document.getElementById("formId") ? parseInt(document.getElementById("formId").value) || 0 : 0,
        Title: document.getElementById("formTitle") ? document.getElementById("formTitle").value.trim() : "",
        Email: document.getElementById("formEmail") ?
            (document.getElementById("formEmail").value.trim() === "" ? null : document.getElementById("formEmail").value.trim())
            : null,
        Description: document.getElementById("formDescription") ? document.getElementById("formDescription").value.trim() : "",
        IsActive: document.getElementById("isActive") ? document.getElementById("isActive").checked : false,
        Fields: Array.from(document.querySelectorAll(".field-container")).map((field, index) => ({
            Id: field.querySelector("input[name^='Fields'][name$='.Id']") ?
                parseInt(field.querySelector("input[name^='Fields'][name$='.Id']").value) || null : null, // ✅ Retrieve FieldId safely
            FieldName: field.querySelector("input[name^='Fields'][name$='.FieldName']") ?
                field.querySelector("input[name^='Fields'][name$='.FieldName']").value.trim() : "",
            FieldHint: field.querySelector("textarea[name^='Fields'][name$='.FieldHint']") ?
                field.querySelector("textarea[name^='Fields'][name$='.FieldHint']").value.trim() : "",
            FieldType: field.querySelector("select[name^='Fields'][name$='.FieldType']") ?
                field.querySelector("select[name^='Fields'][name$='.FieldType']").value : "text",
            IsRequired: field.querySelector(".isRequiredSwitch") ?
                field.querySelector(".isRequiredSwitch").checked : false,
            IsPublished: field.querySelector(".PublishSwitch") ?
                field.querySelector(".PublishSwitch").checked : false,
            Order: index,
            Options: getFieldOptions(field) // ✅ Use the fixed function below
        }))
    };
}


// 🔹 Get all form fields
function getFormFields() {
    let fields = [];

    document.querySelectorAll(".field-container").forEach((field, index) => {
        fields.push({
            fieldName: field.querySelector("[name*='FieldName']").value,
            fieldHint: field.querySelector("[name*='FieldHint']") ?
                field.querySelector("[name*='FieldHint']").value.trim() : "",
            fieldType: field.querySelector(".fieldType").value,
            isRequired: field.querySelector(".isRequiredSwitch").checked,
            isPublished: field.querySelector(".PublishSwitch").checked,
            order: index, // Use index as order
            options: getFieldOptions(field)
        });
    });

    return fields;
}

// 🔹 Get options for a specific field
function getFieldOptions(field) {
    let options = [];
    const optionInputs = field.querySelectorAll(".option-list input.option-input");

    if (optionInputs.length > 0) {
        optionInputs.forEach((opt, index) => {
            // ✅ Get the hidden input for OptionId
            const idInput = opt.closest("div").querySelector(
                "input[name^='Fields'][name$='.Options[" + index + "].Id']"
            );

            options.push({
                Id: idInput && idInput.value ? parseInt(idInput.value) || null : null, // ✅ Retrieve OptionId safely
                OptionText: opt.value.trim(), // ✅ Ensure optionText is not undefined
                Order: index
            });
        });
    }

    return options;
}


// 🔹 Make fields sortable using Drag & Drop
function makeFieldsSortable() {
    new Sortable(document.getElementById("fieldsContainer"), {
        animation: 150,
        handle: ".drag-handle",
        onEnd: function () {
            updateFieldOrder();
        }
    });
}

// 🔹 Handle changes in field type 
function handleFieldTypeChange(fieldIndex, selectElement) {
    const fieldContainer = document.querySelector(`[data-index="${fieldIndex}"]`);
    const optionsContainer = document.getElementById(`optionsContainer-${fieldIndex}`);
    let inputContainer = fieldContainer.querySelector(".dynamic-input-container");

    // If the container doesn't exist, create it
    if (!inputContainer) {
        inputContainer = document.createElement("div");
        inputContainer.classList.add("dynamic-input-container", "mt-2");
        fieldContainer.querySelector(".flex-grow-1").appendChild(inputContainer);
    }

    // Clear previous content before adding new elements
    inputContainer.innerHTML = "";

    // Show options container for checkbox/select, hide for others
    if (selectElement.value === "checkbox" || selectElement.value === "select") {
        optionsContainer.classList.remove("d-none");
    } else {
        optionsContainer.classList.add("d-none");
    }

    // Handle dynamic input rendering for date, file, and rating fields
    switch (selectElement.value) {
        case "date":
            inputContainer.innerHTML = `<input type="date" class="form-control" placeholder="Select a date" />`;
            break;

        case "file":
            inputContainer.innerHTML = `<input type="file" class="form-control" />`;
            break;

        case "rating":
            inputContainer.innerHTML = `
                <div class="rating-container d-flex gap-1" onclick="selectRating(event, ${fieldIndex})">
                    <span class="rating-star" data-value="1">⭐</span>
                    <span class="rating-star" data-value="2">⭐</span>
                    <span class="rating-star" data-value="3">⭐</span>
                    <span class="rating-star" data-value="4">⭐</span>
                    <span class="rating-star" data-value="5">⭐</span>
                    <input type="hidden" name="Fields[${fieldIndex}].Rating" class="rating-value" value="0" />
                </div>`;
            break;

        default:
            inputContainer.innerHTML = ""; // Remove dynamic field if not applicable
            break;
    }
}

// 🔹 Update field order after sorting
function updateFieldOrder() {
    document.querySelectorAll(".field-container").forEach((field, index) => {
        field.setAttribute("data-index", index);
        field.querySelector(".field-order").value = index;
    });
}

// Function to update order when options are removed
function updateOptionOrder(fieldIndex) {
    const options = document.querySelectorAll(`#optionsContainer-${fieldIndex} .option-item`);
    options.forEach((option, index) => {
        option.querySelector(".optionOrder").value = index;
    });
}

function previewForm() {
    const formTitle = document.getElementById("formTitle").value;
    const formDescription = document.getElementById("formDescription").value;
    const formFields = getFormFields();

    // Set modal content
    document.getElementById("previewFormTitle").textContent = formTitle || "Untitled Form";
    document.getElementById("previewFormDescription").textContent = formDescription || "";

    const previewContainer = document.getElementById("previewFormContainer");
    previewContainer.innerHTML = ""; // Clear previous preview

    formFields.forEach((field) => {
        let requiredMark = field.isRequired ? `<span class="text-danger fw-bold">*</span>` : "";
        let fieldHtml = `<div class="mb-3">
            <label class="form-label fw-bold">${field.fieldName} ${requiredMark}</label>`;

        switch (field.fieldType) {
            case "text":
                fieldHtml += `<input type="text" class="form-control" ${field.isRequired ? "required" : ""} />`;
                break;
            case "textarea":
                fieldHtml += `<textarea class="form-control" ${field.isRequired ? "required" : ""}></textarea>`;
                break;
            case "select":
                fieldHtml += `<select class="form-select" ${field.isRequired ? "required" : ""}>`;

                if (field.options && field.options.length > 0) {
                    field.options.forEach(option => {
                        fieldHtml += `<option>${option.OptionText || "Unnamed Option"}</option>`;
                    });
                } else {
                    fieldHtml += `<option disabled>No options available</option>`;
                }

                fieldHtml += `</select>`;
                break;
            case "checkbox":
                if (field.options && field.options.length > 0) {
                    field.options.forEach(option => {
                        fieldHtml += `<div class="form-check">
                            <input class="form-check-input" type="checkbox">
                            <label class="form-check-label">${option.OptionText || "Unnamed Option"}</label> 
                        </div>`;
                    });
                } else {
                    fieldHtml += `<p class="text-muted">No options available</p>`;
                }
                break;
            case "file":
                fieldHtml += `<input type="file" class="form-control" ${field.isRequired ? "required" : ""} />`;
                break;
            case "rating":
                fieldHtml += `<div class="rating-container d-flex gap-1" data-name="Fields[${field.order}].Rating">
                    <span class="rating-star" data-value="1">⭐</span>
                    <span class="rating-star" data-value="2">⭐</span>
                    <span class="rating-star" data-value="3">⭐</span>
                    <span class="rating-star" data-value="4">⭐</span>
                    <span class="rating-star" data-value="5">⭐</span>
                </div>`;
                break;
            case "date":
                fieldHtml += `<input type="date" class="form-control" ${field.isRequired ? "required" : ""} />`;
                break;
            case "phoneNumber":
                fieldHtml += `<input type="tel" class="form-control" placeholder="Enter phone number" ${field.isRequired ? "required" : ""} />`;
                break;
            case "email":
                fieldHtml += `<input type="email" class="form-control" placeholder="Enter email address" ${field.isRequired ? "required" : ""} />`;
                break;
        }

        fieldHtml += `</div>`; // Close field div
        previewContainer.innerHTML += fieldHtml;
    });

    // Show modal
    const previewModal = new bootstrap.Modal(document.getElementById("formPreviewModal"));
    previewModal.show();
}

